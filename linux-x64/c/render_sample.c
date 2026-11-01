/*
 * libbarbridge.so (BarBridge native library, Linux x64) called from C.
 * No build step beyond a single compiler invocation; no runtime dependency
 * besides the shared library itself.
 *
 * Build (gcc or clang):
 *     gcc render_sample.c -I ../.. -L ../native -lbarbridge -o render_sample
 *     LD_LIBRARY_PATH=../native ./render_sample
 *
 * (barbridge.h lives at the repo root, two levels up from this file; -I
 * ../.. reaches it. ../native holds libbarbridge.so, which the dynamic
 * linker needs to find at runtime, hence LD_LIBRARY_PATH.)
 */

#include <stdio.h>
#include <stdlib.h>
#include "barbridge.h"

int main(void) {
    FILE *f = fopen("../common/request.json", "rb");
    if (!f) { fprintf(stderr, "cannot open ../common/request.json\n"); return 1; }
    fseek(f, 0, SEEK_END);
    long len = ftell(f);
    fseek(f, 0, SEEK_SET);
    char *request_json = (char *)malloc((size_t)len + 1);
    fread(request_json, 1, (size_t)len, f);
    request_json[len] = '\0';
    fclose(f);

    uint8_t *png_data = NULL;
    uintptr_t png_len = 0;
    int32_t status = barbridge_render_png(request_json, &png_data, &png_len);
    free(request_json);

    if (status != 0) {
        fprintf(stderr, "barbridge_render_png failed (status=%d): %s\n",
                status, barbridge_last_error());
        return 1;
    }

    FILE *out = fopen("output.png", "wb");
    fwrite(png_data, 1, (size_t)png_len, out);
    fclose(out);
    printf("wrote %zu bytes -> output.png\n", (size_t)png_len);

    barbridge_bytes_free(png_data, png_len);
    return 0;
}
