# BarBridge Technical Note: GS1 Sunrise 2027 — Dual-Marking 2D Barcodes on Thermal Labels (203 dpi / 300 dpi)

Date: 2026-11-05
Published by: BarBridge project (Across Report Renderer / ACR)

> This document is a technical study by the BarBridge project (ACR). The figures shown are estimates calculated from sample data. It does not represent the official position of GS1 or GS1 Japan.

---

## 1. Background

- GS1 Sunrise 2027 is an industry target for retail POS systems to be able to scan 2D barcodes (GS1 DataMatrix / QR Code with GS1 Digital Link) by the end of 2027. It is not a mandate.
- During the transition, 2D barcodes are expected to be **dual-marked** alongside the JAN (EAN-13) barcode, so the label area required increases.
- **203 dpi thermal printers**, which are common in Japan, can only produce module widths that are integer multiples of the dot pitch. This makes it hard to meet the specification and fit the label size at the same time.

## 2. Relevant Specifications

| Item | Requirement | Source |
|---|---|---|
| 2D X-dimension (retail POS) | Min 0.396 mm / Target 0.495 mm / Max 0.990 mm | GS1 General Specifications 5.12.3.1 (Symbol Specification Table 1) |
| JAN (EAN-13) X-dimension | Nominal 0.33 mm (100%), min 0.264 mm (80%) | GS1 General Specifications |
| Quiet zone | QR: 4X (all sides); GS1 DataMatrix: 1X (all sides) | 2D in Retail Guideline |
| Placement (50 mm rule) | The 2D barcode must be **within 50 mm of the centre of the linear barcode** (to achieve a POS throughput of 40–70 items per minute) | 2D in Retail Guideline 4.1.2 |
| QR error correction | L 7% / M 15% / Q 25% / H 30% (higher levels make the symbol larger) | Same as above |

## 3. Dot Count vs. X-Dimension

| Dots | 203 dpi (1 dot = 0.125 mm) | 300 dpi (1 dot = 0.0847 mm) |
|---|---|---|
| 2 dots | 0.250 mm | — |
| 3 dots | 0.375 mm | 0.254 mm |
| 4 dots | 0.500 mm | 0.339 mm |
| 5 dots | 0.626 mm | 0.423 mm |
| 6 dots | — | 0.508 mm |

### Available options

| Purpose | 203 dpi | 300 dpi |
|---|---|---|
| 2D minimum (≥ 0.396 mm) | **4 dots = 0.500 mm** (3 dots = 0.375 mm is out of spec) | **5 dots = 0.423 mm** |
| 2D target (≥ 0.495 mm) | 4 dots = 0.500 mm | 6 dots = 0.508 mm |
| JAN (≥ 0.264 mm) | **3 dots = 0.375 mm (113.7%)** (2 dots = 0.25 mm is out of spec) | **4 dots = 0.339 mm (102.6%)** |

→ **At 203 dpi, 4 dots is the effective minimum for 2D, and the JAN is fixed at 3 dots.**

## 4. Symbol Size Estimates

### QR Code (GS1 Digital Link)

| Data content (example) | Characters | Version | Modules (incl. quiet zone) |
|---|---|---|---|
| id.gs1.org + GTIN only | 36 | V3 | 29 + 8 = 37 |
| + batch/lot + expiry | 56 | V4 | 33 + 8 = 41 |
| Own domain + lot + serial + expiry (ECC M) | 85 | V6 | 41 + 8 = 49 |

### GS1 DataMatrix (element strings)

| Data content (example) | Size | Modules (incl. quiet zone) |
|---|---|---|
| (01) GTIN | 16×16 | 18 |
| (01) + (17) expiry + (10) lot | 20×20 | 22 |
| + (21) serial | 22×22 | 24 |

## 5. Verifying the 50 mm Rule (203 dpi)

### Assumptions

- JAN at 3 dots (X = 0.375 mm). Bar height 26.0 mm, height including HRI 29.5 mm, width including quiet zones 42.4 mm.
- The "centre of the JAN" is taken as the centre of the bar area (HRI excluded).
- Distance from the centre to the outer edge: right 20.5 mm / left 22.0 mm (left quiet zone is 11X) / top 13.0 mm / bottom 16.5 mm (because of the HRI).
- The 2D barcode is centred on the JAN axis, and quiet zones do not overlap.
- Distance is evaluated as the Euclidean distance to the farthest corner of the 2D barcode (the strictest interpretation).

