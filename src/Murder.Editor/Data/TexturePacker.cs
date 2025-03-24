using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Data.Graphics;
using Murder.Editor.Services;
using Murder.Serialization;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using static Murder.Editor.Data.Graphics.Aseprite;

namespace Murder.Editor.Data
{
    /// <summary>
    /// Indicates in which direction to split an unused area when it gets used
    /// </summary>
    public enum SplitType
    {
        /// <summary>
        /// Split Horizontally (textures are stacked up)
        /// </summary>
        Horizontal,

        /// <summary>
        /// Split verticaly (textures are side by side)
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// Different types of heuristics in how to use the available space
    /// </summary>
    public enum BestFitHeuristic
    {
        /// <summary>
        ///
        /// </summary>
        Area,

        /// <summary>
        ///
        /// </summary>
        MaxOneAxis,
    }

    /// <summary>
    /// A node in the Atlas structure
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Bounds of this node in the atlas
        /// </summary>
        public Rectangle Bounds;

        /// <summary>
        /// Texture this node represents
        /// </summary>
        public TextureInfo? Texture;

        /// <summary>
        /// If this is an empty node, indicates how to split it when it will  be used
        /// </summary>
        public SplitType SplitType;
    }

    /// <summary>
    /// The texture atlas
    /// </summary>
    public class Atlas
    {
        /// <summary>
        /// Width in pixels
        /// </summary>
        public int Width;

        /// <summary>
        /// Height in Pixel
        /// </summary>
        public int Height;

        /// <summary>
        /// List of the nodes in the Atlas. This will represent all the textures that are packed into it and all the remaining free space
        /// </summary>
        public List<Node> Nodes = new();
    }

    /// <summary>
    /// Objects that performs the packing task. Takes a list of textures as input and generates a set of atlas textures/definition pairs
    /// </summary>
    public class Packer
    {
        /// <summary>
        /// List of all the textures that need to be packed
        /// </summary>
        public List<TextureInfo> SourceTextures;

        /// <summary>
        /// Stream that recieves all the info logged
        /// </summary>
        public StringWriter Log;

        /// <summary>
        /// Stream that recieves all the error info
        /// </summary>
        public StringWriter Error;

        /// <summary>
        /// Number of pixels that separate textures in the atlas
        /// </summary>
        private int _padding;

        /// <summary>
        /// Size of the atlas in pixels. Represents one axis, as atlases are square
        /// </summary>
        private int _atlasSize;

        /// <summary>
        /// Toggle for debug mode, resulting in debug atlasses to check the packing algorithm
        /// </summary>
        private bool _debugMode;

        /// <summary>
        /// Which heuristic to use when doing the fit
        /// </summary>
        public BestFitHeuristic FitHeuristic;

        /// <summary>
        /// List of all the output atlases
        /// </summary>
        public List<Atlas> Atlasses;

        public List<Aseprite> AsepriteFiles;

        public bool CropAlpha = true;

        public Packer()
        {
            SourceTextures = new List<TextureInfo>();
            Log = new StringWriter();
            Error = new StringWriter();

            Atlasses = new();
            AsepriteFiles = new();
        }

