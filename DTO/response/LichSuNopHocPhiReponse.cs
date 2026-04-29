using System;
using Ttlaixe.DTO.request;

namespace Ttlaixe.DTO.response
{
    public class LichSuNopHocPhiReponse : LichSuNopHocPhiRequest
    {
        public int IdNopTien { get; set; }
        public DateTime NgayKhoiTao { get; set; }
    }
}
