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
            var lines = System.IO.File.ReadAllLines(file);
            var builder = ImmutableArray.CreateBuilder<FontInfo>();

            int index = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;
                var parsed = line.Trim().Split(':');
                string name = parsed[0];
                int size;
                int.TryParse(parsed[1], out size);
                builder.Add(new FontInfo(index++, name, size));    
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
