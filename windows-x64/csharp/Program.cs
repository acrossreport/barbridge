// barbridge.dll (BarBridge native library, Windows x64) called from C# / .NET
// via P/Invoke. No build step beyond `dotnet run`.
//
// Run:
//     cd windows-x64/csharp
//     dotnet run

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BarBridgeSample;

internal static class Native
{
    private const string LibName = "barbridge";

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int barbridge_render_png(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string requestJson,
        out IntPtr outData,
        out UIntPtr outLen);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void barbridge_bytes_free(IntPtr data, UIntPtr len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr barbridge_last_error();
}

internal static class Program
{
    private static void RegisterResolver()
    {
        // windows-x64/csharp から見て windows-x64 (このプラットフォーム用ツリーのルート) は1階層上。
        var sampleDir = AppContext.BaseDirectory;
        var platformRoot = Path.GetFullPath(Path.Combine(sampleDir, "..", "..", "..", ".."));
        var candidate = Environment.GetEnvironmentVariable("BARBRIDGE_NATIVE_LIB")
            ?? Path.Combine(platformRoot, "native", "barbridge.dll");

        NativeLibrary.SetDllImportResolver(typeof(Program).Assembly, (name, assembly, searchPath) =>
        {
            if (name != "barbridge") return IntPtr.Zero;
            if (File.Exists(candidate))
            {
                return NativeLibrary.Load(candidate);
            }
            throw new FileNotFoundException($"native library not found: {candidate}");
        });
    }

    private static int Main()
    {
        RegisterResolver();

        var sampleDir = AppContext.BaseDirectory;
        var commonDir = Path.GetFullPath(Path.Combine(sampleDir, "..", "..", "..", "..", "common"));
        var requestJson = File.ReadAllText(Path.Combine(commonDir, "request.json"));

        var status = Native.barbridge_render_png(requestJson, out var outData, out var outLen);
        if (status != 0)
        {
            var err = Marshal.PtrToStringUTF8(Native.barbridge_last_error());
            Console.Error.WriteLine($"barbridge_render_png failed (status={status}): {err}");
            return 1;
        }

        try
        {
            var len = (int)outLen;
            var pngBytes = new byte[len];
            Marshal.Copy(outData, pngBytes, 0, len);
            var outPath = Path.Combine(AppContext.BaseDirectory, "output.png");
            File.WriteAllBytes(outPath, pngBytes);
            Console.WriteLine($"wrote {pngBytes.Length} bytes -> {outPath}");
        }
        finally
        {
            Native.barbridge_bytes_free(outData, outLen);
        }

        return 0;
    }
}
