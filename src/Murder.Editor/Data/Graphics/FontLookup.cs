using Murder.Diagnostics;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.Data.Graphics
{
    internal readonly struct FontLookup
    {
        public readonly struct FontInfo
        {
            public readonly string FontName = string.Empty;
            public readonly int Size = 10;
            public readonly int Index = 0;

            public FontInfo(int index, string fontName, int size)
            {
                Index = index;
                FontName = fontName;
                Size = size;
            }
        }
        public readonly ImmutableArray<FontInfo> Fonts;

        public FontLookup(string file)
        {
            var lines = File.ReadAllLines(file);
            var builder = ImmutableArray.CreateBuilder<FontInfo>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                int firstSpaceIndex = line.IndexOf(' ');
                if (firstSpaceIndex == -1)
                {
                    GameLogger.Error($"Invalid fonts.murder format! Skipping line: {line}.");
                    continue;
                }

                ReadOnlySpan<char> readOnlyLine = line.AsSpan();

                if (!int.TryParse(readOnlyLine.Slice(0, firstSpaceIndex), out int index))
                {
                    GameLogger.Error($"Invalid fonts.murder font index! Skipping line: {line}.");
                    continue;
                }

                string[]? parsed = readOnlyLine.Slice(firstSpaceIndex).Trim().ToString().Split(':');
                string name = parsed[0];

                if (parsed.Length != 2 || !int.TryParse(parsed[1], out int size))
                {
                    GameLogger.Error($"Invalid fonts.murder font size! Skipping line: {line}.");
                    continue;
                }

                builder.Add(new FontInfo(index, name, size));
            }

            Fonts = builder.ToImmutable();
        }

        internal FontInfo? GetInfo(string fontName)
        {
            foreach (var info in Fonts)
            {
                if (info.FontName.Equals(fontName, StringComparison.InvariantCultureIgnoreCase))
                    return info;
            }

            return null;
        }
    }
}