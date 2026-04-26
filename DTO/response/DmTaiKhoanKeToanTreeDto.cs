using System.Collections.Generic;

namespace Ttlaixe.DTO.response
{
    public class DmTaiKhoanKeToanTreeDto
    {
        public string MaTaiKhoan { get; set; } = string.Empty;

        public string TenTaiKhoan { get; set; } = string.Empty;

        public string? MaTaiKhoanCha { get; set; }

        public int Cap { get; set; }

        public string? MaLoaiTaiKhoan { get; set; }

        public int? SoThuTu { get; set; }

        public List<DmTaiKhoanKeToanTreeDto> Children { get; set; } = new();
    }
}
