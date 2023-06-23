using Murder.Diagnostics;
using System.Runtime.InteropServices;

namespace Murder.Editor.Utilities
{
    public static class OperatingSystemHelpers
    {
        public static void SetTextForOsx(string text)
        {
            // Reference: https://stackoverflow.com/a/51997266
            if (OperatingSystem.IsMacOS())
            {
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

                    return;
                }
                catch { }
            }

            GameLogger.Warning("We did not implement clipboard for the target operating system.");
        }

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_getClass(string className);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr sel_registerName(string selectorName);

        const string NSPasteboardTypeString = "public.utf8-plain-text";
    }
}
