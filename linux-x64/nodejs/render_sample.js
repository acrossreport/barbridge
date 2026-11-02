// libbarbridge.so (BarBridge native library, Linux x64) called from Node.js
// via koffi (a prebuilt-binary FFI package; no native build toolchain
// required, unlike ffi-napi/node-gyp).
//
// Setup:
//     npm install
// Run:
//     node render_sample.js

const fs = require("fs");
const path = require("path");
const koffi = require("koffi");

const sampleDir = __dirname;
const platformRoot = path.dirname(sampleDir);
const commonDir = path.join(platformRoot, "common");
const nativeLib = process.env.BARBRIDGE_NATIVE_LIB || path.join(platformRoot, "native", "libbarbridge.so");

if (!fs.existsSync(nativeLib)) {
  console.error(`native library not found: ${nativeLib}`);
  process.exit(1);
}

const lib = koffi.load(nativeLib);

const barbridge_render_png = lib.func(
  "int32_t barbridge_render_png(const char *request_json, _Out_ uint8_t **out_data, _Out_ size_t *out_len)"
);
const barbridge_bytes_free = lib.func("void barbridge_bytes_free(uint8_t *data, size_t len)");
const barbridge_last_error = lib.func("const char *barbridge_last_error()");

const requestJson = fs.readFileSync(path.join(commonDir, "request.json"), "utf8");

const outData = [null];
const outLen = [0];
const status = barbridge_render_png(requestJson, outData, outLen);
if (status !== 0) {
  console.error(`barbridge_render_png failed (status=${status}): ${barbridge_last_error()}`);
  process.exit(1);
}

try {
  const pngBytes = koffi.decode(outData[0], "uint8_t", outLen[0]);
  const buffer = Buffer.from(pngBytes);
  const outPath = path.join(sampleDir, "output.png");
  fs.writeFileSync(outPath, buffer);
  console.log(`wrote ${buffer.length} bytes -> ${outPath}`);
} finally {
  barbridge_bytes_free(outData[0], outLen[0]);
}
