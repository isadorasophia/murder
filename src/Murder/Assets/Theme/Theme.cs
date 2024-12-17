using Murder.Utilities;
using System.Numerics;

namespace Murder.Assets
{
    public class Theme
    {
        public Vector4 Bg = "#282a36".ToVector4Color();
        public Vector4 BgFaded = "#44475a".ToVector4Color();
        public Vector4 Foreground = "#9260ab".ToVector4Color();
        public Vector4 HighAccent = "#ff79c6".ToVector4Color();
        public Vector4 Accent = "#bd93f9".ToVector4Color();
        public Vector4 RedFaded = "#5A444BFF".ToVector4Color();
        public Vector4 Faded = "#6272a4".ToVector4Color();
        public Vector4 Red = "#ff5545".ToVector4Color();
        public Vector4 Green = "#42ff22".ToVector4Color();
        public Vector4 Warning = "#eb8e42".ToVector4Color();
        public Vector4 White = "#f8f8f2".ToVector4Color();
        public Vector4 GenericAsset = new Vector4(1f, 0.4f, 0.6f, 1);
        public Vector4 Yellow = "#f1fa8c".ToVector4Color();
        public Vector4 YellowFaded = "#615F3F".ToVector4Color();
    }
}