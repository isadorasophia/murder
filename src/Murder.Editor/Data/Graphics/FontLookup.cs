﻿using Murder.Core.Geometry;
using Murder.Diagnostics;
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
            public readonly Point Offset = Point.Zero;
            public readonly Point Padding = Point.Zero;
            public readonly int Size = 10;
            public readonly int Index = 0;
            public readonly ImmutableArray<char> Chars = ImmutableArray< char >.Empty;

            public FontInfo(int index, string fontName, int size, Point offset, Point padding)
            {
                Index = index;
                FontName = fontName;
                Size = size;
                Offset = offset;
                Padding = padding;
            }
            
            public FontInfo(int index, string fontName, int size, Point offset, Point padding, ImmutableArray<char> chars)
            {
                Index = index;
                FontName = fontName;
                Size = size;
                Offset = offset;
                Chars = chars;
                Padding = padding;
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

                if (parsed.Length != 2)
                {
                    GameLogger.Error($"Invalid fonts.murder line format, did you include only one ':'? Skipping line: {line}.");
                    continue;
                } 
                
                string[] numbers = parsed[1].Trim(' ').Split(' ');

                if (numbers.Length<1 || !int.TryParse(numbers[0], out int size))
                {
                    GameLogger.Error($"Invalid fonts.murder font size! Skipping line: {line}.");
                    continue;
                }

                int offsetX = 0;
                int offsetY = 0;
                int paddingX = 0;
                int paddingY = 0;

                if (numbers.Length>=2 && int.TryParse(numbers[1], out int parsedX))
                {
                    offsetX = parsedX;
                }

                if (numbers.Length >= 3 && int.TryParse(numbers[2], out int parsedY))
                {
                    offsetY = parsedY;
                }

                // For now we skip the paddingX since it's unnused

                if (numbers.Length >= 4 && int.TryParse(numbers[3], out int parsedPaddingY))
                {
                    paddingY = parsedPaddingY;
                }

                if (numbers.Length >= 5)
                {
                    string charsDefineFile = numbers[4];
                    string? charsDefineFilePath = Path.Combine(Path.GetDirectoryName(file) ?? string.Empty, charsDefineFile);
                    if (File.Exists(charsDefineFilePath))
                    {
                        var charsText = File.ReadAllText(charsDefineFilePath);
                        builder.Add(new FontInfo(index, name, size, new Point(offsetX, offsetY), new Point(paddingX, paddingY), charsText.ToCharArray().Distinct().ToImmutableArray()));
                    }
                    else
                    {
                        builder.Add(new FontInfo(index, name, size, new Point(offsetX, offsetY), new Point(paddingX, paddingY), ImmutableArray<char>.Empty));
                    }
                }
                else
                {
                    builder.Add(new FontInfo(index, name, size, new Point(offsetX, offsetY), new Point(paddingX, paddingY)));
                }
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