### Distance from JAN centre to the farthest corner of the 2D barcode (mm)

| 2D barcode | Dots | Side | Right | Left | Top | Bottom |
|---|---|---|---|---|---|---|
| QR V3 (GTIN only) | 4 | 18.5 | 40.1 | 41.5 | 32.8 | 36.2 |
| QR V4 (+ lot, expiry) | 4 | 20.5 | 42.2 | 43.7 | 35.1 | 38.4 |
| QR V6 (+ serial) | 4 | 24.5 | 46.6 | 48.1 | 39.5 | 42.8 |
| QR V6 (+ serial) | 5 | 30.7 | **53.4 ✕** | **54.8 ✕** | 46.3 | 49.6 |
| DM 16×16 | 4 | 9.0 | 29.8 | 31.3 | 22.5 | 25.9 |
| DM 20×20 | 4 | 11.0 | 31.9 | 33.4 | 24.6 | 28.1 |
| DM 22×22 | 4 | 12.0 | 33.0 | 34.5 | 25.7 | 29.1 |

### Assessment

- **QR**: At 4 dots, every placement (top, bottom, left, right) is within 50 mm. With a serial number on the left side, however, it reaches 48.1 mm, which is very tight. At 5 dots with more data, left and right placements fail.
- **DataMatrix**: 15–20 mm of margin in every placement. No issue.
- **The placement with the most margin** is above the JAN (the side without the HRI).
- Enlarging the JAN to 4 dots (0.50 mm, 152%) increases the centre-to-right-edge distance to 27.3 mm, and a QR V6 (4 dots) on the right then reaches 51.8 mm, which exceeds the limit. **The JAN must stay at 3 dots.**

## 6. Label Footprint (203 dpi, 4 dots)

| Combination | Side by side (W × H) | Stacked (W × H) |
|---|---|---|
| JAN + QR V4 | 62.9 × 29.5 mm | 42.4 × 50.0 mm |
| JAN + DM 20×20 | 53.4 × 29.5 mm | 42.4 × 40.5 mm |

→ **Side-by-side placement does not fit on a 50 mm-wide label.** With QR, a stacked layout still needs 50 mm of height. **Label area, not the 50 mm rule, is the real bottleneck.**

## 6-2. Placement Study for 203 dpi + DataMatrix (Japanese Market)

Since 203 dpi printers dominate the Japanese market, the realistic combination is expected to be **203 dpi + GS1 DataMatrix**.

### Side by side: does not fit on a 50 mm-wide label

At 203 dpi the JAN is fixed at 3 dots (113.7%), and the JAN alone is 42.4 mm wide including quiet zones.

| Side by side | Required width | 50 mm-wide label |
|---|---|---|
| JAN + DM 16×16 (GTIN only) | 51.4 mm | ✕ |
| JAN + DM 20×20 (+ lot, expiry) | 53.4 mm | ✕ |
| JAN + DM 22×22 (+ serial) | 54.4 mm | ✕ |

→ Even with GTIN only, the width exceeds 50 mm. **It physically does not fit.**

### Stacked: meets both the 50 mm rule and a 50 mm-wide label

| Stacked (DM placed above the JAN) | DM size | Code area (W × H) | With margins (approx., 1.5 mm all sides) | 50 mm rule (farthest corner) |
|---|---|---|---|---|
| JAN + DM 20×20 (+ lot, expiry) | 11.0 × 11.0 mm | 42.4 × 40.5 mm | approx. 45 × 44 mm | 24.6 mm (above) / 28.1 mm (below) |
| JAN + DM 22×22 (+ serial) | 12.0 × 12.0 mm | 42.4 × 41.5 mm | approx. 45 × 45 mm | 25.7 mm (above) / 29.1 mm (below) |
| JAN + rectangular DM 12×36 (+ lot, expiry) | approx. 7 × 19 mm | 42.4 × 36.5 mm | approx. 45 × 40 mm | Ample margin |
| JAN + rectangular DM 16×36 (+ serial) | approx. 9 × 19 mm | 42.4 × 38.5 mm | approx. 45 × 42 mm | Ample margin |

Note: DM at 4 dots (X = 0.500 mm), including the 1X quiet zone. Rectangular DM capacity: 12×36 holds 22 codewords; 16×36 holds 32 codewords.

### Assessment by label size

