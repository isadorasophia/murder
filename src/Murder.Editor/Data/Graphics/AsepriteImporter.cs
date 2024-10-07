using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Color = Microsoft.Xna.Framework.Color;

// Gist from:
// https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

// File Format:
// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

// Note: I didn't test with with Indexed or Grayscale colors
// Only implemented the stuff I needed / wanted, other stuff is ignored

namespace Murder.Editor.Data.Graphics;

public partial class Aseprite
{

    public enum Modes
    {
        Indexed = 1,
        Grayscale = 2,
        RGBA = 4
    }

    private enum Chunks
    {
        OldPaletteA = 0x0004,
        OldPaletteB = 0x0011,
        Layer = 0x2004,
        Cel = 0x2005,
        CelExtra = 0x2006,
        ColorProfile = 0x2007,
        ExternalProfile = 0x2008,
        MaskDEPRECATED = 0x2016,
        Path = 0x2017,
        FrameTags = 0x2018,
        Palette = 0x2019,
        UserData = 0x2020,
        Slice = 0x2022,
        Tileset = 0x2023
    }

    public readonly string Name;
    public readonly string Source;
    public readonly string FullSource;
    public readonly Modes Mode;
    public readonly int Width;
    public readonly int Height;
    public readonly int FrameCount;
    public readonly UserData UserData;
    public readonly bool SplitLayers;

    public List<Tileset> Tilesets = new();
    public List<Layer> Layers = new();
    public List<Frame> Frames = new();
    public List<Tag> Tags = new();
    public List<Slice> Slices = new();

    #region .ase Parser