        /// Temp method, will remove this when all the image loading is done by the ResourceImporters
        public void Process(string sourcePath, int atlasSize, int padding, bool debugMode)
        {
            _padding = padding;
            _atlasSize = atlasSize;
            _debugMode = debugMode;

            //1: scan for all the textures we need to pack
            if (!Directory.Exists(sourcePath))
            {
                GameLogger.Error("TexturePacker couldn't find a source directory");
                return;
            }

            DirectoryInfo di = new DirectoryInfo(sourcePath);

            List<string> files = di.GetFiles("*.*", SearchOption.AllDirectories).Where(fi => !fi.Name.StartsWith('_')).Select(fi => fi.FullName).ToList();
            ScanForTextures(files);

            // textures = SourceTextures.ToList();
            List<TextureInfo> textures = SourceTextures.Where(t => (t.CroppedBounds.Width > 0 && t.CroppedBounds.Height > 0)).ToList();

            //2: generate as many atlasses as needed (with the latest one as small as possible)
            Atlasses = new List<Atlas>();
            while (textures.Count > 0)
            {
                Atlas atlas = new Atlas();
                atlas.Width = atlasSize;
                atlas.Height = atlasSize;

                List<TextureInfo> leftovers = LayoutAtlas(textures, atlas);

                if (leftovers.Count == 0)
                {
                    // we reached the last atlas. Check if this last atlas could have been twice smaller
                    while (leftovers.Count == 0)
                    {
                        atlas.Width /= 2;
                        atlas.Height /= 2;
                        leftovers = LayoutAtlas(textures, atlas);
                    }
                    // we need to go 1 step larger as we found the first size that is to small
                    atlas.Width *= 2;
                    atlas.Height *= 2;
                    leftovers = LayoutAtlas(textures, atlas);

                    foreach (var t in SourceTextures)
                    {
                        if (t.CroppedBounds.Width <= 0 || t.CroppedBounds.Height <= 0)
                        {
                            atlas.Nodes.Add(new Node()
                            {
                                Texture = t,
                                Bounds = Rectangle.Empty
                            });
                        }
                    }
                }

                Atlasses.Add(atlas);

                textures = leftovers;
            }
        }

        // TODO: Dynamically calculate th size of the atlas
        public void Process(List<string> files, int atlasSize, int padding, bool debugMode)
        {
            _padding = padding;
            _atlasSize = atlasSize;
            _debugMode = debugMode;

            //1: scan for all the textures we need to pack
            ScanForTextures(files);

            // textures = SourceTextures.ToList();
            List<TextureInfo> textures = SourceTextures.Where(t => (t.CroppedBounds.Width > 0 && t.CroppedBounds.Height > 0)).ToList();

            //2: generate as many atlasses as needed (with the latest one as small as possible)
            Atlasses = new List<Atlas>();
            while (textures.Count > 0)
            {
                Atlas atlas = new Atlas();
                atlas.Width = atlasSize;
                atlas.Height = atlasSize;

                List<TextureInfo> leftovers = LayoutAtlas(textures, atlas);

                if (leftovers.Count == 0)
                {
                    // we reached the last atlas. Check if this last atlas could have been twice smaller
                    while (leftovers.Count == 0)
                    {
                        atlas.Width /= 2;
                        atlas.Height /= 2;
                        leftovers = LayoutAtlas(textures, atlas);
                    }
                    // we need to go 1 step larger as we found the first size that is to small
                    atlas.Width *= 2;
                    atlas.Height *= 2;
                    leftovers = LayoutAtlas(textures, atlas);

                    foreach (var t in SourceTextures)
                    {
                        if (t.CroppedBounds.Width <= 0 || t.CroppedBounds.Height <= 0)
                        {
                            atlas.Nodes.Add(new Node()
                            {
                                Texture = t,
                                Bounds = Rectangle.Empty
                            });
                        }
                    }
                }

                Atlasses.Add(atlas);

                textures = leftovers;
            }
        }

        /// <summary>
        /// Save the processed atlasses at <paramref name="targetFilePathWithoutExtension"/>.
        /// </summary>
        public (int count, int maxWidth, int maxHeight) SaveAtlasses(string targetFilePathWithoutExtension)
        {
            int atlasCount = 0;

            GameLogger.Verify(Path.IsPathRooted(targetFilePathWithoutExtension));

            if (Path.GetDirectoryName(targetFilePathWithoutExtension) is string directory)
            {
                FileManager.GetOrCreateDirectory(directory);
            }

            int width = 0;
            int height = 0;
            foreach (Atlas atlas in Atlasses)
            {
                string suffix = string.Format("{0:000}" + TextureServices.QOI_GZ_EXTENSION, atlasCount);
                string filePath = targetFilePathWithoutExtension + suffix;
                
                // 1: Save images
                using Texture2D img = CreateAtlasImage(atlas);
                EditorTextureServices.SaveAsQoiGz(img, filePath);

                width = Math.Max(width, img.Width);
                height = Math.Max(height, img.Height);

                ++atlasCount;
            }

            var tw = new StreamWriter(targetFilePathWithoutExtension + ".log");

            tw.WriteLine("--- LOG -------------------------------------------");
            tw.WriteLine(Log.ToString());
            tw.WriteLine("--- ERROR -----------------------------------------");
            tw.WriteLine(Error.ToString());
            tw.Close();

            return (atlasCount, width, height);
        }

