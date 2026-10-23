# Driver API is a contract-for-contract port of the NestJS sonhung service

`/api/v1/drivers`, `/verify` and `/profiles` already serve the exam site from the NestJS `sonhung` service. They are added here
so the same behaviour lives next to the rest of the Trung tam lai xe code, and the partner must not notice the move. So the
routes, parameters, validation rules, SQL and response fields are kept as they are, even where they differ from this project's
habits:

- JSON is camelCase through per-DTO `[JsonPropertyName]`, although `Startup` sets snake_case globally. Changing the global
  policy would break every existing client, so only these DTOs opt out.
- Input is validated by hand (`DriverRequestValidator`) instead of data annotations, because the contract rejects unknown
  properties, repeated query keys and empty values, which model binding silently accepts or turns into `null`. Errors use the
  NestJS body shape (`statusCode`, `message[]`, `error`).
- Data access is explicit parameterized SQL through Dapper, not EF: `DM_TrangThai` is not in the scaffolded model, and the
  SQL stays identical to the original so the two can be compared row for row. The connection comes from `IGplxTenantProvider`,
  so `x-loai-xe: moto` selects CSDT_MOTO like every other endpoint.
- Photos are decoded with Magick.NET (already a dependency, handles JPEG 2000) instead of sharp + openjpeg. Bytes differ,
  the 240 x 330 crop (fill the box, keep the top) and quality 82 do not. The stored UNC path is opened directly
  (Windows host with access to the share); the NestJS service needed a path map because it ran in Linux containers. The
  path lookup sits behind `IDriverPhotoPathResolver` so a non-Windows environment can supply its own.

Why it is worth remembering: the next person will be tempted to "fix" the camelCase or move validation to annotations. Do that
only together with the partner.
