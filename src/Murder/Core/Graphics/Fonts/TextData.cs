using Murder.Assets;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

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
    /// Reset color property.
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

    /// <summary>
    /// Reset outline property.
    /// </summary>
    ResetOutline = 0b1000,

    /// <summary>
    /// Reset shadow property.
    /// </summary>
    ResetShadow = 0b1000,

    /// <summary>
    /// Reset current profile.
    /// </summary>
    ResetProfile = 0b1000,

    /// <summary>
    /// Signalizes that a new information was presented with <new>info</new>.
    /// </summary>
    New = 0b1000000
}

public enum LetterSkipMetadataFlag
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
    Speed = 1 << 7,
    Icon = 1 << 8,
    New = 1 << 9,
    Shadow = 1 << 10,
    Outline = 1 << 11
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

    public Color? OutlineColor { get; init; }

    public Color? ShadowColor { get; init; }

    public Portrait? Icon { get; init; }

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
            OutlineColor = OutlineColor ?? other.OutlineColor,
            ShadowColor = ShadowColor ?? other.ShadowColor,
            Icon = Icon ?? other.Icon,
            Speed = Speed != 0 ? Speed : other.Speed
        };
    }

    public RuntimeLetterProperties WithColor(LetterSkipMetadataFlag metadata, Color color)
    {
        switch (metadata)
        {
            case LetterSkipMetadataFlag.Color:
                return this with { Color = color };

            case LetterSkipMetadataFlag.Shadow:
                return this with { ShadowColor = color };

            case LetterSkipMetadataFlag.Outline:
                return this with { OutlineColor = color };
        }

        return this;
    }

    public RuntimeLetterProperties Reset(LetterSkipMetadataFlag metadata)
    {
        RuntimeLetterPropertiesFlag resetFlag = metadata switch
        {
            LetterSkipMetadataFlag.Shadow => RuntimeLetterPropertiesFlag.ResetShadow,
            LetterSkipMetadataFlag.Outline => RuntimeLetterPropertiesFlag.ResetOutline,
            _ => RuntimeLetterPropertiesFlag.ResetColor,
        };

        return this with { Properties = Properties | resetFlag };
    }
}

