using System.Collections.Generic;

namespace Ttlaixe.DTO.response
{
    public class TongHopThangReponse
    {
        public List<TongHopChungTuDto> SoDuDauKy { get; set; }
        public List<TongHopChungTuDto> SoPhatSinhTrongKy { get; set; }
        public List<TongHopChungTuDto> SoDuCuoiKy { get; set; }
    }
}
