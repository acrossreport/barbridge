package com.acrossreport.sample;

import com.sun.jna.Library;
import com.sun.jna.Native;
import com.sun.jna.Pointer;
import com.sun.jna.ptr.LongByReference;
import com.sun.jna.ptr.PointerByReference;

/**
 * barbridge（BarBridgeのネイティブ共有ライブラリ）の FFI 境界を JNA で
 * マッピングしたもの。uintptr_t（Windows ARM64 では8バイト）は Java の
 * long（8バイト）に対応する LongByReference でマッピングしている。
 */
public interface BarBridge extends Library {

    BarBridge INSTANCE = Native.load("barbridge", BarBridge.class);

    int barbridge_render_png(String requestJson, PointerByReference outData, LongByReference outLen);

    void barbridge_bytes_free(Pointer data, long len);

    Pointer barbridge_last_error();
}
