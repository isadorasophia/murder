using Murder.Utilities;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Text;
using System.Numerics;

namespace Murder.Core.Graphics;

public record struct RuntimeTextDataKey(string Text, int Font, int Width) { }

/// <summary>
/// This has runtime information about a text which is displayed in screen.
/// </summary>
public readonly struct RuntimeTextData
{
    public readonly string Text = string.Empty;

    public readonly ImmutableDictionary<int, Color?>? Colors { get; init; } = null;
    public readonly ImmutableHashSet<int>? NonSkippableLineEnding { get; init; } = null;

    public readonly int ExtraCharacters { get; init; } = 0;

    public readonly int Font { get; init; } = 0;
    public readonly bool HiRes { get; init; } = false;

    public RuntimeTextData() { }

    public RuntimeTextData(string text) => Text = text;

    public bool Empty => string.IsNullOrEmpty(Text);

    public int Length => Text.Length;
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

        ImmutableDictionary<int, Color?>.Builder? colors = null;
        ImmutableHashSet<int>.Builder? nonSkippableLineEnding = null;

        StringBuilder result = new();

        // Replace single newline with space
        text = ReplaceSingleNewLine().Replace(text, " ");

        // Replace two consecutive newlines with a single one
        text = ReplaceTwoNewLines().Replace(text, "\n");

        // Replace two or more spaces with a single one
        text = TrimSpaces().Replace(text, " ");

        int lastIndex = 0;
        ReadOnlySpan<char> rawText = text;

        MatchCollection matches = ColorTags().Matches(text);
        if (matches.Count > 0)
        {
            // Map the color indices according to the index in the string.
            // If the color is null, reset to the default color.
            colors = ImmutableDictionary.CreateBuilder<int, Color?>();

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                result.Append(rawText.Slice(lastIndex, match.Index - lastIndex));

                Color colorForText = Color.FromName(match.Groups[1].Value);
                string currentText = match.Groups[2].Value;

                // Map the start of this current text as the color switch.
                colors[result.Length] = colorForText;

                result.Append(currentText);

                colors[result.Length] = default;

                lastIndex = match.Index + match.Length;
            }

            // Look, I also don't think this is correct. But I am still procrastinating doing the right solution for this.
            // So I'll just make sure existing \n do not mess up the color calculation since we are skipping \n
            // when calculating the right color index.
            nonSkippableLineEnding = ImmutableHashSet.CreateBuilder<int>();
            for (int i = 0; i < result.Length; ++i)
            {
                if (result[i] == '\n')
                {
                    nonSkippableLineEnding.Add(i);
                }
            }
        }

        if (lastIndex < rawText.Length)
        {
            result.Append(rawText.Slice(lastIndex));
        }

        string parsedText = result.ToString();
        if (settings.MaxWidth > 0)
        {
            string wrappedText = font.WrapString(parsedText, settings.MaxWidth, settings.Scale.X);
            parsedText = wrappedText.ToString();
        }

        int extraCharacters = Math.Max(0, parsedText.Length - text.Length);

        data = new RuntimeTextData(parsedText) with
        {
            Colors = colors?.ToImmutable(),
            NonSkippableLineEnding = nonSkippableLineEnding?.ToImmutable(),
            ExtraCharacters = extraCharacters,
            Font = font.Index,
            HiRes = settings.HiRes
        };

        _cache[key] = data;
        return data;
    }

    [GeneratedRegex("(?<!\n)\n(?!\n)")]
    private static partial Regex ReplaceSingleNewLine();

    [GeneratedRegex("\n\n")]
    private static partial Regex ReplaceTwoNewLines();

    [GeneratedRegex(" {2,}")]
    private static partial Regex TrimSpaces();

    [GeneratedRegex("<c=([^>]+)>([^<]+)</c>")]
    private static partial Regex ColorTags();

    [GeneratedRegex("<c=([^>]+)>|</c>")]
    public static partial Regex EscapeRegex();
}
