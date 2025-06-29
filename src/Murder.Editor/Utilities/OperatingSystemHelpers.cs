using Murder.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Murder.Editor.Utilities;

public unsafe static class OperatingSystemHelpers
{
    private static IntPtr _clipboard;

    public static bool HasMacOsClipboardDependency() => File.Exists(AppKit);

    public static bool HasLinuxClipboardDependency()
    {
        try
        {
            Marshal.Prelink(typeof(OperatingSystemHelpers).GetMethod(nameof(OperatingSystemHelpers.SDL_GetClipboardText),
                BindingFlags.NonPublic | BindingFlags.Static)!);
            Marshal.Prelink(typeof(OperatingSystemHelpers).GetMethod(nameof(OperatingSystemHelpers.SDL_SetClipboardText),
                BindingFlags.NonPublic | BindingFlags.Static)!);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool ClipboardDependencyExists()
    {
        if (OperatingSystem.IsMacOS())
        {
            return HasMacOsClipboardDependency();
        }

        if (OperatingSystem.IsLinux())
        {
            return HasLinuxClipboardDependency();
        }

        return false;
    }

    public static readonly IntPtr? GetFnPtr = typeof(OperatingSystemHelpers)
        .GetMethod(nameof(Get), BindingFlags.NonPublic | BindingFlags.Static)?.MethodHandle.GetFunctionPointer();

    public static readonly IntPtr? SetFnPtr = typeof(OperatingSystemHelpers)
        .GetMethod(nameof(Set), BindingFlags.NonPublic | BindingFlags.Static)?.MethodHandle.GetFunctionPointer();

    private static unsafe byte* Get(void* userdata)
    {
        if (_clipboard != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_clipboard);
            _clipboard = IntPtr.Zero;
        }

        string result = string.Empty;
        int length = 0;

        if (OperatingSystem.IsMacOS())
        {
            result = GetTextForOsx() ?? string.Empty;
            length = Encoding.UTF8.GetByteCount(result);
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                result = SDL_GetClipboardText() ?? string.Empty;
                length = Encoding.UTF8.GetByteCount(result);
            }
            catch { }
        }

        var bytes = (byte*)(_clipboard = Marshal.AllocHGlobal(length + 1));

        Encoding.UTF8.GetBytes(result, new Span<byte>(bytes, length));
        bytes[length] = 0;

        return bytes;
    }

    private static unsafe void Set(void* userdata, byte* text)
    {
        int len = 0;
        while (text[len] != 0) len++;

        string? result = Encoding.UTF8.GetString(text, len) ?? string.Empty;

        if (OperatingSystem.IsMacOS())
        {
            SetTextForOsx(result);
        }

        if (OperatingSystem.IsLinux())
        {
            SDL_SetClipboardText(result);
        }
    }

    private static bool SetTextForOsx(string text)
    {
        // Reference: https://stackoverflow.com/a/51997266

        try
        {
            var nsString = objc_getClass("NSString");
            var str = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), text);
            var dataType = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), NSPasteboardTypeString);

            var nsPasteboard = objc_getClass("NSPasteboard");
            var generalPasteboard = objc_msgSend(nsPasteboard, sel_registerName("generalPasteboard"));

            objc_msgSend(generalPasteboard, sel_registerName("clearContents"));
            objc_msgSend(generalPasteboard, sel_registerName("setString:forType:"), str, dataType);

            objc_msgSend(str, sel_registerName("release"));
            objc_msgSend(dataType, sel_registerName("release"));

            return true;
        }
        catch { }

        return false;
    }

    private static string? GetTextForOsx()
    {
        try
        {
            var nsString = objc_getClass("NSString");
            var nsPasteboard = objc_getClass("NSPasteboard");

            var nsStringPboardType = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), "NSStringPboardType");
            var generalPasteboard = objc_msgSend(nsPasteboard, sel_registerName("generalPasteboard"));
            var ptr = objc_msgSend(generalPasteboard, sel_registerName("stringForType:"), nsStringPboardType);
            var charArray = objc_msgSend(ptr, sel_registerName("UTF8String"));
            return Marshal.PtrToStringAnsi(charArray);
        }
        catch { }

        return null;
    }

    public const string AppKit = "/System/Library/Frameworks/AppKit.framework/AppKit";

    [DllImport(AppKit)]
    static extern IntPtr objc_getClass(string className);

    [DllImport(AppKit)]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport(AppKit)]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

    [DllImport(AppKit)]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

    [DllImport(AppKit)]
    static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport(AppKit)]
    static extern IntPtr sel_registerName(string selectorName);

    const string NSPasteboardTypeString = "public.utf8-plain-text";

    public const string SDL = "libSDL3.so";

    [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
    private static extern int SDL_SetClipboardText(string text);

    [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
    private static extern string SDL_GetClipboardText();
}