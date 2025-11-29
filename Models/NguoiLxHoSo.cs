using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

/// <summary>
/// Lưu thông tin chính về Biên bản kiểm tra hồ sơ lái xe
/// </summary>
public partial class NguoiLxHoSo
{
    /// <summary>
    /// Mã đăng ký = &lt;Mã VPĐK&gt;-&lt;YYYYMMDD&gt;-&lt;HH24MISSMS&gt;
    /// </summary>
    public string MaDk { get; set; }

    /// <summary>
    /// Số hồ sơ = &lt;HangGPLX&gt;&lt;Số tự tăng theo hạng từ 01-99&gt;
    /// </summary>
    public string SoHoSo { get; set; }

    /// <summary>
    /// Mã Cơ sở đào tạo lái xe. Ghi Mã CSDT trong bảng DM_DonViGTVT
    /// </summary>
    public string MaCsdt { get; set; }

    /// <summary>
    /// Mã Sở GTVT. Ghi mã Sở GTVT trong bảng DM_DonViGTVT
    /// </summary>
    public string MaSoGtvt { get; set; }

    /// <summary>
    /// Nơi nhận hồ sơ = Mã Văn phòng đăng ký trong bảng DM_DonViGTVT
    /// </summary>
    public string MaDvnhanHso { get; set; }

    /// <summary>
    /// Ngày nhận hồ sơ xin cấp, đổi GPLX
    /// </summary>
    public DateTime NgayNhanHso { get; set; }

    /// <summary>
    /// Cán bộ nhận hồ sơ. Ghi đầy đủ họ và tên cán bộ
    /// </summary>
    public string NguoiNhanHso { get; set; }

    /// <summary>
    /// Ngày hẹn trả kết quả đối với cấp, đổi.
    /// </summary>
    public DateTime? NgayHenTra { get; set; }

    /// <summary>
    /// Loại hồ sơ. Tham chiếu đến bảng DM_LoaiHS
    /// </summary>
    public int MaLoaiHs { get; set; }

    /// <summary>
    /// Trạng thái xử lý hồ sơ cấp, đổi GPLX. Ghi mã trạng thái xử lý trong bảng DM_TrangThai
    /// </summary>
    public string TtXuLy { get; set; }

    public string DuongDanAnh { get; set; }

    public int? ChatLuongAnh { get; set; }

    public DateTime? NgayThuNhanAnh { get; set; }

    public string NguoiThuNhanAnh { get; set; }

    /// <summary>
    /// Số GPLX hiện có của người lái xe.
    /// </summary>
    public string SoGplxdaCo { get; set; }

    /// <summary>
    /// Hạng GPLX hiện có của Người lái xe
    /// </summary>
    public string HangGplxdaCo { get; set; }

    /// <summary>
    /// Đơn vị cấp GPLX hiện có của Người lái xe
    /// </summary>
    public string DonViCapGplxdaCo { get; set; }

    public string NoiCapGplxdaCo { get; set; }

    /// <summary>
    /// Ngày cấp giấy phép lái xe hiện có của người lái xe
    /// </summary>
    public string NgayCapGplxdaCo { get; set; }

    /// <summary>
    /// Ngày hết hạn GPLX hiện có của NGười lái xe
    /// </summary>
    public string NgayHhgplxdaCo { get; set; }

    public string NgayTtgplxdaCo { get; set; }

    /// <summary>
    /// Nơi học lái xe trước đây. Ghi mã CSĐT trong bảng DM_DonViGTVT
    /// </summary>
    public string DonViHocLx { get; set; }

    /// <summary>
    /// Năm học lái xe trước đây. Ghi theo định dạng YYYY
    /// </summary>
    public int? NamHocLx { get; set; }

    /// <summary>
    /// Hạng GPLX đề nghị cấp. Tham chiếu đến bảng DM_HangGPLX
    /// </summary>
    public string HangGplx { get; set; }

