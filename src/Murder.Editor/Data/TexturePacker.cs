using InstallWizard.Core;
using InstallWizard.DebugUtilities;
using InstallWizard.Graphics;
using InstallWizard.Util;
using Editor;
using Microsoft.Xna.Framework.Graphics;
using Aseprite = Editor.Data.Aseprite.Aseprite;

namespace TexturePacker
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

            Atlasses = new ();
            AsepriteFiles = new();
        }
        public void Process(string _SourceDir, int atlasSize, int padding, bool debugMode)
        {
            _padding = padding;
            _atlasSize = atlasSize;
            _debugMode = debugMode;

            //1: scan for all the textures we need to pack
            ScanForTextures(_SourceDir);

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

        public (int count, int maxWidth, int maxHeight) SaveAtlasses(string _Destination)
        {
            int atlasCount = 0;
            string prefix = _Destination.Replace(Path.GetExtension(_Destination), "");
            string foldername = Path.GetDirectoryName(_Destination)!;
                
            int width = 0;
            int height = 0;
            foreach (Atlas atlas in Atlasses)
            {
                _ = FileHelper.GetOrCreateDirectory(foldername);

                string atlasName = String.Format(prefix + "{0:000}" + ".png", atlasCount);

                //1: Save images
                Texture2D img = CreateAtlasImage(atlas);
                var stream = File.OpenWrite(atlasName);
                img.SaveAsPng(stream, img.Width, img.Height);

                width = Math.Max(width, img.Width);
                height = Math.Max(height, img.Height);

                ++atlasCount;
                stream.Close();
            }

            var tw = new StreamWriter(prefix + ".log");
            tw.WriteLine("--- LOG -------------------------------------------");
            tw.WriteLine(Log.ToString());
            tw.WriteLine("--- ERROR -----------------------------------------");
            tw.WriteLine(Error.ToString());
            tw.Close();

            return (atlasCount, width, height);
        }

        private void ScanForTextures(string _Path)
        {
            DirectoryInfo di = new DirectoryInfo(_Path);
            FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo fi in files)
            {
                if (fi.Name.StartsWith("_"))
                    continue;

                switch (fi.Extension.ToLower())
                {
                    case ".ase":
                    case ".aseprite":
                        ScanAsepriteFile(fi);
                        break;

                    case ".png":
                        ScanPngFile(fi);
                        break;

                    case ".clip":
                    case ".psd":
                        // We ignore PSD and CLIP files
                        continue;

                    default:
                        GameLogger.Log($"Unknown extension {fi.Extension}");
                        continue;
                }
            }
        }
        private void ScanAsepriteFile(FileInfo fi)
        {
            // GameDebugger.Log($"Loading the file {fi.FullName}.");
            var ase = new Aseprite(fi.FullName);

            AsepriteFiles.Add(ase);

            if(ase.Width <= _atlasSize && ase.Height <= _atlasSize)
            {
                if (ase.SplitLayers)
                {
                    for (int layer = 0; layer < ase.Layers.Count; layer++)
                        for (int i = 0; i < ase.FrameCount; i++)
                        {
                            if (!ase.Layers[layer].Name.Equals("REF", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ScanAsepriteFrame(fi, ase, i, layer);
                            }
                        }
                }
                else
                { 
                    for (int i = 0; i < ase.FrameCount; i++)
                    {
                        ScanAsepriteFrame(fi, ase, i);
                    }
                }
            }
            else
            {
                var error = fi.FullName + " is too large to fix in the atlas. Skipping!";
                GameLogger.Error(error);
                Error.WriteLine(error);
            }
            
        }

        private void ScanAsepriteFrame(FileInfo fi, Aseprite ase, int frame, int layer = -1)
        {
            TextureInfo ti = new TextureInfo();
            ti.Source = fi.FullName;

            ti.OriginalSize = new(ase.Width, ase.Height);
            if (ti.HasLayers)
            {
                ti.CroppedBounds = CalculateCrop(GetPixelsFromLayer(ase, frame, layer), ti.OriginalSize);
            }
            else
            {
                
                ti.CroppedBounds = CalculateCrop(ase.Frames[frame].Pixels, ti.OriginalSize);
            }
            if (ti.CroppedBounds.Width <= 0 && ti.CroppedBounds.Height <= 0)
            //Image '{fi.Name}' is completelly transparent! Let's ignore it?
            {
                ti.CroppedBounds = IntRectangle.Empty;
            }
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

            Log.WriteLine("Added " + ti.Source);
        }

        private void ScanPngFile(FileInfo fi)
        {
            Texture2D img = Texture2D.FromFile(Architect.GraphicsDevice, fi.FullName);
            if (img != null)
            {
                if (img.Width <= _atlasSize && img.Height <= _atlasSize)
                {
                    TextureInfo ti = new TextureInfo();

                    ti.Source = fi.FullName;
                    ti.OriginalSize = new(img.Width, img.Height);
                    var pixels = new Microsoft.Xna.Framework.Color[img.Width * img.Height];
                    img.GetData(pixels);
                    ti.CroppedBounds = CalculateCrop(pixels, ti.OriginalSize);
                    SourceTextures.Add(ti);

                    Log.WriteLine("Added " + fi.FullName);
                }
                else
                {
                    var error = fi.FullName + " is too large to fix in the atlas. Skipping!";
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
            var image = new RenderTarget2D(graphicsDevice, _Atlas.Width, _Atlas.Height, false, SurfaceFormat.Rgba64, DepthFormat.None);
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
                            var ase = AsepriteFiles[n.Texture.AsepriteFile];
                            sourceImg = new Texture2D(Architect.GraphicsDevice, n.Texture.OriginalSize.X, n.Texture.OriginalSize.Y);
                            Microsoft.Xna.Framework.Color[]? data;
                            if (n.Texture.Layer == -1)
                                data = ase.Frames[n.Texture.Frame].Pixels;
                            else
                                data = GetPixelsFromLayer(ase, n.Texture.Frame, n.Texture.Layer);

                            if (data != null)
                            {
                                sourceImg.SetData(data);

                                if (_debugMode)
                                    RenderServices.DrawQuadOutline(new Rectangle(n.Bounds.X, n.Bounds.Y, n.Texture.CroppedBounds.Width, n.Texture.CroppedBounds.Height), Color.Magenta);

                                RenderServices.DrawTextureQuad(sourceImg,
                                source: n.Texture.CroppedBounds,
                                destination: new Rectangle(n.Bounds.X, n.Bounds.Y, n.Texture.CroppedBounds.Width, n.Texture.CroppedBounds.Height),
                                Microsoft.Xna.Framework.Matrix.Identity, Color.White, BlendState.AlphaBlend);
                            }
                            break;

                        case ".png":
                            sourceImg = Texture2D.FromFile(graphicsDevice, n.Texture.Source);

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


                    if (sourceImg != null)
                    {
                        // RenderServices.DrawTextureQuad(sourceImg, new Rectangle(n.Bounds.X, n.Bounds.Y, sourceImg.Bounds.Width, sourceImg.Bounds.Height), RenderServices.BlendNormal, Color.White);
                    }
                    else
                    {
                        Error.WriteLine($"Image '{n.Texture.Source}' couldn't be drawn.");
                        DrawMissingImage(n.Bounds);
                    }
                    sourceImg?.Dispose();
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

        private static Microsoft.Xna.Framework.Color[] GetPixelsFromLayer(Aseprite ase, int frame, int layer)
        {
            foreach (var cell in ase.Frames[frame].Cels)
            {
                if (cell.Layer.Index == layer)
                {
                    return cell.Pixels;
                }
            }

            return new Microsoft.Xna.Framework.Color[ase.Width*ase.Height];
        }

        private IntRectangle CalculateCrop(Microsoft.Xna.Framework.Color[] pixels, Point originalSize)
        {
            if (!CropAlpha)
            {
                return new IntRectangle(0,0, originalSize.X, originalSize.Y);
            }

            IntRectangle cropArea = new(-1, -1, -1, -1);
            int xHeadstart1 = 0;
            int xHeadstart2 = 0;
            // Find top
            for (int y = 0; y < originalSize.Y; y++)
            {
                for (int x = 0; x < originalSize.X; x++)
                {
                    if (pixels[Calculator.OneD(x,y, originalSize.X)].A != 0)
                    {
                        cropArea.Y = y;
                        // Get a headstart on the left
                        xHeadstart1 = x-1;
                        break;
                    }
                }
                if (cropArea.Y >= 0)
                    break;
            }

                // Find bottom
                for (int y = originalSize.Y - 1; y > 0; y--)
                {
                    for (int x = originalSize.X - 1; x > 0; x--)
                    {
                        if (pixels[Calculator.OneD(x, y, originalSize.X)].A != 0)
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
            for (int x = 0; x < Math.Max(xHeadstart1, xHeadstart2); x++)
            {
                for (int y = cropArea.Top; y < cropArea.Bottom - 1; y++)
                { 
                    if (pixels[Calculator.OneD(x, y, originalSize.X)].A != 0)
                    {
                        cropArea.X = x;
                        break;
                    }
                }
                if (cropArea.X >= 0)
                    break;
            }

            // Find right
            for (int x = originalSize.X - 1; x > Math.Min(xHeadstart1, xHeadstart2); x--)
            {
                for (int y = originalSize.Y - 1; y > 0; y--)
                {
                    if (pixels[Calculator.OneD(x, y, originalSize.X)].A != 0)
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