| Label size | Square DM (20×20 / 22×22) | Rectangular DM (12×36) |
|---|---|---|
| 50 × 40 mm | ✕ (insufficient height) | △ (barely fits; no room for other content) |
| 50 × 50 mm | ○ (little space left for other content) | ○ |
| 50 × 60 mm or larger | ○ | ○ |

### Notes

- **Space for other content**: In a stacked layout, the codes alone take about 40 mm of height. Almost no room remains for the product name, price, expiry date text, etc. In practice, this is the biggest constraint.
- **Placement**: Placing the DM **above the JAN is recommended**. Placing it below puts it under the JAN's HRI, which increases both the distance and the label height.
- **Gap between symbols**: Make sure the quiet zones do not overlap. The specification defines no top/bottom quiet zone for the JAN, so keep at least the DM's 1X = 0.5 mm.
- **Rectangular DM**: **Needs confirmation** whether rectangular formats are permitted for retail POS use and whether POS scanners support them.

### Conclusion

- A stacked layout satisfies both the 50 mm rule and a 50 mm-wide label.
- On labels of 50 × 40 mm or smaller, the codes alone fill almost the entire label. The practical deciding factor is either **moving up one label size** or **whether rectangular DM can be used**.
- The impossibility of side-by-side placement is a **structural problem of 203 dpi**: the JAN is fixed at 3 dots and takes up 42 mm of width. This can be clearly presented as an industry issue.

## 7. Comparison with 300 dpi

### QR V4 (GTIN + lot + expiry)

| | 203 dpi, 4 dots | 300 dpi, 5 dots | 300 dpi, 6 dots |
|---|---|---|---|
| X | 0.500 mm | 0.423 mm | 0.508 mm |
| QR side length | 20.5 mm | 17.4 mm | 20.8 mm |
| 50 mm rule (strictest: left placement) | 43.7 mm | 38.2 mm | 42.0 mm |
| Stacked footprint | 42.4 × 50.0 mm | 38.3 × 44.0 mm | 38.3 × 47.4 mm |
| Side-by-side footprint | 62.9 × 29.5 mm | 55.6 × 26.6 mm | 59.1 × 26.6 mm |

### Other combinations at 300 dpi

| 2D barcode | Dots | Side | 50 mm rule (left) | Side by side | Stacked |
|---|---|---|---|---|---|
| QR V6 | 5 | 20.7 | 41.9 | 59.0 × 26.6 | 38.3 × 47.4 |
| QR V6 | 6 | 24.9 | 46.4 | 63.2 × 26.6 | 38.3 × 51.5 |
| DM 20×20 | 5 | 9.3 | 29.5 | 47.6 × 26.6 | 38.3 × 35.9 |
| DM 22×22 | 6 | 12.2 | 32.6 | 50.5 × 26.6 | 38.3 × 38.8 |

### Assessment

- At 300 dpi the compliance problem disappears, and the target size (6 dots) can be selected. The JAN can also be printed close to 100%.
- Label area remains the issue. Side-by-side QR still needs a width of 55 mm or more even at 300 dpi. With DataMatrix, side-by-side width can be kept to about 48 mm.

## 8. Read Performance (GS1 Japan verification, October 2025)

| X-dimension | Print quality ≥ 1.5 | POS scanner | iPhone | Android |
|---|---|---|---|---|
| 0.25 mm (out of spec) | 11% | 20.7% | 98.1% | 45.6% |
| 0.396 mm (minimum) | 98.6% | 69.9% | 100% | 92.5% |
| 0.495 mm (target) | 100% | 80.8% | 100% | 100% |
| 0.990 mm (maximum) | 100% | 82.6% | 100% | 100% |

- Printers tested were 300 / 305 / 360 dpi; **203 dpi was not tested**.
- Only one POS scanner model (IS-921) and two smartphone models were tested.
- Smartphones can read small symbols, but **the bottleneck is POS scanner performance and print quality**.
- 3 dots (0.375 mm) is almost the same as 0.396 mm, so if print quality grade 1.5 or higher can be achieved at 203 dpi, similar read rates can be expected (needs actual measurement).

## 9. Summary of Issues

