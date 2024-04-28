using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System.Runtime.InteropServices;

namespace Murder.Editor.Core;

public enum SystemCursor
{
    Arrow,
    IBeam,
    Wait,
    Crosshair,
    WaitArrow,
    SizeNWSE,
    SizeNESW,
    SizeWE,
    SizeNS,
    SizeAll,
    No,
    Hand
}

/// <summary>
/// Describes a mouse cursor.
/// </summary>
public partial class MouseCursor : IDisposable
{
    /// <summary>
    /// Gets the default arrow cursor.
    /// </summary>
    public static MouseCursor Arrow { get; private set; }

    /// <summary>
    /// Gets the cursor that appears when the mouse is over text editing regions.
    /// </summary>
    public static MouseCursor IBeam { get; private set; }

    /// <summary>
    /// Gets the waiting cursor that appears while the application/system is busy.
    /// </summary>
    public static MouseCursor Wait { get; private set; }

    /// <summary>
    /// Gets the crosshair ("+") cursor.
    /// </summary>
    public static MouseCursor Crosshair { get; private set; }

    /// <summary>
    /// Gets the cross between Arrow and Wait cursors.
    /// </summary>
    public static MouseCursor WaitArrow { get; private set; }

    /// <summary>
    /// Gets the northwest/southeast ("\") cursor.
    /// </summary>
    public static MouseCursor SizeNWSE { get; private set; }

    /// <summary>
    /// Gets the northeast/southwest ("/") cursor.
    /// </summary>
    public static MouseCursor SizeNESW { get; private set; }

    /// <summary>
    /// Gets the horizontal west/east ("-") cursor.
    /// </summary>
    public static MouseCursor SizeWE { get; private set; }

    /// <summary>
    /// Gets the vertical north/south ("|") cursor.
    /// </summary>
    public static MouseCursor SizeNS { get; private set; }

    /// <summary>
    /// Gets the size all cursor which points in all directions.
    /// </summary>
    public static MouseCursor SizeAll { get; private set; }

    /// <summary>
    /// Gets the cursor that points that something is invalid, usually a cross.
    /// </summary>
    public static MouseCursor No { get; private set; }

    /// <summary>
    /// Gets the hand cursor, usually used for web links.
    /// </summary>
    public static MouseCursor Hand { get; private set; }

    static MouseCursor()
    {
        Arrow = new MouseCursor(SystemCursor.Arrow);
        IBeam = new MouseCursor(SystemCursor.IBeam);
        Wait = new MouseCursor(SystemCursor.Wait);
        Crosshair = new MouseCursor(SystemCursor.Crosshair);
        WaitArrow = new MouseCursor(SystemCursor.WaitArrow);
        SizeNWSE = new MouseCursor(SystemCursor.SizeNWSE);
        SizeNESW = new MouseCursor(SystemCursor.SizeNESW);
        SizeWE = new MouseCursor(SystemCursor.SizeWE);
        SizeNS = new MouseCursor(SystemCursor.SizeNS);
        SizeAll = new MouseCursor(SystemCursor.SizeAll);
        No = new MouseCursor(SystemCursor.No);
        Hand = new MouseCursor(SystemCursor.Hand);
    }

    private MouseCursor(SystemCursor cursor)
    {
        Handle = SDL.SDL_CreateSystemCursor((SDL.SDL_SystemCursor)cursor);
    }

    /// <summary>
    /// Creates a mouse cursor from the specified texture.
    /// </summary>
    /// <param name="texture">Texture to use as the cursor image.</param>
    /// <param name="originx">X cordinate of the image that will be used for mouse position.</param>
    /// <param name="originy">Y cordinate of the image that will be used for mouse position.</param>
    public static MouseCursor FromTexture2D(Texture2D texture, int originx, int originy)
    {
        IntPtr surface = IntPtr.Zero;
        IntPtr handle = IntPtr.Zero;
        try
        {
            var bytes = new byte[texture.Width * texture.Height * 4];
            texture.GetData(bytes);
            surface = CreateRGBSurfaceFrom(bytes, texture.Width, texture.Height, 32, texture.Width * 4, 0x000000ff, 0x0000FF00, 0x00FF0000, 0xFF000000);

            if (surface == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create surface for mouse cursor: " + SDL.SDL_GetError());
            }

            handle = SDL.SDL_CreateColorCursor(surface, originx, originy);
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to set surface for mouse cursor: " + SDL.SDL_GetError());
            }
        }
        finally
        {
            if (surface != IntPtr.Zero)
            {
                SDL.SDL_FreeSurface(surface);
            }
        }

        return new MouseCursor(handle);
    }

    public static IntPtr CreateRGBSurfaceFrom(byte[] pixels, int width, int height, int depth, int pitch, uint rMask, uint gMask, uint bMask, uint aMask)
    {
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            return SDL.SDL_CreateRGBSurfaceFrom(handle.AddrOfPinnedObject(), width, height, depth, pitch, rMask, gMask, bMask, aMask);
        }
        finally
        {
            handle.Free();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        PlatformDispose();
        _disposed = true;
    }

    private void PlatformDispose()
    {
        if (Handle == IntPtr.Zero)
        {
            return;
        }

        SDL.SDL_FreeCursor(Handle);
        Handle = IntPtr.Zero;
    }

    public IntPtr Handle { get; private set; }

    private bool _disposed;

    private MouseCursor(IntPtr handle)
    {
        Handle = handle;
    }
}