    /// <summary>
    /// Số năm lái xe
    /// </summary>
    public int? SoNamLx { get; set; }

    /// <summary>
    /// Số Km lái xe an toàn
    /// </summary>
    public int? SoKmLxanToan { get; set; }

    /// <summary>
    /// Giấy chứng nhận sức khỏe. Ghi 0=Không hợp lệ; 1=Hợp lệ. Mặc định = 0.
    /// </summary>
    public bool? GiayCnsk { get; set; }

    /// <summary>
    /// Lý do cấp đổi, cấp lại GPLX
    /// </summary>
    public string LyDoCapDoi { get; set; }

    /// <summary>
    /// Mục đích cấp đổi, cấp lại GPLX
    /// </summary>
    public string MucDichCapDoi { get; set; }

    /// <summary>
    /// Nội dung sát hạch. Ghi mã trong bảng DM_NoiDungSH
    /// </summary>
    public int? NoiDungSh { get; set; }

    /// <summary>
    /// Mã khóa đào tạo, tham chiếu đến trường MaKH của bảng KhoaHoc
    /// </summary>
    public string MaKhoaHoc { get; set; }

    /// <summary>
    /// Hạng đào tạo lái xe. Ghi Mã hạng đào tạo trong bảng DM_HangDT
    /// </summary>
    public string HangDaoTao { get; set; }

    /// <summary>
    /// Số giấy chứng nhận tốt nghiệp
    /// </summary>
    public string SoGiayCntn { get; set; }

    /// <summary>
    /// Số chứng chỉ nghề
    /// </summary>
    public string SoCcn { get; set; }

    /// <summary>
    /// Mã Báo cáo 1. Ghi Mã Báo cáo 1 trong bảng dbo.BaoCaoI theo định dạng: &lt;Mã Khóa học&gt;&lt;BCI&gt;&lt;01-99&gt;
    /// </summary>
    public string MaBc1 { get; set; }

    /// <summary>
    /// Kiểm tra tuổi tuyển sinh của người lái xe. 0=Không đủ tuổi; 1=Đủ tuổi. Mặc định là 1
    /// </summary>
    public bool? Bc1TuoiTs { get; set; }

    /// <summary>
    /// Kiểm tra thâm niên lái xe đối với nâng hạng. 0=Không đủ thâm niên; 1=Đủ thâm niên. Mặc định = 1
    /// </summary>
    public bool? Bc1ThamNien { get; set; }

    /// <summary>
    /// Mã Báo cáo 2. Ghi Mã Báo cáo 2 trong bảng dbo.BaoCao2 theo định dạng: &lt;Mã CSĐT&gt;&lt;BCII&gt;&lt;YY&gt;&lt;01-99&gt;
    /// </summary>
    public string MaBc2 { get; set; }

    public bool? KetQuaBc2 { get; set; }

    /// <summary>
    /// Mã Lý do từ chối Báo cáo 2. Ghi mã Lý do từ chối báo cáo 2 trong bảng dbo.DM_LyDoTCBC2
    /// </summary>
    public int? MaLyDoTcbc2 { get; set; }

    /// <summary>
    /// Mã Kỳ sát hạch = &lt;Mã TTSH&gt;&lt;K&gt;&lt;YY&gt;&lt;01-99&gt;
    /// </summary>
    public string MaKySh { get; set; }

    /// <summary>
    /// Số báo danh của thí sinh dự sát hạch lái xe =&lt;001-999&gt;
    /// </summary>
    public string SoBd { get; set; }

    /// <summary>
    /// Lần sát hạch thứ 1 hay thứ 2. Mặc định là 1
    /// </summary>
    public int? LanSh { get; set; }

    /// <summary>
    /// Số quyết định tổ chức kỳ sát hạch
    /// </summary>
    public string SoQdsh { get; set; }

    /// <summary>
    /// Ngày QĐ tổ chức kỳ sát hạch
    /// </summary>
    public DateTime? NgayQdsh { get; set; }