        private void ScanForTextures(List<string> files)
        {
            foreach (string path in files)
            {
                if (path.StartsWith("_"))
                    continue;

                switch (Path.GetExtension(path).ToLower())
                {
                    case ".ase":
                    case ".aseprite":
                        ScanAsepriteFile(path);
                        break;

                    case ".png":
                        ScanPngFile(path);
                        break;

                    default:
                        GameLogger.Log($"Unknown extension {Path.GetExtension(path)} ({path}), consider adding this to \"Ignored Texture Packing Extensions\" in the Editor Settings.");
                        continue;
                }
            }
        }
        private void ScanAsepriteFile(string path)
        {
            // GameDebugger.Log($"Loading the file {fi.FullName}.");
            var ase = new Aseprite(path);

            // Skips files starting with underscore
            if (path.StartsWith('_'))
                return;

            AsepriteFiles.Add(ase);

            if (ase.Width <= _atlasSize && ase.Height <= _atlasSize)
            {
                for (int slice = 0; slice < Math.Max(1, ase.Slices.Count); slice++)
                {
                    if (ase.SplitLayers)
                    {
                        for (int layer = 0; layer < ase.Layers.Count; layer++)
                            for (int i = 0; i < ase.FrameCount; i++)
                            {
                                if (!ase.Layers[layer].Name.Equals("REF", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    ScanAsepriteFrame(path, ase, i, layer, slice);
                                }
                            }
                    }
                    else
                    {
                        for (int i = 0; i < ase.FrameCount; i++)
                        {
                            ScanAsepriteFrame(path, ase, i, -1, slice);
                        }
                    }
                }
            }
            else
            {
                var error = path + " is too large to fix in the atlas. Skipping!";
                GameLogger.Error(error);
                Error.WriteLine(error);
            }

        }

        private void ScanAsepriteFrame(string path, Aseprite ase, int frame, int layer, int sliceIndex)
        {
            TextureInfo ti = new TextureInfo();
            Slice slice = ase.Slices[sliceIndex];

            ti.Source = path;
            ti.SliceName = slice.Name;
            ti.HasSlices = ase.Slices.Count > 1;

            ti.SliceSize = new(slice.Width, slice.Height);

            var startingCrop = new IntRectangle(slice.OriginX, slice.OriginY, slice.Width, slice.Height);

            if (ti.HasLayers)
            {
                ti.CroppedBounds = CalculateCrop(GetPixelsFromLayer(ase, frame, layer), new(ase.Width, ase.Height), startingCrop);
            }
            else
            {
                ti.CroppedBounds = CalculateCrop(ase.Frames[frame].Pixels, new(ase.Width, ase.Height), startingCrop);
            }

            //Image '{fi.AtlasId}' is completelly transparent! Let's ignore it?
            if (ti.CroppedBounds.Width <= 0 || ti.CroppedBounds.Height <= 0)
            {
                ti.CroppedBounds = IntRectangle.Empty;
            }
            ti.TrimArea = new IntRectangle(ti.CroppedBounds.X - slice.OriginX, ti.CroppedBounds.Y - slice.OriginY, ti.CroppedBounds.Width, ti.CroppedBounds.Height);

            if (layer >= 0)
            {
                ti.HasLayers = true;
                ti.Layer = layer;
                ti.LayerName = ase.Layers[layer].Name;
            }
            ti.Frame = frame;
            if (ase.FrameCount > 1)
                ti.IsAnimation = true;

            ti.AsepriteFile = AsepriteFiles.Count - 1;

            SourceTextures.Add(ti);

            // Log.WriteLine("Added " + ti.Source);
        }

        private void ScanPngFile(string path)
        {
            using Texture2D img = TextureServices.FromFile(Architect.GraphicsDevice, path);

            if (img != null)
            {
                if (img.Width <= _atlasSize && img.Height <= _atlasSize)
                {
                    TextureInfo ti = new TextureInfo();

                    ti.Source = path;
                    ti.SliceSize = new(img.Width, img.Height);
                    var pixels = new Microsoft.Xna.Framework.Color[img.Width * img.Height];
                    img.GetData(pixels);
                    ti.TrimArea = ti.CroppedBounds = CalculateCrop(pixels, ti.SliceSize, new(0, 0, ti.SliceSize.X, ti.SliceSize.Y));
                    SourceTextures.Add(ti);

                    Log.WriteLine("Added " + path);
                }
                else
                {
                    var error = path + " is too large to fix in the atlas. Skipping!";
                    GameLogger.Error(error);
                    Error.WriteLine(error);
                }
            }
        }

        private void HorizontalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new Node();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + _padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - _padding;
            n1.Bounds.Height = _Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new Node();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + _padding;
            n2.Bounds.Width = _ToSplit.Bounds.Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - _padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }

        private void VerticalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new Node();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + _padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - _padding;
            n1.Bounds.Height = _ToSplit.Bounds.Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new Node();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + _padding;
            n2.Bounds.Width = _Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - _padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }

        private TextureInfo? FindBestFitForNode(Node _Node, List<TextureInfo> textures)
        {
            TextureInfo? bestFit = null;

            float nodeArea = _Node.Bounds.Width * _Node.Bounds.Height;
            float maxCriteria = 0.0f;

            foreach (TextureInfo ti in textures)
            {
                switch (FitHeuristic)
                {
                    // Max of Width and Height ratios
                    case BestFitHeuristic.MaxOneAxis:
                        if (ti.CroppedBounds.Width <= _Node.Bounds.Width && ti.CroppedBounds.Height <= _Node.Bounds.Height)
                        {
                            float wRatio = (float)ti.CroppedBounds.Width / (float)_Node.Bounds.Width;
                            float hRatio = (float)ti.CroppedBounds.Height / (float)_Node.Bounds.Height;
                            float ratio = wRatio > hRatio ? wRatio : hRatio;
                            if (ratio > maxCriteria)
                            {
                                maxCriteria = ratio;
                                bestFit = ti;
                            }
                        }
                        break;

                    // Maximize Area coverage
                    case BestFitHeuristic.Area:

                        if (ti.CroppedBounds.Width <= _Node.Bounds.Width && ti.CroppedBounds.Height <= _Node.Bounds.Height)
                        {
                            float textureArea = ti.CroppedBounds.Width * ti.CroppedBounds.Height;
                            float coverage = textureArea / nodeArea;
                            if (coverage > maxCriteria)
                            {
                                maxCriteria = coverage;
                                bestFit = ti;
                            }
                        }
                        break;
                }
            }

            return bestFit;
        }

        private List<TextureInfo> LayoutAtlas(List<TextureInfo> _Textures, Atlas _Atlas)
        {
            List<Node> freeList = new List<Node>();

            _Atlas.Nodes = new List<Node>();

            List<TextureInfo> textures = _Textures.ToList();

            Node root = new Node();
            root.Bounds.Size = new Point(_Atlas.Width, _Atlas.Height);
            root.SplitType = SplitType.Horizontal;

            freeList.Add(root);

            while (freeList.Count > 0 && textures.Count > 0)
            {
                Node node = freeList[0];
                freeList.RemoveAt(0);

                TextureInfo? bestFit = FindBestFitForNode(node, textures);
                if (bestFit != null)
                {
                    if (node.SplitType == SplitType.Horizontal)
                    {
                        HorizontalSplit(node, bestFit.CroppedBounds.Width, bestFit.CroppedBounds.Height, freeList);
                    }
                    else
                    {
                        VerticalSplit(node, bestFit.CroppedBounds.Width, bestFit.CroppedBounds.Height, freeList);
                    }

                    node.Texture = bestFit;
                    node.Bounds.Width = bestFit.CroppedBounds.Width;
                    node.Bounds.Height = bestFit.CroppedBounds.Height;

                    textures.Remove(bestFit);
                }

                _Atlas.Nodes.Add(node);
            }

            return textures;
        }