    public Aseprite(string file, bool loadImageData = true)
    {
        using (var stream = File.OpenRead(file))
        {
            Name = Path.GetFileNameWithoutExtension(file);
            Source = GetRelativeToContent(FileHelper.GetPathWithoutExtension(file));
            FullSource = file;

            var reader = new BinaryReader(stream);

            // wrote these to match the documentation names so it's easier (for me, anyway) to parse
            byte BYTE() { return reader.ReadByte(); }
            ushort WORD() { return reader.ReadUInt16(); }
            short SHORT() { return reader.ReadInt16(); }
            uint DWORD() { return reader.ReadUInt32(); }
            long LONG() { return reader.ReadInt32(); }
            string STRING() { return Encoding.UTF8.GetString(BYTES(WORD())); }
            byte[] BYTES(int number) { return reader.ReadBytes(number); }
            void SEEK(int number) { reader.BaseStream.Position += number; }


            // Header
            {
                // file size
                DWORD();

                // Magic number (0xA5E0)
                var magic = WORD();
                if (magic != 0xA5E0)
                    throw new Exception("File is not in .ase format");

                // Frames / Width / Height / Color Mode
                FrameCount = WORD();
                Width = WORD();
                Height = WORD();
                Mode = (Modes)(WORD() / 8);

                // Other Info, Ignored
                DWORD();       // Flags
                WORD();        // Speed (deprecated)
                DWORD();       // Set be 0
                DWORD();       // Set be 0
                BYTE();        // Palette entry 
                SEEK(3);       // Ignore these bytes
                WORD();        // Number of colors (0 means 256 for old sprites)
                BYTE();        // Pixel width
                BYTE();        // Pixel height
                SHORT();       // X position of the grid
                SHORT();       // y position of the grid
                WORD();        // Grid width
                WORD();        // Grid height
                SEEK(84);      // For Future
            }

            // temporary variables
            var temp = new byte[Width * Height * (int)Mode];
            var palette = new Color[256];
            IUserData? last = null;

            // Frames
            for (int i = 0; i < FrameCount; i++)
            {
                int nextTagUserData = 0;

                var frame = new Frame(this);
                if (loadImageData)
                    frame.Pixels = new Color[Width * Height];
                Frames.Add(frame);

                long frameStart, frameEnd;
                int chunkCount;

                // frame header
                {
                    frameStart = reader.BaseStream.Position;
                    frameEnd = frameStart + DWORD();
                    WORD();                  // Magic number (always 0xF1FA)
                    chunkCount = WORD();     // Number of "chunks" in this frame
                    frame.Duration = WORD(); // Frame duration (in milliseconds)
                    SEEK(6);                 // For future (set to zero)
                }

                // chunks
                for (int j = 0; j < chunkCount; j++)
                {
                    long chunkStart, chunkEnd;
                    Chunks chunkType;

                    // chunk header
                    {
                        chunkStart = reader.BaseStream.Position;
                        chunkEnd = chunkStart + DWORD();
                        chunkType = (Chunks)WORD();
                    }
                    void SKIP() { reader.BaseStream.Position = chunkEnd; }

                    // LAYER CHUNK
                    if (chunkType == Chunks.Layer)
                    {
                        // create layer
                        var layer = new Layer();

                        // get layer data
                        layer.Flag = (Layer.Flags)WORD();
                        layer.Type = (Layer.Types)WORD();
                        layer.ChildLevel = WORD();
                        WORD(); // width (unused)
                        WORD(); // height (unused)
                        layer.BlendMode = WORD();
                        layer.Alpha = BYTE() / 255f;
                        SEEK(3); // for future
                        layer.Name = STRING();
                        layer.Index = Layers.Count;

                        last = layer;
                        Layers.Add(layer);
                    }
                    // CEL CHUNK
                    else if (chunkType == Chunks.Cel)
                    {
                        // create cel
                        var cel = new Cel();

                        // get cel data
                        cel.Layer = Layers[WORD()];
                        cel.X = SHORT();
                        cel.Y = SHORT();
                        cel.Alpha = BYTE() / 255f;
                        var celType = WORD(); // type
                        SEEK(7);

                        // SPLIT sprites (that save individual layers) always save all layers
                        // Normal sprites that flatten everything ignore layers that are not toggled on in aseprite
                        if (loadImageData && (SplitLayers || cel.Layer.Flag.HasFlag(Layer.Flags.Visible)) && !cel.Layer.Flag.HasFlag(Layer.Flags.Reference) && cel.Layer.Name != "REF")
                        {
                            // RAW or DEFLATE
                            if (celType == 0 || celType == 2)
                            {
                                cel.Width = WORD();
                                cel.Height = WORD();

                                var count = cel.Width * cel.Height * (int)Mode;
                                var pixelBuffer = new byte[count];

                                // Raw
                                if (celType == 0)
                                {
                                    SEEK(2);

                                    reader.Read(pixelBuffer, 0, count);
                                }
                                // Compressed Image
                                else if (celType == 2)
                                {
                                    SEEK(2);

                                    const int HEADER_SIZE = 6;

                                    int chunkLength = (int)(chunkEnd - chunkStart);
                                    int compressedDataSize = (int)(chunkLength - 22 /* magic number? */) - HEADER_SIZE;

                                    DeflateStream gzip = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);

                                    int len = 0;
                                    MemoryStream uncompressed = new MemoryStream();

                                    byte[] buffer = new byte[1024];

                                    do
                                    {
                                        len = gzip.Read(buffer, 0, buffer.Length);

                                        if (len > 0)
                                        {
                                            uncompressed.Write(buffer, 0, len);
                                        }

                                    } while (len > 0);

                                    uncompressed.Position = 0;
                                    BinaryReader ureader = new BinaryReader(uncompressed);
                                    pixelBuffer = ureader.ReadBytes(count);
                                }

                                cel.Pixels = new Color[cel.Width * cel.Height];
                                BytesToPixels(pixelBuffer, cel.Pixels, Mode, palette);
                                CelToCel(cel, cel, Width, Height);
                            }
                            // Linked
                            else if (celType == 1)
                            {
                                cel.Link = WORD();
                            }
                            // Tilemap
                            if (celType == 3)
                            {
                                int tileW = WORD();
                                int tileH = WORD();
                                int bitsPerTile = WORD() / 8;
                                var tileBitmaskId = DWORD();
                                var xFlipMask = DWORD();
                                var yFlipMask = DWORD();
                                var rotationMask = DWORD();
                                _ = BYTES(10);

                                SEEK(2); // The first 2-bytes of the compressed stream are the zlib headers, which the DeflateStream used to decompress it doesn't need

                                var tilemap = Tilesets.Last(); // TODO: This doesn't look right

                                var deflate = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
                                var tilesBuffer = new byte[tilemap.dataLength];
                                deflate.Read(tilesBuffer, 0, tilemap.dataLength);

                                cel.Width = tilemap.TileWidth * tileW;
                                cel.Height = tilemap.TileHeight * tileH;
                                cel.Pixels = new Color[cel.Width * cel.Height];

                                var tiles = new Tile[tileW * tileH];
                                BytesToTiles(tilesBuffer, tiles, bitsPerTile, tileBitmaskId, xFlipMask, yFlipMask, rotationMask);

                                for (int y = 0; y < tileH; y++)
                                {
                                    for (int x = 0; x < tileW; x++)
                                    {
                                        int index = y * tileW + x;
                                        Tile tile = tiles[index];

                                        Blit(
                                            cel.Pixels, cel.Width,
                                            tilemap.Pixels, tilemap.TileWidth,
                                            x * tilemap.TileWidth, y * tilemap.TileHeight,
                                            0, tilemap.TileHeight * tile.Id,
                                            tilemap.TileWidth, tilemap.TileHeight);

                                    }
                                }

                                CelToCel(cel, cel, Width, Height);
                            }
                        }

                        last = cel;
                        frame.Cels[cel.Layer.Index] = cel;
                    }
                    // PALETTE CHUNK
                    else if (chunkType == Chunks.Palette)
                    {
                        var size = DWORD();
                        var start = DWORD();
                        var end = DWORD();
                        SEEK(8); // for future

                        for (int p = 0; p < end - start + 1; p++)
                        {
                            var hasName = WORD();
                            palette[start + p] = Color.FromNonPremultiplied(BYTE(), BYTE(), BYTE(), BYTE());
                            if (IsBitSet(hasName, 0))
                                STRING();
                        }
                    }
                    // TAG
                    else if (chunkType == Chunks.FrameTags)
                    {
                        var count = WORD();
                        SEEK(8);
                        for (int t = 0; t < count; t++)
                        {
                            var tag = new Tag();
                            tag.From = WORD();
                            tag.To = WORD();
                            tag.LoopDirection = (Tag.LoopDirections)BYTE();
                            tag.Repeat = WORD();
                            SEEK(10);
                            tag.Name = STRING().Trim();

                            Tags.Add(tag);
                        }

                    }
                    // USERDATA
                    else if (chunkType == Chunks.UserData)
                    {
                        var userData = new UserData();
                        var flags = DWORD();
                        if ((flags & 1) != 0)
                            userData.Text = STRING();
                        if ((flags & 2) != 0)
                            userData.Color = new Color(BYTE(), BYTE(), BYTE(), BYTE());
                        if ((flags & 4) != 0)
                            SKIP();

                        if (last is IUserData target)
                        {
                            target.UserData = userData;
                            last = null;
                        }
                        else if (Tags.Count > 0 && nextTagUserData < Tags.Count)
                        {
                            Tags[nextTagUserData++].UserData = userData;
                        }
                        else // This file's userdata
                        {
                            UserData = userData;
                            if (UserData.Text != null && UserData.Text.Equals("split", StringComparison.InvariantCultureIgnoreCase))
                                SplitLayers = true;
                        }
                    }
                    // SLICE
                    else if (chunkType == Chunks.Slice)
                    {
                        var count = DWORD();
                        var flags = (int)DWORD();
                        DWORD(); // reserved
                        var name = STRING();

                        for (int s = 0; s < count; s++)
                        {
                            var slice = new Slice();
                            slice.Name = name;
                            slice.Frame = (int)DWORD();
                            slice.OriginX = (int)LONG();
                            slice.OriginY = (int)LONG();
                            slice.Width = (int)DWORD();
                            slice.Height = (int)DWORD();

                            // NineSlice
                            if (IsBitSet(flags, 0))
                            {
                                slice.NineSlice = new Rectangle(LONG(), LONG(), DWORD(), DWORD());
                            }

                            // pivot point
                            if (IsBitSet(flags, 1))
                                slice.Pivot = new Point((int)DWORD(), (int)DWORD());

                            last = slice;
                            Slices.Add(slice);
                        }
                    }
                    // TILESET
                    else if (chunkType == Chunks.Tileset)
                    {
                        var tileset = new Tileset();

                        tileset.Id = DWORD();
                        int flags = (int)DWORD();
                        tileset.TileCount = (int)DWORD();
                        tileset.TileWidth = WORD();
                        tileset.TileHeight = WORD();
                        short baseIndex = SHORT();

                        _ = BYTES(14);

                        tileset.Name = STRING();

                        // Tileset in external file
                        if (IsBitSet(flags, 0))
                        {
                            var externalFileId = DWORD();
                            var tilesetId = DWORD();
                        }

                        // Tileset is here
                        if (IsBitSet(flags, 1))
                        {
                            tileset.dataLength = (int)DWORD();

                            int count = tileset.TileWidth * tileset.TileHeight * tileset.TileCount * (int)Mode;

                            SEEK(2);

                            var deflate = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
                            var pixelBuffer = new byte[count];
                            tileset.Pixels = new Color[tileset.TileWidth * tileset.TileHeight * tileset.TileCount];

                            deflate.Read(pixelBuffer, 0, count);

                            BytesToPixels(pixelBuffer, tileset.Pixels, Mode, palette);
                        }

                        Tilesets.Add(tileset);
                    }
                    else if (chunkType == Chunks.ColorProfile)
                    {
                    }
                    else if (chunkType == Chunks.OldPaletteA)
                    {
                    }
                    else if (chunkType == Chunks.OldPaletteB)
                    {
                    }
                    else
                    {
                        GameLogger.Log($"Aseprite file {file} has an unknown chunk type");
                    }


                    reader.BaseStream.Position = chunkEnd;
                }

                reader.BaseStream.Position = frameEnd;
            }
        }


