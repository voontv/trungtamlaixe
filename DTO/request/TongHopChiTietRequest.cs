using System;
using System.Collections.Generic;

namespace Ttlaixe.DTO.request
{
    public class TongHopChiTietRequest
    {
        public List<string> MaTaiKhoans { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }
}
