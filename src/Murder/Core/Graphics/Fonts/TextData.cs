using Murder.Utilities;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Globalization;
using System.Text;

namespace Murder.Core.Graphics;

public record struct RuntimeTextDataKey(string Text, int Font, int Width) { }

public enum RuntimeLetterPropertiesFlag
{
    /// <summary>
    /// They ~wave~ while being displayed. 
    /// </summary>
    Wave = 0b1,

    /// <summary>
    /// They are in !FEAR! while being displayed.
    /// </summary>
    Fear = 0b10,

    /// <summary>
    /// Properties that guarantees that the writer does NOT skip this
    /// index when calculating. For example, '\n' will be ignored by
    /// default unless this is present.
    /// </summary>
    DoNotSkipLineEnding = 0b100,

    /// <summary>
    /// Reset any color.
    /// </summary>
    ResetColor = 0b1000,

    /// <summary>
    /// Reset any speed.
    /// </summary>
    ResetSpeed = 0b10000,

    /// <summary>
    /// Reset any glitch.
    /// </summary>
    ResetGlitch = 0b100000,
}

/// <summary>
/// Properties of a letter when printing it.
/// </summary>
public readonly record struct RuntimeLetterProperties
{
    public RuntimeLetterPropertiesFlag Properties { get; init; }

    /// <summary>
    /// Amount of pause after this letter is printed.
    /// </summary>
    public int Pause { get; init; }

    /// <summary>
    /// Amount of small pauses after this letter is printed.
    /// </summary>
    public int SmallPause { get; init; }

    /// <summary>
    /// Whether this will trigger a !SHAKE! (and intensity).
    /// </summary>
    public float Shake { get; init; }

    /// <summary>
    /// Whether this will trigger a ~GLITCH~.
    /// </summary>
    public float Glitch { get; init; }

    public Color? Color { get; init; }

    /// <summary>
    /// Override the text speed from this letter on.
    /// </summary>
    public float Speed { get; init; }

    public RuntimeLetterProperties CombineWith(RuntimeLetterProperties other)
    {
        return new RuntimeLetterProperties() with
        {
            Properties = Properties | other.Properties,
            Pause = Pause != 0 ? Pause : other.Pause,
            Shake = Shake != 0 ? Shake : other.Shake,
            Glitch = Glitch != 0 ? Glitch : other.Glitch,
            Color = Color ?? other.Color,
            Speed = Speed != 0 ? Speed : other.Speed
        };
    }
}

/// <summary>
/// This has runtime information about a text which is displayed in screen.
/// </summary>
public readonly struct RuntimeTextData
{
    public readonly string Text = string.Empty;

    private readonly ImmutableDictionary<int, RuntimeLetterProperties>? _letters = null;

    /// <summary>
    /// Index of the font used to calculate the runtime text data.
    /// </summary>
    public readonly int Font { get; init; } = 0;

    /// <summary>
    /// Whether this is high resolution.
    /// </summary>
    public readonly bool HiRes { get; init; } = false;

    public RuntimeTextData() { }

    public RuntimeTextData(string text) =>
        Text = text;

    public RuntimeTextData(string text, ImmutableDictionary<int, RuntimeLetterProperties>? letters) : this(text) => 
        _letters = letters;

    public bool Empty => string.IsNullOrEmpty(Text);

    public int Length => Text.Length;

    public RuntimeLetterProperties? TryGetLetterProperty(int index)
    {
        if (_letters is null)
        {
            return null;
        }

        if (!_letters.TryGetValue(index, out RuntimeLetterProperties properties))
        {
            return null;
        }

        return properties;
    }
}

public readonly struct TextSettings
{
    public readonly Vector2 Scale { get; init; } = Vector2.One;

    public readonly bool HiRes { get; init; } = false;

    public readonly int MaxWidth { get; init; } = -1;

    public TextSettings() { }
}

public static partial class TextDataServices
{
    private enum LetterSkipMetadataFlag
    {
        /// <summary>
        /// None, letter won't be skipped.
        /// </summary>
        None = 0,
        Pause = 1 << 1,
        Shake = 1 << 2,
        Glitch = 1 << 3,
        Color = 1 << 4,
        Wave = 1 << 5,
        Fear = 1 << 6,
        Speed = 1 << 7
    }

    // [Perf] Cache the last strings parsed.
    private static readonly CacheDictionary<RuntimeTextDataKey, RuntimeTextData> _cache = new(64);

