using Ttlaixe.DTO.request;

namespace Ttlaixe.DTO.response
{
    public class NguoiLxResponse : NguoiLxCreateRequest
    {
        public string MaDk { get; set; }

        /// <summary>Loại hồ sơ (DM_LoaiHS), ví dụ: 3 = đào tạo mới</summary>
        public int MaLoaiHs { get; set; }

    }
}