1. **203 dpi minimum-size problem**: 3 dots = 0.375 mm falls just short of the 0.396 mm minimum. This forces 4 dots, which makes the symbol larger.
2. **Gap in verification coverage**: GS1 Japan's verification covered only 300 dpi and above, not the 203 dpi printers common in Japan.
3. **Unclear measurement of the 50 mm rule**: It is not specified whether the whole symbol or its centre is measured, whether Euclidean or axial distance is used, or whether the HRI is included.
4. **Increased area during dual marking**: The combined area of the JAN and the 2D barcode does not fit on small labels.
5. **Treatment of in-store labels**: It is unclear how Sunrise 2027 applies to labels using in-store codes (prefix 02, 20–29) for fresh produce and prepared foods.

## 10. Open Issues for the Industry (Points Needing Clarification)

To move Sunrise 2027 forward in Japanese practice, the following points need clarification as practical issues on the ground.

1. **Minimum X at 203 dpi**: Whether there is room to permit 0.375 mm (3 dots), or treat it as an exception.
2. **Verification on 203 dpi printers**: Print and read verification using the 203 dpi thermal printers that dominate the Japanese market.
3. **How the 50 mm rule is measured**: Reference point, definition of distance, and treatment of the HRI.
4. **Symbol choice for small labels**: Whether GS1 DataMatrix (element strings) can be recommended.
5. **In-store labels**: Whether labels using in-store codes are within the scope of Sunrise 2027.
6. **Rectangular DataMatrix**: Whether rectangular formats (12×36, 16×36, etc.) may be used for retail POS.
7. **Placement guidance for small labels**: At 203 dpi, side-by-side placement does not fit on a 50 mm-wide label, so guidance such as a recommended stacked layout is needed.

## 11. Conclusion

- **203 dpi**: 4 dots is the effective minimum. The 50 mm rule can be met, but QR effectively requires a label of 50 × 50 mm or larger.
- **203 dpi + DataMatrix (the realistic option for Japan)**: Side-by-side placement on a 50 mm-wide label is impossible. A stacked layout works, but on labels of 50 × 40 mm or smaller the codes alone fill almost the entire label. Whether rectangular DM can be used is the key.
- **300 dpi**: The compliance problem is resolved, but side-by-side QR on small labels is still difficult.
- **Practical options**: For small labels where consumer smartphone use is not needed, use GS1 DataMatrix. If QR is required, revise the label size on the assumption of a stacked layout and keep the URL and data elements to a minimum.
- Actual measurements of print quality grade and POS read rates with 3-dot printing would provide strong evidence for raising these issues.

## 12. What BarBridge Can Do

BarBridge is a library for generating 1D/2D barcode pairs for GS1 Sunrise 2027. It helps in software with exactly the balance studied in this document: meeting the specification while still fitting on the label.

- **GS1-compliant 1D/2D pair generation**: Generates JAN (EAN-13) and similar barcodes together with GS1 QR Code / GS1 DataMatrix.
- **Automatic maximum compliant size**: Computes the largest GS1-compliant print size that still fits your target footprint.
- **Output formats**: In-memory raster, PNG, and SVG.
- **Multi-language support**: A Rust core usable through a C ABI, with samples for Python, C/C++, C#, VB.NET, Java, PHP, and Node.js. Supports Windows, macOS, and Linux (x64 and ARM64).
- **License**: The native library and sample code are free to use under MIT OR Apache-2.0. Paid contracts for source code access and dedicated support are also available.

Details and contact: <https://www.barbridge.org/>

---

## Sources

- [2D Barcodes at Retail Point-of-Sale Implementation Guideline 1.1 (GS1)](https://ref.gs1.org/guidelines/2d-in-retail/1.1.0/GS1-2DRetailPOS-Guideline-i1.1-r-2025-12-15)
- [GS1 2D Symbol Printing and Reading Verification Project Report, October 2025 (GS1 Japan, in Japanese)](https://www.gs1jp.org/standard/industry/2d-in-retail/GS1-2D-Symbol-Printing-and-Reading-Verification-Project.pdf)
- [How big should a QR code powered by GS1 be (GS1 UK)](https://www.gs1uk.org/knowledge-hub/qr-codes-powered-by-gs1/how-big-should-a-qr-code-powered-by-gs1-be)
- [Global industry endorsement statement: QR codes with GS1 standards (GS1)](https://gs1.org/sites/gs1/files/2024-06/global-industry-endorsement-statement-qr-codes-with-gs1-standards.pdf)

Note: Symbol versions and sizes are estimates calculated from sample data. Actual results will vary with the data content and printing conditions.