        private Texture2D CreateAtlasImage(Atlas _Atlas)
        {
            var graphicsDevice = Architect.GraphicsDevice;
            var image = new RenderTarget2D(graphicsDevice, _Atlas.Width, _Atlas.Height, false, SurfaceFormat.Color, DepthFormat.None);
            graphicsDevice.SetRenderTarget(image);
            graphicsDevice.Clear(Color.Transparent);

            if (_debugMode)
            {
                RenderServices.DrawQuad(new Rectangle(0, 0, _Atlas.Width, _Atlas.Height), Color.Gray);
            }

            foreach (Node n in _Atlas.Nodes)
            {
                if (n.Texture != null)
                {
                    Texture2D? sourceImg = null;

                    var extension = Path.GetExtension(n.Texture.Source);
                    switch (extension)
                    {
                        case ".ase":
                        case ".aseprite":
                            sourceImg = CreateAsepriteImageFromNode(n);
                            break;

                        case TextureServices.QOI_GZ_EXTENSION:
                        case TextureServices.PNG_EXTENSION:
                            sourceImg = TextureServices.FromFile(graphicsDevice, n.Texture.Source);

                            RenderServices.DrawTextureQuad(sourceImg,
                                source: n.Texture.CroppedBounds,
                                destination: new Rectangle(n.Bounds.X, n.Bounds.Y, n.Texture.CroppedBounds.Width, n.Texture.CroppedBounds.Height),
                                Microsoft.Xna.Framework.Matrix.Identity, Color.White, BlendState.AlphaBlend);
                            break;

                        case ".clip":
                        case ".psd":
                            // We ignore PSD and CLIP files
                            break;

                        default:
                            Error.WriteLine($"Image '{n.Texture.Source}' has an unknown extension '{extension}'.");
                            break;
                    }

                    if (sourceImg == null)
                    {
                        Error.WriteLine($"Image '{n.Texture.Source}' couldn't be drawn.");
                        DrawMissingImage(n.Bounds);
                    }
                    else
                    {
                        sourceImg.Dispose();
                    }
                }
                else
                {
                    // Node is empty, which is all right, just means we didn't manage to fill it completelly.
                    // DrawMissingImage(g, n.Bounds);
                }
            }

            graphicsDevice.SetRenderTarget(null);
            return image;
        }

        private Texture2D CreateAsepriteImageFromNode(Node n)
        {
            Debug.Assert(n.Texture is not null);

            Texture2D? sourceImg;
            var ase = AsepriteFiles[n.Texture!.AsepriteFile];

            sourceImg = new Texture2D(Architect.GraphicsDevice, ase.Width, ase.Height);
            sourceImg.Name = $"Source:{n.Texture.Source}";
            Microsoft.Xna.Framework.Color[]? data;
            if (n.Texture.Layer == -1)
            {
                data = ase.Frames[n.Texture.Frame].Pixels; // TODO: Crop pixels to the size of the slice
            }
            else
                data = GetPixelsFromLayer(ase, n.Texture.Frame, n.Texture.Layer);

            if (data != null)
            {
                Debug.Assert(data.Length == ase.Width * ase.Height, $"Data length of {n.Texture.Source} is not equal to the size of the texture.");
                sourceImg.SetData(data);

                if (_debugMode)
                    RenderServices.DrawQuadOutline(new Rectangle(n.Bounds.X, n.Bounds.Y, n.Texture.CroppedBounds.Width, n.Texture.CroppedBounds.Height), Color.Magenta);

                RenderServices.DrawTextureQuad(sourceImg,
                source: n.Texture.CroppedBounds,
                destination: new Rectangle(n.Bounds.X, n.Bounds.Y, n.Texture.CroppedBounds.Width, n.Texture.CroppedBounds.Height),
                Microsoft.Xna.Framework.Matrix.Identity, Color.White, BlendState.AlphaBlend);
            }

            return sourceImg;
        }

