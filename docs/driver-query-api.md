# Driver query API (`/api/v1/drivers`)

Three read-only endpoints for partner systems (the exam site), ported from the NestJS `sonhung` service
(`teknovavn/lai-xe-driver`) with the same contract and the same SQL. Code lives in `DriverApi/` and
`Controllers/DriversController.cs`; tests in `Ttlaixe.Tests/`.

Terms are defined in [GLOSSARY.md](../GLOSSARY.md). Decisions: [ADR 0001](adr/0001-driver-api-ported-from-sonhung.md),
[ADR 0002](adr/0002-driver-api-ip-allowlist.md).

## Access

There is no login. Only the IPs in `DriverApi:AllowedIps` (separated by `;`, exact match) may call these endpoints; anyone
else gets an empty `403`. `appsettings.json` ships with `72.62.73.239`. **An empty or missing list rejects every call**, so
overriding it with an empty value closes the API.

```json
"DriverApi": { "AllowedIps": "72.62.73.239;203.0.113.11" }
```

Setting `DriverApi__AllowedIps` replaces the whole list, it does not add to it.

Environment variable form: `DriverApi__AllowedIps`. The client IP is the `X-Real-IP` header, else the connection's remote
address. The reverse proxy in front must **set** `X-Real-IP` (overwriting what the client sent); if the app is reachable
without that proxy, a caller can forge the header and the allowlist protects nothing.

`verify` and `profiles` return identity numbers and photos, so keep the network path to this API restricted as well.

## Database

Like the rest of this API the database follows the `x-loai-xe` header: `moto` uses `Gplx_XeMay` (CSDT_MOTO), anything else
`Gplx_Oto` (CSDT_OTO). The SQL is the NestJS service's, run with Dapper on that connection (`DM_TrangThai` is not in the EF
model). Nothing is written.

## Endpoints

All JSON is camelCase, unlike the snake_case of the other controllers (DTOs set the names with `[JsonPropertyName]`).

### `GET /api/v1/drivers`

Each result is one `NguoiLX` row joined with its `NguoiLX_HoSo` row by `MaDK` and the matching `DM_TrangThai`. Only rows where
all three `TrangThai = 1` are returned, sorted by `MaDK`. All filters are optional, exact match, and combine with AND:

| Parameter     | Field                     | Rule                                                        |
| ------------- | ------------------------- | ----------------------------------------------------------- |
| `soHoSo`      | `NguoiLX_HoSo.SoHoSo`     | non-empty, max 18                                           |
| `ttXuLy`      | `NguoiLX_HoSo.TT_XuLy`    | non-empty, max 2                                            |
| `maDK`        | `NguoiLX.MaDK`            | non-empty, max 25                                           |
| `hoVaTen`     | `NguoiLX.HoVaTen`         | non-empty, max 50                                           |
| `ngaySinh`    | `NguoiLX.NgaySinh`        | `yyyyMMdd`                                                  |
| `hangGPLX`    | `NguoiLX_HoSo.HangGPLX`   | one code of max 3 characters, or up to 10 separated by `,`  |
| `hangDaoTao`  | `NguoiLX_HoSo.HangDaoTao` | non-empty, max 20                                           |
| `tenTT`       | `DM_TrangThai.TenTT`      | non-empty, max 50                                           |
| `q`           | name or `MaDK`            | max 50; substring, ignores case and diacritics on the name  |
| `page`        |                           | integer 1..21474837, default 1                              |
| `limit`       |                           | integer 1..100, default 10                                  |

`q` matches `%`, `_`, `[` literally. `hangGPLX=B1,B11` matches either code. The response is
`{ data, page, limit, total, totalPages }`; `ngayNhanHSo` is `yyyy-MM-dd`.

### `POST /api/v1/drivers/verify`

```json
{ "soCMT": "012345678901", "ngaySinh": "20021110" }
```

Returns `{ "data": [...] }`: active drivers whose identity number **and** birth date both match, newest HoSo first
(`ORDER BY HoSo.NgayTao DESC`, at most 20), with the search fields plus `ngayTao` (UTC, `2026-03-04T09:30:15.120Z`).
`soCMT` is compared in SQL and never returned. No match is `200` with an empty `data`. It is a POST so the identity number
stays out of URLs and access logs.

### `POST /api/v1/drivers/profiles`

```json
{ "maDKs": ["48012-20260304-095308"] }
```

1 to 50 unique, non-empty `MaDK` (max 25 characters each). Returns, per active driver, `maDK`, `ngaySinh` (`yyyyMMdd`), `soCMT`
and `anhBase64`: the photo of the active HoSo (`DuongDanAnh`) cropped and resized to **240 x 330 px** as a base64 JPEG
(quality 82), or `null` when there is no readable photo. A driver without an active HoSo is still returned, with `null` photo.
The response carries `Cache-Control: no-store`. The caller prints an exam report from it and must not store or log it.

The stored path (`DuongDanAnh`) is a UNC path of the image share, e.g.
`\\192.168.100.248\B\2026\im_gplx\48012K26B0100\48012-20260304-103333.jp2`. It is opened as it is, so the API must
run on a Windows host whose account can read that share (as the rest of this project already assumes). Paths that are not
absolute, or that contain `..`, are refused and give no photo. A Linux or container deployment cannot open UNC paths and
returns `null` photos unless it supplies its own `IDriverPhotoPathResolver`. Decoding (JPEG 2000 included), orientation, the
crop (fill the box, keep the top) and the JPEG encoding use Magick.NET.

## Errors

- `400` for any invalid input: unknown query or body property, repeated query key, empty value, bad format, wrong type.
  Body: `{ "statusCode": 400, "message": ["..."], "error": "Bad Request" }`.
- `403` empty body, IP not allowed.
- `500` is produced by the project-wide exception filter and contains the exception message.

## Differences from the NestJS service

- Validation message wording differs; status codes match (checked on 26 GET and 13 POST cases against the same database).
- `500` bodies differ (see above).
- Photo bytes differ (Magick.NET instead of sharp); size, crop and quality are the same.
- The database can be switched with `x-loai-xe`; the NestJS service only reads CSDT_OTO.
- Access control: allowlist here, network rules there.

## Tests

```sh
dotnet test Ttlaixe.sln
```

`Ttlaixe.Tests` covers the validation rules, the SQL builders, photo path mapping, the IP filter and the timestamp format.
`Ttlaixe.csproj` excludes `Ttlaixe.Tests\**` from compilation, keep it when adding more test files.
