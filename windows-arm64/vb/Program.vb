' barbridge.dll (BarBridge native library, Windows ARM64) called from VB.NET
' via P/Invoke. No build step beyond `dotnet run`.
'
' Run:
'     cd windows-arm64/vb
'     dotnet run

Imports System
Imports System.IO
Imports System.Runtime.InteropServices

Module Native
    Private Const LibName As String = "barbridge"

    <DllImport(LibName, CallingConvention:=CallingConvention.Cdecl)>
    Public Function barbridge_render_png(
        <MarshalAs(UnmanagedType.LPUTF8Str)> requestJson As String,
        <Out> ByRef outData As IntPtr,
        <Out> ByRef outLen As UIntPtr) As Integer
    End Function

    <DllImport(LibName, CallingConvention:=CallingConvention.Cdecl)>
    Public Sub barbridge_bytes_free(data As IntPtr, len As UIntPtr)
    End Sub

    <DllImport(LibName, CallingConvention:=CallingConvention.Cdecl)>
    Public Function barbridge_last_error() As IntPtr
    End Function
End Module

Module Program

    Private Sub RegisterResolver()
        ' windows-arm64/vb から見て windows-arm64 (このプラットフォーム用ツリーのルート) は1階層上。
        Dim sampleDir As String = AppContext.BaseDirectory
        Dim platformRoot As String = Path.GetFullPath(Path.Combine(sampleDir, "..", "..", "..", ".."))
        Dim candidate As String = Environment.GetEnvironmentVariable("BARBRIDGE_NATIVE_LIB")
        If String.IsNullOrEmpty(candidate) Then
            candidate = Path.Combine(platformRoot, "native", "barbridge.dll")
        End If

        NativeLibrary.SetDllImportResolver(GetType(Program).Assembly,
            Function(name As String, assembly As Reflection.Assembly, searchPath As DllImportSearchPath?) As IntPtr
                If name <> "barbridge" Then Return IntPtr.Zero
                If File.Exists(candidate) Then
                    Return NativeLibrary.Load(candidate)
                End If
                Throw New FileNotFoundException($"native library not found: {candidate}")
            End Function)
    End Sub

    Function Main() As Integer
        RegisterResolver()

        Dim sampleDir As String = AppContext.BaseDirectory
        Dim commonDir As String = Path.GetFullPath(Path.Combine(sampleDir, "..", "..", "..", "..", "common"))
        Dim requestJson As String = File.ReadAllText(Path.Combine(commonDir, "request.json"))

        Dim outData As IntPtr
        Dim outLen As UIntPtr
        Dim status As Integer = Native.barbridge_render_png(requestJson, outData, outLen)
        If status <> 0 Then
            Dim err As String = Marshal.PtrToStringUTF8(Native.barbridge_last_error())
            Console.Error.WriteLine($"barbridge_render_png failed (status={status}): {err}")
            Return 1
        End If

        Try
            Dim len As Integer = CInt(outLen)
            Dim pngBytes(len - 1) As Byte
            Marshal.Copy(outData, pngBytes, 0, len)
            Dim outPath As String = Path.Combine(AppContext.BaseDirectory, "output.png")
            File.WriteAllBytes(outPath, pngBytes)
            Console.WriteLine($"wrote {pngBytes.Length} bytes -> {outPath}")
        Finally
            Native.barbridge_bytes_free(outData, outLen)
        End Try

        Return 0
    End Function

End Module