    public static RuntimeTextData GetOrCreateText(int fontId, string text, TextSettings settings)
    {
        PixelFont font = Game.Data.GetFont(fontId);
        return GetOrCreateText(font.PixelFontSize, text, settings);
    }

    public static RuntimeTextData GetOrCreateText(PixelFontSize font, string text, TextSettings settings)
    {
        RuntimeTextDataKey key = new(text, font.Index, settings.MaxWidth);
        if (_cache.TryGetValue(key, out RuntimeTextData data))
        {
            return data;
        }

        // Replace single newline with space
        text = ReplaceSingleNewLine().Replace(text, " ");

        // Replace two consecutive newlines with a single one
        text = ReplaceTwoNewLines().Replace(text, "\n");

        // Replace two or more spaces with a single one
        text = TrimSpaces().Replace(text, " ");

        // Look for pause characters: |
        MatchCollection matchesForPauses = TrimPauses().Matches(text);

        // Look for shake characters: <shake/>
        MatchCollection matchesForShakes = ShakeTags().Matches(text);

        // Look for glitch characters: <glitch=.3/>
        MatchCollection matchesForGlitches = GlitchTags().Matches(text);

        // Look for colors: <c=#fff>text</c>
        MatchCollection matchesForColors = ColorTags().Matches(text);

        // Look for pause characters: <wave>text</wave>
        MatchCollection matchesForWaves = WaveTags().Matches(text);

        // Look for fear characters: <fear>text</fear>
        MatchCollection matchesForFear = FearTags().Matches(text);

        // Look for speed characters: <speed=.3/>
        MatchCollection matchesForSpeed = SpeedTags().Matches(text);

        int allocatedLengthForSpecialCharacters = 0;
        if (matchesForPauses.Count != 0 || matchesForShakes.Count != 0 || matchesForColors.Count != 0 ||
            matchesForWaves.Count != 0 || matchesForFear.Count != 0 || matchesForSpeed.Count != 0 || matchesForGlitches.Count != 0)
        {
            allocatedLengthForSpecialCharacters = text.Length;
        }

        // Tracks the data for this letter.
        Span<RuntimeLetterProperties> lettersBuilder = stackalloc RuntimeLetterProperties[allocatedLengthForSpecialCharacters];

        // Tracks all the letters in rawText which were skipped.
        Span<LetterSkipMetadataFlag> skippedLetters = stackalloc LetterSkipMetadataFlag[allocatedLengthForSpecialCharacters];

        if (matchesForPauses.Count > 0)
        {
            for (int i = 0; i < matchesForPauses.Count; ++i)
            {
                Match match = matchesForPauses[i];

                // Mark all the characters that matched that they will be skipped.
                for (int j = 0; j < match.Length; ++j)
                {
                    skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Pause;
                }

                // Track that there is a pause at this index.
                int index = Math.Max(0, match.Index - 2);
                lettersBuilder[index] = lettersBuilder[index] with { Pause = match.Length };
            }
        }

        if (matchesForShakes.Count > 0)
        {
            for (int i = 0; i < matchesForShakes.Count; ++i)
            {
                Match match = matchesForShakes[i];

                // Mark all the characters that matched that they will be skipped.
                for (int j = 0; j < match.Length; ++j)
                {
                    skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Shake;
                }

                int index = Math.Min(lettersBuilder.Length - 1, match.Index + match.Length);

                // TODO: I haven't tested this yet. This might be wrong.
                float shake = 1;
                if (match.Groups.Count > 1)
                {
                    shake = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                }

                lettersBuilder[index] = lettersBuilder[index] with { Shake = shake };
            }
        }

        if (matchesForGlitches.Count > 0)
        {
            for (int i = 0; i < matchesForGlitches.Count; ++i)
            {
                Match match = matchesForGlitches[i];

                // Mark all the characters that matched that they will be skipped.
                for (int j = 0; j < match.Length; ++j)
                {
                    skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Glitch;
                }

                // TODO: I haven't tested this yet. This might be wrong.
                float glitch = 1;
                if (match.Groups[3].Length == 0)
                {
                    glitch = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                    int index = Math.Min(lettersBuilder.Length - 1, match.Index + match.Length);
                    lettersBuilder[index] = lettersBuilder[index] with { Glitch = glitch };
                }
                else
                {
                    glitch = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

                    Group textGroup = match.Groups[3];
                    for (int j = 0; j < textGroup.Length; ++j)
                    {
                        skippedLetters[textGroup.Index + j] &= ~LetterSkipMetadataFlag.Glitch;
                    }

                    // Map the start of this current text as the speed switch.
                    lettersBuilder[match.Index] = lettersBuilder[match.Index] with { Glitch = glitch };

                    RuntimeLetterProperties l = lettersBuilder[match.Index + match.Length - 1];
                    lettersBuilder[match.Index + match.Length - 1] = lettersBuilder[match.Index + match.Length - 1]
                        with
                    { Properties = l.Properties | RuntimeLetterPropertiesFlag.ResetGlitch };
                }
            }
        }

        if (matchesForSpeed.Count > 0)
        {
            for (int i = 0; i < matchesForSpeed.Count; ++i)
            {
                Match match = matchesForSpeed[i];

                // Mark all the characters that matched that they will be skipped.
                for (int j = 0; j < match.Length; ++j)
                {
                    skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Speed;
                }

                float speed = 1;

                if (match.Groups[3].Length == 0)
                {
                    int index = Math.Min(lettersBuilder.Length - 1, match.Index + match.Length);
                    speed = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                    lettersBuilder[index] = lettersBuilder[index] with { Speed = speed };
                }
                else
                {
                    speed = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

                    Group textGroup = match.Groups[3];
                    for (int j = 0; j < textGroup.Length; ++j)
                    {
                        skippedLetters[textGroup.Index + j] &= ~LetterSkipMetadataFlag.Speed;
                    }

                    // Map the start of this current text as the speed switch.
                    lettersBuilder[match.Index] = lettersBuilder[match.Index] with { Speed = speed };

                    RuntimeLetterProperties l = lettersBuilder[match.Index + match.Length - 1];
                    lettersBuilder[match.Index + match.Length - 1] = lettersBuilder[match.Index + match.Length - 1]
                        with { Properties = l.Properties | RuntimeLetterPropertiesFlag.ResetSpeed };
                }
            }
        }

        if (matchesForColors.Count > 0)
        {
            for (int i = 0; i < matchesForColors.Count; i++)
            {
                Match match = matchesForColors[i];

                Color colorForText = Color.FromName(match.Groups[1].Value);

                Group textGroup = match.Groups[2];

                // Mark all the characters that matched that they will be skipped.
                for (int j = 0; j < match.Length; ++j)
                {
                    int index = match.Index + j;
                    if (index >= textGroup.Index && index < textGroup.Index + textGroup.Length)
                    {
                        continue;
                    }

                    skippedLetters[index] |= LetterSkipMetadataFlag.Color;
                }

                // Map the start of this current text as the color switch.
                lettersBuilder[match.Index] = lettersBuilder[match.Index] with { Color = colorForText };

                RuntimeLetterProperties l = lettersBuilder[match.Index + match.Length - 1];
                lettersBuilder[match.Index + match.Length - 1] = lettersBuilder[match.Index + match.Length - 1] 
                    with { Properties = l.Properties | RuntimeLetterPropertiesFlag.ResetColor };
            }
        }

        if (matchesForWaves.Count > 0)
        {
            for (int i = 0; i < matchesForWaves.Count; ++i)
            {
                Match match = matchesForWaves[i];

                for (int j = 0; j < match.Length; ++j)
                {
                    skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Wave;
                }

                Group group = match.Groups[1];

                for (int j = 0; j < group.Length; ++j)
                {
                    int index = group.Index + j;
                    skippedLetters[index] &= ~LetterSkipMetadataFlag.Wave;

                    RuntimeLetterProperties l = lettersBuilder[index];
                    lettersBuilder[index] = l with { Properties = l.Properties | RuntimeLetterPropertiesFlag.Wave };
                }
            }
        }

        if (matchesForFear.Count > 0)
        {
            for (int i = 0; i < matchesForFear.Count; ++i)
            {
                Match match = matchesForFear[i];

                for (int j = 0; j < match.Length; ++j)
                {
                    skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Fear;
                }

                Group group = match.Groups[1];

                for (int j = 0; j < group.Length; ++j)
                {
                    int index = group.Index + j;
                    skippedLetters[index] &= ~LetterSkipMetadataFlag.Fear;

                    RuntimeLetterProperties l = lettersBuilder[index];
                    lettersBuilder[index] = l with { Properties = l.Properties | RuntimeLetterPropertiesFlag.Fear };
                }
            }
        }

        ImmutableDictionary<int, RuntimeLetterProperties>.Builder? letters = ImmutableDictionary.CreateBuilder<int, RuntimeLetterProperties>();

        string parsedText;
        if (allocatedLengthForSpecialCharacters == 0)
        {
            // Great! There was no data to parse! Just return right away!
            parsedText = text;
        }
        else
        {
            StringBuilder result = new();

            // Oh boy, we got data to parse.
            int finalIndex = 0;
            int currentIndex = 0;

            Span<int> indices = stackalloc int[text.Length];
            for (currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                indices[currentIndex] = finalIndex;

                if (skippedLetters[currentIndex] != LetterSkipMetadataFlag.None)
                {
                    continue;
                }

                finalIndex++;

                char c = text[currentIndex];
                result.Append(c);

                // Look, I also don't think this is correct. But I am still procrastinating doing the right solution for this.
                // So I'll just make sure existing \n do not mess up the color calculation since we are skipping \n
                // when calculating the right color index.
                if (c == '\n')
                {
                    RuntimeLetterProperties l = lettersBuilder[currentIndex];
                    lettersBuilder[currentIndex] = l with { Properties = l.Properties | RuntimeLetterPropertiesFlag.DoNotSkipLineEnding };
                }
            }

            for (int i = 0; i < indices.Length; ++i)
            {
                RuntimeLetterProperties properties = lettersBuilder[i];
                if (properties == default)
                {
                    continue;
                }

                int indexInFinalString = indices[i];

                if (letters.TryGetValue(indexInFinalString, out RuntimeLetterProperties previousProperties))
                {
                    letters[indexInFinalString] = previousProperties.CombineWith(properties);
                }
                else
                {
                    letters[indexInFinalString] = properties;
                }
            }

            parsedText = result.ToString();
        }

        // Now, manually apply pauses according to the pontuaction.
        // We don't bother adding this to very last character, though.
        for (int i = 0; i < parsedText.Length - 1; ++i)
        {
            // For now, do this weird heuristic of not applying pause if the last character was uppercase.
            bool shouldSkipPause = i != 0 && char.IsUpper(parsedText[i - 1]);
            if (shouldSkipPause)
            {
                continue;
            }

            char c = parsedText[i];
            if (!letters.TryGetValue(i, out RuntimeLetterProperties l))
            {
                l = new();
            }

            switch (c)
            {
                case '!':
                // case ',': // I don't like this pause here.
                case ':':
                case '?':
                    letters[i] = l with { SmallPause = l.SmallPause + 1 };
                    break;

                case '.':
                    if (l.Pause == 0)
                    {
                        letters[i] = l with { Pause = 1 };
                    }

                    break;
            }
        }

        if (settings.MaxWidth > 0)
        {
            string wrappedText = font.WrapString(parsedText, settings.MaxWidth, settings.Scale.X);
            parsedText = wrappedText.ToString();
        }

        data = new RuntimeTextData(parsedText, letters?.ToImmutable()) with
        {
            Font = font.Index,
            HiRes = settings.HiRes
        };

        _cache[key] = data;
        return data;
    }