        // Bake the cel pixels into the frames
        for (int frameIndex = 0; frameIndex < Frames.Count; frameIndex++)
        {
            var frame = Frames[frameIndex];
            foreach (var cel in frame.Cels.Values)
            {
                if (cel.Link != null)
                {
                    CelToFrame(frame, Frames[cel.Link.Value].Cels[cel.Layer.Index]);
                }
                else
                {
                    CelToFrame(frame, cel);
                }
            }
        }

        // If there are no slices, create one for the whole image so it's easier to deal with it.
        if (Slices.Count == 0)
        {
            Slices.Add(new Slice()
            {
                Name = "",
                Frame = 0,
                OriginX = 0,
                OriginY = 0,
                Width = Width,
                Height = Height
            });
        }
    }

    #endregion

    #region Blend Modes

    // Copied from Aseprite's source code:
    // https://github.com/aseprite/aseprite/blob/master/src/doc/blend_funcs.cpp

    private delegate void Blend(ref Color dest, Color src, byte opacity);

    private readonly static Blend[] _blendModes = new Blend[]
    {
        // 0 - NORMAL
        (ref Color dest, Color src, byte opacity) =>
        {
            // Normalize opacity to a range between 0 and 1 for calculations
            float normalizedOpacity = opacity / 255f;

            // Calculate the effective alpha (transparency) of the source color,
            // factoring in the additional opacity level.
            float srcEffectiveAlpha = src.A / 255f * normalizedOpacity;

            // If the destination is completely transparent,
            // directly apply the source color modified by its effective alpha.
            if (dest.A == 0)
            {
                dest.R = (byte)(src.R * srcEffectiveAlpha);
                dest.G = (byte)(src.G * srcEffectiveAlpha);
                dest.B = (byte)(src.B * srcEffectiveAlpha);
                dest.A = (byte)(src.A * normalizedOpacity);
            }
            // If the source is completely transparent or opacity is 0, leave dest as is.
            else if (src.A == 0 || opacity == 0)
            {
                // No operation needed; dest remains unchanged.
            }
            else
            {
                // For non-transparent blending, calculate the final color.
                // Pre-multiply source color components by their effective alpha.
                float srcPreR = src.R * srcEffectiveAlpha;
                float srcPreG = src.G * srcEffectiveAlpha;
                float srcPreB = src.B * srcEffectiveAlpha;

                // Normalize destination alpha for calculations.
                float destNormalizedAlpha = dest.A / 255f;

                // Blend each component.
                dest.R = (byte)((srcPreR + dest.R * destNormalizedAlpha * (1 - srcEffectiveAlpha)) / (srcEffectiveAlpha + destNormalizedAlpha * (1 - srcEffectiveAlpha)));
                dest.G = (byte)((srcPreG + dest.G * destNormalizedAlpha * (1 - srcEffectiveAlpha)) / (srcEffectiveAlpha + destNormalizedAlpha * (1 - srcEffectiveAlpha)));
                dest.B = (byte)((srcPreB + dest.B * destNormalizedAlpha * (1 - srcEffectiveAlpha)) / (srcEffectiveAlpha + destNormalizedAlpha * (1 - srcEffectiveAlpha)));

                // Calculate and apply the final alpha.
                dest.A = (byte)((srcEffectiveAlpha + destNormalizedAlpha * (1 - srcEffectiveAlpha)) * 255);
            }
        }
    };


    private static int MUL_UN8(int a, int b)
    {
        var t = a * b + 0x80;
        t = (t >> 8) + t >> 8;
        return t;
    }

    #endregion

    #region Utils


    /// <summary>
    /// Converts an array of Bytes to an array of Tiles
    /// </summary>
    private void BytesToTiles(byte[] bytes, Tile[] tiles, int bitsPerTile, uint bitmaskId, uint bitmaskFlipX, uint bitmaskFlipY, uint bitmaskRotate)
    {
        int len = tiles.Length * bitsPerTile;
        for (int p = 0; p < len; p += bitsPerTile)
        {
            int tileInfo = 0;
            Tile tile = new();

            for (int i = 0; i < bitsPerTile; i++)
            {
                // 0x0000101 => 5
                tileInfo += bytes[p + i] << i;
            }

            tile.Id = (int)(tileInfo & bitmaskId);
            tile.FlipX = (tileInfo & bitmaskFlipX) == 1;
            tile.FlipY = (tileInfo & bitmaskFlipX) == 1;
            tile.Rotate = (tileInfo & bitmaskRotate) == 1;

            tiles[p / bitsPerTile] = tile;
        }

    }


    /// <summary>
    /// Converts an array of Bytes to an array of Colors, using the specific Aseprite Mode & Palette
    /// </summary>
    private void BytesToPixels(byte[] bytes, Color[] pixels, Modes mode, Color[] palette)
    {
        int len = pixels.Length;
        if (mode == Modes.RGBA)
        {
            for (int p = 0, b = 0; p < len; p++, b += 4)
            {
                pixels[p].R = (byte)(bytes[b + 0] * bytes[b + 3] / 255);
                pixels[p].G = (byte)(bytes[b + 1] * bytes[b + 3] / 255);
                pixels[p].B = (byte)(bytes[b + 2] * bytes[b + 3] / 255);
                pixels[p].A = bytes[b + 3];
            }
        }
        else if (mode == Modes.Grayscale)
        {
            for (int p = 0, b = 0; p < len; p++, b += 2)
            {
                pixels[p].R = pixels[p].G = pixels[p].B = (byte)(bytes[b + 0] * bytes[b + 1] / 255);
                pixels[p].A = bytes[b + 1];
            }
        }
        else if (mode == Modes.Indexed)
        {
            for (int p = 0, b = 0; p < len; p++, b++)
                pixels[p] = palette[bytes[b]];
        }
    }

    /// <summary>
    /// Applies a Cel's pixels to the Frame, using its Layer's BlendMode & Alpha
    /// </summary>
    private void CelToFrame(Frame frame, Cel cel)
    {
        var opacity = (byte)(cel.Alpha * cel.Layer.Alpha * 255);
        var blend = _blendModes[cel.Layer.BlendMode];

        for (int cx = 0; cx < cel.Width; cx++)
        {
            for (int cy = 0; cy < cel.Height; cy++)
            {
                var framePosition = new Point(cel.X + cx, cel.Y + cy);
                if (framePosition.X < 0 || framePosition.Y < 0 || framePosition.X >= frame.Sprite.Width || framePosition.Y >= frame.Sprite.Height)
                    continue;

                blend(ref frame.Pixels[framePosition.X + framePosition.Y * frame.Sprite.Width], cel.Pixels[cx + cy * cel.Width], opacity);
            }
        }
    }
    /// <summary>
    /// Transfers cel pixel data to another and ensures that it's in the correct size
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void CelToCel(Cel target, Cel source, int width, int height)
    {
        if (!SplitLayers) // Cel data is only used when the aseprite file individual layers are required
            return;       // we can safely skip this to save time.

        var opacity = (byte)(source.Alpha * source.Layer.Alpha * 255);
        var blend = _blendModes[source.Layer.BlendMode];
        var targetArray = new Microsoft.Xna.Framework.Color[width * height];

        for (int cx = 0; cx < source.Width; cx++)
        {
            for (int cy = 0; cy < source.Height; cy++)
            {
                var framePosition = new Point(source.X + cx, source.Y + cy);
                if (framePosition.X < 0 || framePosition.Y < 0 || framePosition.X >= width || framePosition.Y >= height)
                    continue;

                blend(ref targetArray[framePosition.X + framePosition.Y * width], source.Pixels[cx + cy * source.Width], opacity);
            }
        }

        target.Pixels = targetArray;
        target.X = 0;
        target.Y = 0;
        target.Width = width;
        target.Height = height;
    }

    private void Blit(Color[] destination, int destinationWidth, Color[] source, int sourceWidth, int destX, int destY, int srcX, int srcY, int w, int h)
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                var pixelToPlot = source[(x + srcX) % sourceWidth + (int)MathF.Floor((y + srcY) * sourceWidth)];
                destination[(x + destX) % destinationWidth + (int)MathF.Floor((y + destY) * destinationWidth)] = pixelToPlot;
            }
        }
    }

    internal static bool IsBitSet(int b, int pos)
    {
        return (b & 1 << pos) != 0;
    }

    #endregion

    internal IEnumerable<SpriteAsset> CreateAssets(string atlas)
    {
        if (SplitLayers)
            for (int i = 0; i < Layers.Count; i++)
            {
                if (!Layers[i].Name.Equals("ref", StringComparison.InvariantCultureIgnoreCase))
                    foreach (var ase in CreateAssetsFromSlices(i, atlas))
                        yield return ase;
            }
        else
            foreach (var ase in CreateAssetsFromSlices(-1, atlas))
                yield return ase;
    }

    internal IEnumerable<SpriteAsset> CreateAssetsFromSlices(int layer, string atlas)
    {
        for (int i = 0; i < Slices.Count; i++)
        {
            yield return CreateAsset(layer, i, atlas);
        }
    }


    /// <summary>
    /// Creates a new aseprite asset from the current aseprite file.
    /// </summary>
    /// <param name="layer">The current layer to use, -1 means all layers.</param>
    /// <returns></returns>
    private SpriteAsset CreateAsset(int layer, int sliceIndex, string atlas)
    {
        var source = layer >= 0 ? $"{Source}_{Layers[layer].Name}" : Source;
        if (Slices.Count > 1)
        {
            var sliceName = Slices[sliceIndex].Name;
            source = $"{source}_{sliceName}";
        }
        var dictBuilder = ImmutableDictionary.CreateBuilder<string, Animation>(StringComparer.InvariantCultureIgnoreCase);

        Slice? slice;
        if (sliceIndex <= 0)
        {
            slice = Slices.FirstOrDefault();
        }
        else
        {
            slice = Slices[sliceIndex];
        }

        (bool baked, Guid guid) = GetGuid(layer, sliceIndex);

        SpriteEventData? spriteEventData = null;
        Architect.EditorData.TryGetSpriteEventData()?.Events.TryGetValue(guid, out spriteEventData);

        // Create an empty animation with all frames
        {
            int[] frames;
            float[] durations;
            Dictionary<int, string> events = new();

            if (FrameCount > 1)
            {
                var length = Frames.Count;
                frames = new int[length];
                durations = new float[length];

                for (int frame = 0; frame < length; frame++)
                {
                    frames[frame] = frame;
                    durations[frame] = Frames[frame].Duration;
                    FindEventsInframe(events, frame, 0);
                }
            }
            else
            {
                frames = [0];
                durations = [Frames[0].Duration];
            }

            dictBuilder[string.Empty] = new Animation(frames, durations, events, null);

            // Now that we have a guid, make sure if there are any extra events we need to pull.
            if (spriteEventData is not null)
            {
                spriteEventData.FilterEventsForAnimation(string.Empty, ref events);
            }
        }

        for (int i = 0; i < Tags.Count; i++) // Create individual animations for tags
        {
            var tag = Tags[i];

            int[] frames;
            float[] durations;
            Dictionary<int, string> events = new();

            if (FrameCount > 1)
            {
                var length = tag.To - tag.From + 1;

                switch (tag.LoopDirection)
                {
                    case Tag.LoopDirections.Forward:
                    default:

                        frames = new int[length];
                        durations = new float[length];
                        for (int frame = 0; frame < length; frame++)
                        {
                            frames[frame] = frame + tag.From;
                            durations[frame] = Frames[frame + tag.From].Duration;
                            FindEventsInframe(events, frame, tag.From);
                        }
                        break;
                    case Tag.LoopDirections.Reverse:

                        frames = new int[length];
                        durations = new float[length];
                        for (int frame = length - 1; frame >= 0; frame--)
                        {
                            frames[length - 1 - frame] = frame + tag.From;
                            durations[length - 1 - frame] = Frames[frame + tag.From].Duration;
                            FindEventsInframe(events, frame, tag.From);
                        }
                        break;
                    case Tag.LoopDirections.PingPong:

                        frames = new int[length * 2];
                        durations = new float[length * 2];
                        for (int frame = 0; frame < length; frame++)
                        {
                            frames[frame] = frame + tag.From;
                            durations[frame] = Frames[frame + tag.From].Duration;
                            FindEventsInframe(events, frame, tag.From);
                        }
                        for (int frame = 0; frame < length; frame++)
                        {
                            frames[length * 2 - frame - 1] = frame + tag.From;
                            durations[length * 2 - frame - 1] = Frames[frame + tag.From].Duration;
                            FindEventsInframe(events, frame, tag.From);
                        }
                        break;
                }
            }
            else
            {
                frames = [0];
                durations = [Frames[0].Duration];
                FindEventsInframe(events, 0, 0);
            }

            // Now that we have a guid, make sure if there are any extra events we need to pull.
            if (spriteEventData is not null)
            {
                spriteEventData.FilterEventsForAnimation(tag.Name, ref events);
            }

            dictBuilder[tag.Name] = new Animation(frames, durations, events, AnimationSequence.CreateIfPossible(tag.UserData.Text));
        }

        var framesBuilder = ImmutableArray.CreateBuilder<string>();
        if (FrameCount > 1)
            for (int i = 0; i < FrameCount; i++)
            {
                framesBuilder.Add($"{source}_{i:0000}");
            }
        else
            framesBuilder.Add($"{source}");
        Point pivot = slice?.Pivot != null ? new Point(slice.Pivot.Value.X, slice.Pivot.Value.Y) : Point.Zero;

        // No slice or just get the first
        SpriteAsset asset = new(
            guid: guid,
            atlasId: atlas,
            name: source,
            frames: framesBuilder.ToImmutable(),
            animations: dictBuilder.ToImmutable(),
            origin: pivot,
            size: slice is null ? Point.Zero : new Point(slice.Width, slice.Height),
            nineSlice: slice?.NineSlice ?? Rectangle.Empty
        );

        if (Architect.EditorSettings.SaveAsepriteInfoOnSpriteAsset)
        {
            asset.AsepriteFileInfo = new AsepriteFileInfo()
            {
                Source = FullSource,
                Layer = layer,
                SliceIndex = sliceIndex,
                Baked = baked
            };
        }

        return asset;
    }

    private void FindEventsInframe(Dictionary<int, string> events, int frame, int startFrame)
    {
        foreach (var cel in Frames[frame + startFrame].Cels.Values)
        {
            if (!string.IsNullOrWhiteSpace(cel.UserData.Text))
            {
                events.Add(frame, cel.UserData.Text);
                return;
            }
        }
    }

    private Guid GetGuidLegacy(int layerIndex, int sliceIndex)
    {
        if (layerIndex >= 0)
        {
            using var md5 = MD5.Create();
            // TODO: Shouldn't this take the file path instead on the file name?
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes($"{Name}_{Layers[layerIndex].Name}"));
            GameLogger.Warning($"Aseprite file {Source} doesn't have a baked GUID. Consider baking one on it for safety.");
            return new Guid(hash);
        }
        else
        {
            string keyword = "guid:";
            int keywordLength = keyword.Length;

            foreach (var layer in Layers)
            {
                if (layer.UserData.Text != null && layer.UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return new Guid(layer.UserData.Text.Substring(keywordLength));
                }
            }

            using var md5 = MD5.Create();
            if (Slices.Count > 1)
            {
                var slice = Slices[sliceIndex];
                if (slice.UserData.Text != null && slice.UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return new Guid(slice.UserData.Text.Substring(keywordLength));
                }
                // TODO: Remove this and always serialize using slice index.
                // I'm doing this here so we don't have to relink EVERYTING in road to ghoulcrest
                // Idealy we just add everything to the name and generate the GUID from that (even layers).
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes($"{Name}_{Slices[sliceIndex].Name}"));
                GameLogger.Warning($"Aseprite file {Source} doesn't have a baked GUID. Consider baking one on it for safety.");
                return new Guid(hash);
            }
            else
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes($"{Name}"));
                GameLogger.Warning($"Aseprite file {Source} doesn't have a baked GUID. Consider baking one on it for safety.");
                return new Guid(hash);
            }
        }
    }

    private (bool baked, Guid guid) GetGuid(int layerIndex, int sliceIndex)
    {
        string keyword = "guid:";
        int keywordLength = keyword.Length;

        if (layerIndex >= 0) // This is a split file
        {
            var layer = Layers[layerIndex];
            if (layer.UserData.Text != null && layer.UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return (true, new Guid(layer.UserData.Text.Substring(keywordLength)));
            }

            if (Slices.Count > 1)
            {
                var slice = Slices[sliceIndex];
                if (slice.UserData.Text != null && slice.UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return (true, new Guid(slice.UserData.Text.Substring(keywordLength)));
                }
            }
        }
        else // This is a flattened file
        {
            // First look for GUIDs on slices
            if (Slices.Count >= 1)
            {
                var slice = Slices[sliceIndex];
                if (slice.UserData.Text != null && slice.UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return (true, new Guid(slice.UserData.Text.Substring(keywordLength)));
                }
            }

            // Then look for GUIDs on the sprite user data
            if (UserData.Text != null && UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return (true, new Guid(UserData.Text.Substring(keywordLength)));
            }

            // Finally look for guids in one of the layers
            foreach (var layer in Layers)
            {
                if (layer.UserData.Text != null && layer.UserData.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return (true, new Guid(layer.UserData.Text.Substring(keywordLength)));
                }
            }
        }

        // No baked guids were found. Create one on the fly based on the file name.
        using var md5 = MD5.Create();
        Guid guid = new Guid(md5.ComputeHash(Encoding.Default.GetBytes($"{EditorFileHelper.Normalize(Source)}_{Slices[sliceIndex].Name}")));
        GameLogger.Log($"Aseprite file {Source} doesn't have a baked GUID. Consider baking one on it for safety.");
        return (false, guid);
    }

    /// <summary>
    /// Gets the relative path to the content folder from a rooted one
    /// </summary>
    public static string GetRelativeToContent(string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        string relativePath;

        // Honestly I am just too tired at this point, and I don't want to deal with string comparison AND being memory efficient right now.
        // So. TODO: We should fix this later to work with any path. I can't change this path because it's the one using when building the aseprite data.
        if (path.Contains("editor" + Path.DirectorySeparatorChar))
        {
            relativePath = "editor/";
        }
        else if (path.Contains("preload_images" + Path.DirectorySeparatorChar))
        {
            relativePath = "preload_images/";
        }
        else if (path.Contains("cutscene_images" + Path.DirectorySeparatorChar))
        {
            relativePath = "cutscene_images/";
        }
        else
        {
            relativePath = "images/";
        }

        string contentFolder = FileHelper.GetPath(Architect.EditorSettings.RawResourcesPath, relativePath);
        return Path.GetRelativePath(contentFolder, path);
    }
}