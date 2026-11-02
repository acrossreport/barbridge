package com.acrossreport.sample;

import com.sun.jna.Pointer;
import com.sun.jna.ptr.LongByReference;
import com.sun.jna.ptr.PointerByReference;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

/**
 * libbarbridge.dylib（BarBridgeのネイティブ共有ライブラリ、macOS x64 (Intel)版）を
 * Java から呼び出す最小サンプル。
 *
 * 実行:
 *   cd macos-x64/java
 *   mvn package
 *   java -jar target/barbridge-sample-jar-with-dependencies.jar
 */
public final class Main {

    private Main() {
    }

    public static void main(String[] args) throws IOException {
        // macos-x64/java から見て macos-x64 (このプラットフォーム用ツリーのルート) は1階層上。
        Path sampleDir = Paths.get("").toAbsolutePath();
        Path platformRoot = sampleDir.resolve("..").normalize();
        String envNativeLib = System.getenv("BARBRIDGE_NATIVE_LIB");
        Path nativeDir = (envNativeLib != null)
                ? Paths.get(envNativeLib)
                : platformRoot.resolve("native");

        if (!Files.exists(nativeDir.resolve("libbarbridge.dylib"))) {
            System.err.println("native library not found: " + nativeDir.resolve("libbarbridge.dylib"));
            System.exit(1);
        }

        System.setProperty("jna.library.path", nativeDir.toString());
        System.setProperty("jna.encoding", "UTF-8");

        BarBridge lib = BarBridge.INSTANCE;

        Path commonDir = platformRoot.resolve("common");
        String requestJson = Files.readString(commonDir.resolve("request.json"), StandardCharsets.UTF_8);

        PointerByReference outData = new PointerByReference();
        LongByReference outLen = new LongByReference();
        int status = lib.barbridge_render_png(requestJson, outData, outLen);
        if (status != 0) {
            String err = lib.barbridge_last_error().getString(0, "UTF-8");
            System.err.println("barbridge_render_png failed (status=" + status + "): " + err);
            System.exit(1);
        }

        Pointer dataPtr = outData.getValue();
        long len = outLen.getValue();
        try {
            byte[] pngBytes = dataPtr.getByteArray(0, (int) len);
            Path outPath = sampleDir.resolve("output.png");
            Files.write(outPath, pngBytes);
            System.out.println("wrote " + pngBytes.length + " bytes -> " + outPath);
        } finally {
            lib.barbridge_bytes_free(dataPtr, len);
        }
    }
}