    /// <summary>
    /// Kết quả sát hạch lý thuyết
    /// </summary>
    public int? KetQuaLyThuyet { get; set; }

    /// <summary>
    /// Nhận xét của Sát hạch viên Lý thuyết
    /// </summary>
    public string NhanXetLyThuyet { get; set; }

    /// <summary>
    /// Kết quả sát hạch Hình
    /// </summary>
    public int? KetQuaHinh { get; set; }

    /// <summary>
    /// Nhận xét của Sát hạch viên thi Hình
    /// </summary>
    public string NhanXetHinh { get; set; }

    /// <summary>
    /// Kết quả sát hạch Đường trường
    /// </summary>
    public int? KetQuaDuong { get; set; }

    /// <summary>
    /// Nhận xét của Sát hạch viên Đường trường
    /// </summary>
    public string NhanXetDuong { get; set; }

    /// <summary>
    /// Kết quả kỳ sát hạch. DA=Đạt; RO=Rớt; VA=Vắng; KH=Không dự hết sát hạch
    /// </summary>
    public string KetQuaSh { get; set; }

    /// <summary>
    /// Số QĐ công nhận trúng tuyển
    /// </summary>
    public string SoQdtt { get; set; }

    /// <summary>
    /// Ngày QĐ công nhận trúng tuyển sát hạch
    /// </summary>
    public DateTime? NgayQdtt { get; set; }

    /// <summary>
    /// Người ký quyết định cấp GPLX.
    /// </summary>
    public string NguoiKy { get; set; }

    public string GhiChu { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public string SoGplxtmp { get; set; }

    public DateTime? NgayKtbc1 { get; set; }

    public string NguoiKtbc1 { get; set; }

    public DateTime? NgayKtbc2 { get; set; }

    public string NguoiKtbc2 { get; set; }

    public string MaIn { get; set; }

    /// <summary>
    /// Kết quả đối sánh với TW. 0=Hợp lệ; 1=Không hợp lệ
    /// </summary>
    public bool? KetQuaDoiSanhTw { get; set; }

    public string GhiChuKqdstw { get; set; }

    public string ChuKy { get; set; }

    public bool? TrangThai { get; set; }

    /// <summary>
    /// Mã hình thức cấp. Ghi mã Hình thức cấp GPLX trong bảng DM_HTCapGPLX
    /// </summary>
    public string MaHtcap { get; set; }

    public long Ids { get; set; }

    public string TtXuLyOld { get; set; }

    public bool? KqBc1 { get; set; }

    public string KqBc1GhiChu { get; set; }

    public int TransferFlag { get; set; }

    public string VaoSoCnnso { get; set; }

    public DateTime? NgayVaoSoCnn { get; set; }

    public string XepLoaiTotNghiep { get; set; }

    public DateTime? NgayCapCcn { get; set; }

    public string SoQuyetDinhTn { get; set; }

    public DateTime? NgayRaQdtn { get; set; }

    public string SoSoTn { get; set; }

    public DateTime? NgayVaoSoTn { get; set; }

    public DateTime? NgayInGiayTn { get; set; }

    public string NamcapLandau { get; set; }

    public string MaTrichNgang { get; set; }

    public string CoQuanQuanLyGplx { get; set; }

    public int? ChonInGplx { get; set; }

    public int? KetQuaShm { get; set; }

    public string NhanXetMoPhong { get; set; }

    public int? HosoDvcc4 { get; set; }

    public virtual DmHangDt HangDaoTaoNavigation { get; set; }

    public virtual DmHangGplx HangGplxNavigation { get; set; }

    public virtual NguoiLx MaDkNavigation { get; set; }

    public virtual DmHtcapGplx MaHtcapNavigation { get; set; }

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; }

    public virtual ICollection<NguoiLxhsGiayTo> NguoiLxhsGiayTos { get; set; } = new List<NguoiLxhsGiayTo>();
}
