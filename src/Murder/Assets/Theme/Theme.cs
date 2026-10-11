using Murder.Utilities;
using System.Numerics;

namespace Murder.Assets
{
    public class Theme
    {
        public readonly Vector4 Bg = "#1e2030".ToVector4Color();
        public readonly Vector4 BgHeader = "#252840".ToVector4Color();
        public readonly Vector4 BgFaded = "#363a55".ToVector4Color();
        public readonly Vector4 Selected = "#3d4166".ToVector4Color();
        public readonly Vector4 Faded = "#5c6494".ToVector4Color();
        public readonly Vector4 Foreground = "#9062b0".ToVector4Color();
        public readonly Vector4 Accent = "#bd93f9".ToVector4Color();
        public readonly Vector4 HighAccent = "#ff79c6".ToVector4Color();
        public readonly Vector4 White = "#f8f8f2".ToVector4Color();
        public readonly Vector4 Yellow = "#e0ddb0".ToVector4Color();
        public readonly Vector4 YellowFaded = "#4e4c32".ToVector4Color();
        public readonly Vector4 Warning = "#eb8e42".ToVector4Color();
        public readonly Vector4 Red = "#e05550".ToVector4Color();
        public readonly Vector4 RedFaded = "#5a3d42".ToVector4Color();
        public readonly Vector4 Green = "#3fc87a".ToVector4Color();
        public readonly Vector4 GenericAsset = "#e07ca0".ToVector4Color();
    }
}