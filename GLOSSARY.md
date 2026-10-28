# Glossary

Terms of the driver registration data (database `CSDT_OTO`, and `CSDT_MOTO` for motorcycles) and of the driver query API.

**NguoiLX**:
The registered person's driver information, identified by a registration code. Table `NguoiLX`, class `NguoiLx`.
_Avoid_: Driver record when referring only to this table's person data.

**MaDK**:
The registration code that identifies a NguoiLX and its corresponding HoSo.
_Avoid_: HoSo number; it is different from SoHoSo.

**HoSo**:
A driver's application record associated with a NguoiLX by MaDK. One MaDK identifies at most one HoSo. Table `NguoiLX_HoSo`, class `NguoiLxHoSo`.
_Avoid_: Registration code.

**SoHoSo**:
The number assigned to a HoSo.
_Avoid_: MaDK.

**SoCMT**:
The identity number of a NguoiLX. It is a credential: it may be compared in a query, and is returned only to a caller that must print it and forget it.
_Avoid_: Driver number, license number.

**TT_XuLy**:
The processing-status code on a HoSo, whose display name is TenTT in DM_TrangThai.
_Avoid_: TenTT when referring to the code.

**TenTT**:
The human-readable name for a processing status.
_Avoid_: TT_XuLy when referring to the display name.

**HangGPLX**:
The driver's license-class code recorded on a HoSo.
_Avoid_: License-class name.

**HangDaoTao**:
The training-class code recorded on a HoSo.
_Avoid_: HangDT, which is a separate field on KhoaHoc.

**Active record**:
A NguoiLX or HoSo whose TrangThai value is 1. A processing status is active when its DM_TrangThai TrangThai value is 1.
_Avoid_: Current, unless referring to a time-based version.

**Portrait**:
The photo of a driver stored on the file share and referenced by the HoSo's DuongDanAnh. The API serves it resized to 240 x 330 px.
_Avoid_: Avatar, thumbnail.

**Tenant database**:
The CSDT database a request reads, chosen by the `x-loai-xe` header: `moto` is CSDT_MOTO, anything else CSDT_OTO.
_Avoid_: Environment.