    [GeneratedRegex("(?<!\n)\n(?!\n)")]
    private static partial Regex ReplaceSingleNewLine();

    [GeneratedRegex("\n\n|<nl>")]
    private static partial Regex ReplaceTwoNewLines();

    [GeneratedRegex(" {2,}")]
    public static partial Regex TrimSpaces();

    [GeneratedRegex("<c=([^>]+)>|</c>", RegexOptions.IgnoreCase)]
    public static partial Regex EscapeRegex();

    [GeneratedRegex("\\|{1,}")]
    private static partial Regex TrimPauses();

    [GeneratedRegex("<shake=([^\\/]+)\\/>|<shake\\/>", RegexOptions.IgnoreCase)]
    private static partial Regex ShakeTags();

    [GeneratedRegex("<glitch=([^\\/]+)\\/>|<glitch=([^>]+)>(.*?)</glitch>", RegexOptions.IgnoreCase)]
    private static partial Regex GlitchTags();

    [GeneratedRegex("<c=([^>]+)>(.*?)</c>", RegexOptions.IgnoreCase)]
    private static partial Regex ColorTags();

    [GeneratedRegex("<wave>(.*?)<\\/wave>", RegexOptions.IgnoreCase)]
    private static partial Regex WaveTags();

    [GeneratedRegex("<fear>(.*?)<\\/fear>", RegexOptions.IgnoreCase)]
    private static partial Regex FearTags();

    [GeneratedRegex("<speed=([^\\/]+)\\/>|<speed=([^>]+)>(.*?)</speed>", RegexOptions.IgnoreCase)]
    private static partial Regex SpeedTags();
}