/// <summary>
/// This has runtime information about a text which is displayed in screen.
/// </summary>
public readonly struct RuntimeTextData
{
    public readonly string Text = string.Empty;

    public readonly ImmutableDictionary<int, RuntimeLetterProperties>? Letters { get; init; } = null;

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
        Letters = letters;

    public bool Empty => string.IsNullOrEmpty(Text);

    public int Length => Text.Length;

    public RuntimeLetterProperties? TryGetLetterProperty(int index)
    {
        if (Letters is null)
        {
            return null;
        }

        if (!Letters.TryGetValue(index, out RuntimeLetterProperties properties))
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

        // Replace two consecutive newlines with a single one
        text = ReplaceBreakpointOnWords().Replace(text, $"{TextDataServices.SPECIAL_BREAK_WORD_CHARACTER}");

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

        // Look for colors: <o=#fff>text</o>
        MatchCollection matchesForOutline = OutlineTags().Matches(text);

        // Look for colors: <s=#fff>text</s>
        MatchCollection matchesForShadow = ShadowTags().Matches(text);

        // Look for colors: <t=1>text</t> or <t>text</t>
        MatchCollection matchesForTextTheme = ThemeTags().Matches(text);

        // Look for pause characters: <wave>text</wave>
        MatchCollection matchesForWaves = WaveTags().Matches(text);

        // Look for fear characters: <fear>text</fear>
        MatchCollection matchesForFear = FearTags().Matches(text);

        // Look for fear characters: <new>text</new>
        MatchCollection matchesForNew = NewTags().Matches(text);

        // Look for speed characters: <speed=.3/>
        MatchCollection matchesForSpeed = SpeedTags().Matches(text);

        // Look for speed characters: <icon=xp/>
        MatchCollection matchesForIcons = IconTags().Matches(text);

        int allocatedLengthForSpecialCharacters = 0;
        if (matchesForPauses.Count != 0 || matchesForShakes.Count != 0 || matchesForColors.Count != 0 ||
            matchesForWaves.Count != 0 || matchesForFear.Count != 0 || matchesForSpeed.Count != 0 || matchesForGlitches.Count != 0 ||
            matchesForIcons.Count != 0 || matchesForNew.Count != 0 || matchesForOutline.Count != 0 || matchesForShadow.Count != 0 ||
            matchesForTextTheme.Count != 0)
        {
            allocatedLengthForSpecialCharacters = text.Length;
        }

        // Tracks the data for this letter.
        Span<RuntimeLetterProperties> lettersBuilder = new RuntimeLetterProperties[allocatedLengthForSpecialCharacters];

        // Tracks all the letters in rawText which were skipped.
        Span<LetterSkipMetadataFlag> skippedLetters = stackalloc LetterSkipMetadataFlag[allocatedLengthForSpecialCharacters];

        if (allocatedLengthForSpecialCharacters > 0)
        {
            if (matchesForPauses.Count > 0)
            {
                // in japanese and chinese, we don't use the ' ' space character, so the pause will apply before showing the next letter.
                // otherwise, the pause applies before showing the last letter.
                bool pauseBeforeShowingCharacter = LocalizationServices.IsTextWrapOnlyOnSpace();

                for (int i = 0; i < matchesForPauses.Count; ++i)
                {
                    Match match = matchesForPauses[i];

                    // Mark all the characters that matched that they will be skipped.
                    for (int j = 0; j < match.Length; ++j)
                    {
                        skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Pause;
                    }

                    // Track that there is a pause at this index.
                    int index = Math.Max(0, match.Index - (pauseBeforeShowingCharacter ? 2 : 1));
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
                        shake = InputServices.ParseFloatSafe(match.Groups[1].Value);
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
                        glitch = InputServices.ParseFloatSafe(match.Groups[1].Value);

                        int index = Math.Min(lettersBuilder.Length - 1, match.Index + match.Length);
                        lettersBuilder[index] = lettersBuilder[index] with { Glitch = glitch };
                    }
                    else
                    {
                        glitch = InputServices.ParseFloatSafe(match.Groups[2].Value);

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
                        speed = InputServices.ParseFloatSafe(match.Groups[1].Value);

                        lettersBuilder[index] = lettersBuilder[index] with { Speed = speed };
                    }
                    else
                    {
                        speed = InputServices.ParseFloatSafe(match.Groups[2].Value);

                        Group textGroup = match.Groups[3];
                        for (int j = 0; j < textGroup.Length; ++j)
                        {
                            skippedLetters[textGroup.Index + j] &= ~LetterSkipMetadataFlag.Speed;
                        }

                        // Map the start of this current text as the speed switch.
                        lettersBuilder[match.Index] = lettersBuilder[match.Index] with { Speed = speed };

                        RuntimeLetterProperties l = lettersBuilder[match.Index + match.Length - 1];
                        lettersBuilder[match.Index + match.Length - 1] = lettersBuilder[match.Index + match.Length - 1]
                            with
                        { Properties = l.Properties | RuntimeLetterPropertiesFlag.ResetSpeed };
                    }
                }
            }

            if (matchesForColors.Count > 0)
            {
                ApplyTaggedProperty(matchesForColors, LetterSkipMetadataFlag.Color, lettersBuilder, skippedLetters);
            }

            if (matchesForOutline.Count > 0)
            {
                ApplyTaggedProperty(matchesForOutline, LetterSkipMetadataFlag.Outline, lettersBuilder, skippedLetters);
            }

            if (matchesForShadow.Count > 0)
            {
                ApplyTaggedProperty(matchesForShadow, LetterSkipMetadataFlag.Shadow, lettersBuilder, skippedLetters);
            }

            if (matchesForTextTheme.Count > 0)
            {
                ApplyTextTheme(matchesForTextTheme, lettersBuilder, skippedLetters);
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

            if (matchesForNew.Count > 0)
            {
                for (int i = 0; i < matchesForNew.Count; ++i)
                {
                    Match match = matchesForNew[i];

                    for (int j = 0; j < match.Length; ++j)
                    {
                        skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.New;
                    }

                    Group group = match.Groups[1];

                    for (int j = 0; j < group.Length; ++j)
                    {
                        int index = group.Index + j;
                        skippedLetters[index] &= ~LetterSkipMetadataFlag.New;

                        RuntimeLetterProperties l = lettersBuilder[index];
                        lettersBuilder[index] = l with { Properties = l.Properties | RuntimeLetterPropertiesFlag.New };
                    }
                }
            }

            if (matchesForIcons.Count > 0)
            {
                for (int i = 0; i < matchesForIcons.Count; ++i)
                {
                    Match match = matchesForIcons[i];

                    // BUT leave the first character alone, we'll use that.
                    // Mark all the characters that matched that they will be skipped.
                    for (int j = 1; j < match.Length; ++j)
                    {
                        skippedLetters[match.Index + j] |= LetterSkipMetadataFlag.Icon;
                    }

                    string iconName = match.Groups[1].Value;

                    if (MurderFontServices.TryGetIconForText(iconName) is Portrait portrait)
                    {
                        // Track that there is a pause at this index.
                        int index = Math.Max(0, match.Index);
                        lettersBuilder[index] = lettersBuilder[index] with { Icon = portrait };
                    }
                }
            }
        }

        ImmutableDictionary<int, RuntimeLetterProperties>.Builder? letters = ImmutableDictionary.CreateBuilder<int, RuntimeLetterProperties>();

        int indexForPause = 0;

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

                if (lettersBuilder[currentIndex].Icon is not null)
                {
                    // hardcode a 'M' character for icons when measuring the line length.
                    c = 'M';
                }

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

                // Now, we need to do a workaround (I'm sorry). We are actually bypassing the last index when applying a pause.
                // So, if the current character is skipped, we actually want to apply the pause to one index BEFORE.
                if (indexInFinalString > 0 && skippedLetters[i] != LetterSkipMetadataFlag.None && properties.Pause > 0)
                {
                    indexForPause = indexInFinalString - 1;

                    RuntimeLetterProperties propertiesButWithPauseOnly = new() { Pause = properties.Pause };
                    if (letters.TryGetValue(indexForPause, out RuntimeLetterProperties previousPropertiesToApplyPause))
                    {
                        letters[indexForPause] = previousPropertiesToApplyPause.CombineWith(propertiesButWithPauseOnly);
                    }
                    else
                    {
                        letters[indexForPause] = propertiesButWithPauseOnly;
                    }

                    // finally, remove the pause from our current property...
                    properties = properties with { Pause = 0 };
                }

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

        indexForPause = 0;

        // Now, manually apply pauses according to the pontuaction.
        // We don't bother adding this to very last character, though.
        for (int i = 0; i < parsedText.Length - 1 && indexForPause < parsedText.Length; ++i, ++indexForPause)
        {
            // For now, do this weird heuristic of not applying pause if the last character was uppercase.
            bool shouldSkipPause = (i > 0 && char.IsUpper(parsedText[i - 1])) || 
                IsPunctuationToIgnorePreviousPause(parsedText[i + 1]);

            if (shouldSkipPause)
            {
                continue;
            }

            if (!letters.TryGetValue(i, out RuntimeLetterProperties l))
            {
                l = new();
            }

            char c = parsedText[i];
            if (c == '\n' && !l.Properties.HasFlag(RuntimeLetterPropertiesFlag.DoNotSkipLineEnding))
            {
                indexForPause = Math.Max(0, indexForPause - 1);
            }

            switch (c)
            {
                // case ',': // I don't like this pause here.
                case '!':
                case '！':
                case ':':
                case '?':
                case '？':
                case '、':
                    letters[indexForPause] = l with { SmallPause = l.SmallPause + 1 };
                    break;

                case '.':
                    // check if this is actually an ellipsis...
                    if (parsedText[indexForPause + 1] == '.')
                    {
                        // pass on that pause
                        letters[indexForPause + 1] = l with { Pause = l.Pause + 1 };

                        // (removed) keeping a small pause for ourselves
                        // SmallPause = l.SmallPause + 1 }; 
                        letters[indexForPause] = l with { Pause = 0 }; 
                    }
                    else if (l.Pause == 0)
                    {
                        letters[indexForPause] = l with { Pause = 1 };
                    }

                    break;

                case '。':
                    if (l.Pause == 0)
                    {
                        letters[indexForPause] = l with { Pause = 1 };
                    }

                    break;

                case '…':
                    if (l.Pause == 0)
                    {
                        letters[indexForPause] = l with { Pause = 2 };
                    }

                    break;
            }
        }

        if (settings.MaxWidth > 0)
        {
            parsedText = font.WrapString(parsedText, settings.MaxWidth, settings.Scale.X);
        }

        data = new RuntimeTextData(parsedText, letters?.ToImmutable()) with
        {
            Font = font.Index,
            HiRes = settings.HiRes
        };

        _cache[key] = data;
        return data;
    }

    /// <summary>
    /// Apply a property that has a value specified, e.g. <c=value></c>.
    /// </summary>
    private static void ApplyTaggedProperty(
        MatchCollection matches,
        LetterSkipMetadataFlag metadata, 
        Span<RuntimeLetterProperties> lettersBuilder,
        Span<LetterSkipMetadataFlag> skippedLetters)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            Group textGroup = match.Groups[2];

            // Mark all the characters that matched that they will be skipped.
            for (int j = 0; j < match.Length; ++j)
            {
                int index = match.Index + j;
                if (index >= textGroup.Index && index < textGroup.Index + textGroup.Length)
                {
                    continue;
                }

                skippedLetters[index] |= metadata;
            }

            // Map the start of this current text as the color switch.
            Color colorForText = Color.FromName(match.Groups[1].Value);

            lettersBuilder[match.Index] = lettersBuilder[match.Index].WithColor(metadata, colorForText);
            lettersBuilder[match.Index + match.Length - 1] = lettersBuilder[match.Index + match.Length - 1].Reset(metadata);
        }
    }

    /// <summary>
    /// Apply a property that has a value specified, e.g. <c=value></c>.
    /// </summary>
    private static void ApplyTextTheme(
        MatchCollection matches,
        Span<RuntimeLetterProperties> lettersBuilder,
        Span<LetterSkipMetadataFlag> skippedLetters)
    {
        ImmutableArray<TextTheme> themes = Game.Profile.TextThemeProfiles;
        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];

            bool hasSpecifiedTheme = match.Groups[1].Length > 0;

            TextTheme theme;
            int chosenTheme = !hasSpecifiedTheme ? 0 : InputServices.ParseIntSafe(match.Groups[1].Value);
            if (chosenTheme < 0 || chosenTheme >= themes.Length)
            {
                // if there is no theme, still apply the tag to skip it.
                theme = new();
            }
            else
            {
                theme = themes[chosenTheme];
            }

            // <t=([^>]+)>(.*?)</t>|<t>(.*?)</t>
            //    ^ group 1 ^ group 2   ^ group 3
            Group textGroup = match.Groups[hasSpecifiedTheme ? 2 : 3];

            // Mark all the characters that matched that they will be skipped.
            for (int j = 0; j < match.Length; ++j)
            {
                int index = match.Index + j;
                if (index >= textGroup.Index && index < textGroup.Index + textGroup.Length)
                {
                    continue;
                }

                // either of them should work here, we just want to tag them to be skipped.
                skippedLetters[index] |= LetterSkipMetadataFlag.Color;
            }

            if (theme.Empty)
            {
                return;
            }

            int endIndex = match.Index + match.Length - 1;

            Color? color = theme.Color;
            if (color is not null)
            {
                lettersBuilder[match.Index] = lettersBuilder[match.Index].WithColor(LetterSkipMetadataFlag.Color, color.Value);
                lettersBuilder[endIndex] = lettersBuilder[endIndex].Reset(LetterSkipMetadataFlag.Color);
            }

            color = theme.Outline;
            if (color is not null)
            {
                lettersBuilder[match.Index] = lettersBuilder[match.Index].WithColor(LetterSkipMetadataFlag.Outline, color.Value);
                lettersBuilder[endIndex] = lettersBuilder[endIndex].Reset(LetterSkipMetadataFlag.Outline);
            }

            color = theme.Shadow;
            if (color is not null)
            {
                lettersBuilder[match.Index] = lettersBuilder[match.Index].WithColor(LetterSkipMetadataFlag.Shadow, color.Value);
                lettersBuilder[endIndex] = lettersBuilder[endIndex].Reset(LetterSkipMetadataFlag.Shadow);
            }
        }
    }

    public static bool IsPunctuation(char c)
    {
        return c switch
        {
            '!' or '！' or ':' or '?' or '？' or '、' or '.' or '。' or '…' => true,
            _ => false,
        };
    }

    public static bool IsPunctuationToIgnorePreviousPause(char c)
    {
        return c switch
        {
            '!' or '！' or ':' or '?' or '？' or '\"' => true,
            _ => false,
        };
    }

    /// <summary>
    /// Used on editor, mostly.
    /// </summary>
    public static void ClearCache()
    {
        _cache.Clear();
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

    [GeneratedRegex("<o=([^>]+)>(.*?)</o>", RegexOptions.IgnoreCase)]
    private static partial Regex OutlineTags();

    [GeneratedRegex("<s=([^>]+)>(.*?)</s>", RegexOptions.IgnoreCase)]
    private static partial Regex ShadowTags();

    [GeneratedRegex("<t=([^>]+)>(.*?)</t>|<t>(.*?)</t>", RegexOptions.IgnoreCase)]
    private static partial Regex ThemeTags();

    [GeneratedRegex("<wave>(.*?)<\\/wave>", RegexOptions.IgnoreCase)]
    private static partial Regex WaveTags();

    [GeneratedRegex("<fear>(.*?)<\\/fear>", RegexOptions.IgnoreCase)]
    private static partial Regex FearTags();

    [GeneratedRegex("<new>(.*?)<\\/new>", RegexOptions.IgnoreCase)]
    private static partial Regex NewTags();

    [GeneratedRegex("<speed=([^\\/]+)\\/>|<speed=([^>]+)>(.*?)</speed>", RegexOptions.IgnoreCase)]
    private static partial Regex SpeedTags();

    [GeneratedRegex("<icon=([^>]+)>", RegexOptions.IgnoreCase)]
    private static partial Regex IconTags();

    [GeneratedRegex("<bp>")]
    private static partial Regex ReplaceBreakpointOnWords();

    public const char SPECIAL_BREAK_WORD_CHARACTER = '\a';
}
