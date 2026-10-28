# Driver endpoints are protected by an IP allowlist, not by JWT

> **Superseded, temporarily.** The IP allowlist (`DriverApiIpFilter`, `DriverApi:AllowedIps`) was removed because it rejected
> every call behind Cloudflare Tunnel, and its `X-Real-IP` check could be forged there. The endpoints are open until a
> replacement exists (for example Cloudflare Access with a service token, or an allowlist built on `CF-Connecting-IP`).
> The text below describes the removed design.

The NestJS service had no credential and left access to the network. Here the three endpoints are `[AllowAnonymous]` (no JWT,
the partner has no account) but gated by `DriverApiIpFilter` against `DriverApi:AllowedIps`, because `verify` and `profiles`
return identity numbers and photos.

- It is a new filter and a new key. The existing `IpLaiXe` filter is not used: it is registered with the key
  `AdminSafeList:Vnpt`, which does not exist in `appsettings.json` (the key there is `IpLaiXe`), so using it would throw, and
  the key belongs to another feature.
- Exact IPs only, separated by `;`, the same format as `AdminSafeList`. No CIDR ranges until someone needs them.
- Fails closed: an empty or missing list rejects everyone, so a deployment that forgot the setting is closed, not open.
- The IP is `X-Real-IP` when present, else the remote address. That is only as trustworthy as the proxy that sets the header;
  it must overwrite client-supplied values.

The filter runs before validation, so a rejected caller learns nothing about the input rules.