        private static Microsoft.Xna.Framework.Color[] GetPixelsFromLayer(Aseprite ase, int frame, int layer)
        {
            if (ase.Frames[frame].Cels.TryGetValue(layer, out var cel))
            {
                return cel.Pixels;
            }

            return new Microsoft.Xna.Framework.Color[ase.Width * ase.Height];
        }

        private IntRectangle CalculateCrop(Microsoft.Xna.Framework.Color[] pixels, Point totalSize, IntRectangle startingCrop)
        {
            if (!CropAlpha)
            {
                return startingCrop;
            }

            IntRectangle cropArea = new(-1, -1, -1, -1);
            int xHeadstart1 = 0;
            int xHeadstart2 = 0;
            // Find top
            for (int y = startingCrop.Top; y < startingCrop.Bottom; y++)
            {
                for (int x = startingCrop.Left; x < startingCrop.Right; x++)
                {
                    int pixelCoord = Calculator.OneD(x, y, totalSize.X);
                    if (pixelCoord < 0 || pixelCoord >= pixels.Length)
                    {
                        continue;
                    }
                    if (pixels[pixelCoord].A != 0)
                    {
                        cropArea.Y = y;
                        // Get a headstart on the left
                        xHeadstart1 = x - 1;
                        break;
                    }
                }
                if (cropArea.Y >= 0)
                    break;
            }

            // Find bottom
            for (int y = startingCrop.Bottom - 1; y > startingCrop.Top; y--)
            {
                for (int x = startingCrop.Right - 1; x > startingCrop.Left; x--)
                {
                    int pixelCoord = Calculator.OneD(x, y, totalSize.X);
                    if (pixelCoord < 0 || pixelCoord >= pixels.Length)
                    {
                        continue;
                    }
                    if (pixels[pixelCoord].A != 0)
                    {
                        cropArea.Height = (y - cropArea.Y) + 1;
                        // Get a headstart on the right
                        xHeadstart2 = x + 1;
                        break;
                    }
                }
                if (cropArea.Height >= 0)
                    break;
            }


            // Find left
            for (int x = startingCrop.Left; x < Math.Max(xHeadstart1, xHeadstart2); x++)
            {
                for (int y = cropArea.Top; y < cropArea.Bottom - 1; y++)
                {
                    int pixelCoord = Calculator.OneD(x, y, totalSize.X);
                    if (pixelCoord < 0 || pixelCoord >= pixels.Length)
                    {
                        continue;
                    }
                    if (pixels[pixelCoord].A != 0)
                    {
                        cropArea.X = x;
                        break;
                    }
                }
                if (cropArea.X >= 0)
                    break;
            }

            // Find right
            for (int x = startingCrop.Right - 1; x > Math.Min(xHeadstart1, xHeadstart2); x--)
            {
                
                for (int y = startingCrop.Bottom - 1; y > startingCrop.Top; y--)
                {
                    int pixelCoord = Calculator.OneD(x, y, totalSize.X);
                    if (pixelCoord < 0 || pixelCoord >= pixels.Length)
                    {
                        continue;
                    }
                    if (pixels[pixelCoord].A != 0)
                    {
                        cropArea.Width = x - cropArea.X + 1;
                        break;
                    }
                }
                if (cropArea.Width >= 0)
                    break;
            }

            return cropArea;
        }

        private static void DrawMissingImage(Rectangle rectangle)
        {
            RenderServices.DrawQuad(rectangle, Color.Magenta);
        }
    }
}