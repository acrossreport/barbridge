"""
barbridge.dll (BarBridge native library, Windows x64) called from Python
via ctypes. No build step; only the standard library is used.

Run:
    python render_sample.py
"""

import ctypes
import os
import pathlib
import sys

SAMPLE_DIR = pathlib.Path(__file__).resolve().parent
PLATFORM_ROOT = SAMPLE_DIR.parent
COMMON_DIR = PLATFORM_ROOT / "common"
NATIVE_LIB = PLATFORM_ROOT / "native" / "barbridge.dll"


def load_library() -> ctypes.CDLL:
    lib_path = pathlib.Path(os.environ.get("BARBRIDGE_NATIVE_LIB", str(NATIVE_LIB)))
    if not lib_path.exists():
        raise FileNotFoundError(f"native library not found: {lib_path}")
    lib = ctypes.CDLL(str(lib_path))

    lib.barbridge_render_png.argtypes = [
        ctypes.c_char_p,
        ctypes.POINTER(ctypes.POINTER(ctypes.c_ubyte)),
        ctypes.POINTER(ctypes.c_size_t),
    ]
    lib.barbridge_render_png.restype = ctypes.c_int32

    lib.barbridge_bytes_free.argtypes = [ctypes.POINTER(ctypes.c_ubyte), ctypes.c_size_t]
    lib.barbridge_bytes_free.restype = None

    lib.barbridge_last_error.restype = ctypes.c_char_p

    return lib


def main() -> int:
    lib = load_library()

    request_json = (COMMON_DIR / "request.json").read_text(encoding="utf-8")

    out_data = ctypes.POINTER(ctypes.c_ubyte)()
    out_len = ctypes.c_size_t(0)
    status = lib.barbridge_render_png(request_json.encode("utf-8"), ctypes.byref(out_data), ctypes.byref(out_len))
    if status != 0:
        print(f"barbridge_render_png failed (status={status}): "
              f"{lib.barbridge_last_error().decode('utf-8', errors='replace')}", file=sys.stderr)
        return 1

    try:
        png_bytes = ctypes.string_at(out_data, out_len.value)
        out_path = SAMPLE_DIR / "output.png"
        out_path.write_bytes(png_bytes)
        print(f"wrote {len(png_bytes)} bytes -> {out_path}")
    finally:
        lib.barbridge_bytes_free(out_data, out_len)

    return 0


if __name__ == "__main__":
    sys.exit(main())
