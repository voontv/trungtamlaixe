namespace Ttlaixe.DTO.response
{
    public class DmHangDaoTaoResponse
    {
        public string MaHangDt { get; set; }

        /// <summary>
        /// Tên hạng đào tạo
        /// </summary>
        public string TenHangDt { get; set; }

        /// <summary>
        /// Hạng GPLX
        /// </summary>
        public string HangGplx { get; set; }

        /// <summary>
        /// Số VBPL quy định
        /// </summary>
        public string SoVbpl { get; set; }

        /// <summary>
        /// Tuổi nhỏ nhất của học viên được phép đào tạo
        /// </summary>
        public int? TuoiHv { get; set; }

        /// <summary>
        /// Thâm niên nâng hạng lái xe
        /// </summary>
        public int? ThamNien { get; set; }
    }
}
