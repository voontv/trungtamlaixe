using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ttlaixe.DTO.response
{
    public class DanhSachBc1
    {
        public string MaDvGui { get; set; }
        public string MaBci { get; set; }
        public List<string> MaDks { get; set; } = new();
    }
    [XmlRoot("BAO_CAO1")]
    public class BaoCao1
    {
        [XmlElement("HEADER")]
        public BaoCao1Header Header { get; set; }
        [XmlElement("DATA")]
        public BaoCao1Data Data { get; set; }
    }

    public class BaoCao1Header
    {
        [XmlElement("MA_DV_GUI")]
        public string MaDvGui { get; set; }
    }

    public class BaoCao1Data
    {
        [XmlElement("KHOA_HOC")]
        public BaoCao1KhoaHoc KhoaHoc { get; set; }

        [XmlElement("NGUOI_LXS")]
        public BaoCao1NguoiLxs NguoiLxs { get; set; }
    }
    public class BaoCao1KhoaHoc
    {
        [XmlElement("MA_BCI")]
        public string MaBci { get; set; }
    }

    public class BaoCao1NguoiLxs
    {
        [XmlElement("NGUOI_LX")]
        public List<BaoCao1NguoiLx> NguoiLx { get; set; } = new();
    }

    public class BaoCao1NguoiLx
    {
        [XmlElement("MA_DK")]
        public string MaDk { get; set; }
    }

}
