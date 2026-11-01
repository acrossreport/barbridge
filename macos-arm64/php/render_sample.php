<?php
/*
 * libbarbridge.dylib (BarBridge native library, macOS ARM64 (Apple Silicon)) called from PHP
 * via the built-in FFI extension (PHP 7.4+). No build step.
 *
 * Requires `extension=ffi` enabled in php.ini (and, outside the CLI,
 * `ffi.enable=true`).
 *
 * Run:
 *     php render_sample.php
 */

$sampleDir = __DIR__;
$platformRoot = dirname($sampleDir);
$commonDir = $platformRoot . DIRECTORY_SEPARATOR . 'common';
$nativeLib = getenv('BARBRIDGE_NATIVE_LIB') ?: ($platformRoot . DIRECTORY_SEPARATOR . 'native' . DIRECTORY_SEPARATOR . 'libbarbridge.dylib');

if (!file_exists($nativeLib)) {
    fwrite(STDERR, "native library not found: $nativeLib\n");
    exit(1);
}

$ffi = FFI::cdef(<<<CDEF
    int32_t barbridge_render_png(const char *request_json, uint8_t **out_data, size_t *out_len);
    void barbridge_bytes_free(uint8_t *data, size_t len);
    const char *barbridge_last_error(void);
CDEF, $nativeLib);

$requestJson = file_get_contents($commonDir . DIRECTORY_SEPARATOR . 'request.json');

$outData = FFI::new('uint8_t*');
$outLen = FFI::new('size_t');

$status = $ffi->barbridge_render_png($requestJson, FFI::addr($outData), FFI::addr($outLen));
if ($status !== 0) {
    $err = FFI::string($ffi->barbridge_last_error());
    fwrite(STDERR, "barbridge_render_png failed (status=$status): $err\n");
    exit(1);
}

try {
    $pngBytes = FFI::string($outData, $outLen->cdata);
    $outPath = $sampleDir . DIRECTORY_SEPARATOR . 'output.png';
    file_put_contents($outPath, $pngBytes);
    echo 'wrote ' . strlen($pngBytes) . " bytes -> $outPath\n";
} finally {
    $ffi->barbridge_bytes_free($outData, $outLen->cdata);
}
