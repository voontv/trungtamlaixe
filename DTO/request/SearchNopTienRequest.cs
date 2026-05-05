using System;

namespace Ttlaixe.DTO.request
{
    public class SearchNopTienRequest
    {
        public string? MaDk { get; set; }
        public DateTime? FromNgayNop { get; set; }
        public DateTime? ToNgayNop { get; set; }
        public string? HinhThucThanhToan { get; set; }

        // search chéo qua hồ sơ
        public string? HoVaTen { get; set; }
        public string? NgaySinh { get; set; }
        public string? SoCmt { get; set; }
    }
}
