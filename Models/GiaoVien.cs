using System;
using System.Collections.Generic;

namespace Ttlaixe.Models;

public partial class GiaoVien
{
    /// <summary>
    /// Mã Giáo viên = &lt;MaCSDT&gt;&lt;Số tự tăng có giá trị từ 001-999&gt;
    /// </summary>
    public string MaGv { get; set; }

    public string MaSoGtvt { get; set; }

    /// <summary>
    /// Cơ quan quản lý Giáo viên
    /// </summary>
    public string MaCsdt { get; set; }

    /// <summary>
    /// Họ và tên đệm của Giáo viên. 
    /// </summary>
    public string HoTenDem { get; set; }

    /// <summary>
    /// Tên của Giáo viên
    /// </summary>
    public string TenGv { get; set; }

    /// <summary>
    /// Ngày sinh của Giáo viên. Ghi theo định dạng: YYYYMMDD
    /// </summary>
    public string NgaySinh { get; set; }

    /// <summary>
    /// Đường dẫn tới ảnh chân dung của Giáo viên
    /// </summary>
    public string AnhCd { get; set; }

    /// <summary>
    /// Số Chứng minh thư của Giáo viên.
    /// </summary>
    public string SoCmt { get; set; }

    public string NoiCt { get; set; }

    public string NoiCtMaDvhc { get; set; }

    public string NoiCtMaDvql { get; set; }

    /// <summary>
    /// Giới tính của Giáo viên. U=Không xác định; M=Nam; F=Nữ
    /// </summary>
    public string GioiTinh { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string DienThoai { get; set; }

    /// <summary>
    /// Hình thức tuyển dụng. CT=Chính thức; HD=Hợp đồng
    /// </summary>
    public string HinhThucTuyenDung { get; set; }

    /// <summary>
    /// Trình độ văn hóa
    /// </summary>
    public string TrinhDoVanHoa { get; set; }

    /// <summary>
    /// Trình độ chuyên môn
    /// </summary>
    public string TrinhDoChuyenMon { get; set; }

    /// <summary>
    /// Trình độ sư phạm
    /// </summary>
    public string TrinhDoSuPham { get; set; }

    /// <summary>
    /// Hạng GPLX của Giáo viên. Được lưu theo định dạng: A1|A2|A3|...
    /// </summary>
    public string HangGplx { get; set; }

    /// <summary>
    /// Ngày cấp GPLX của Giáo viên. 
    /// </summary>
    public DateTime NgayCapGplx { get; set; }

    /// <summary>
    /// Thâm niên lái xe. Tính theo năm
    /// </summary>
    public int? ThamNienLaiXe { get; set; }

    /// <summary>
    /// Số quyết định cấp giấy chứng nhận
    /// </summary>
    public string SoQdGcn { get; set; }

    /// <summary>
    /// Ngày quyết định cấp giấy chứng nhận
    /// </summary>
    public DateTime? NgayQdGcn { get; set; }

    /// <summary>
    /// Loại hình đào tạo. LT=Lý thuyết; TH=Thực hànhl;LH = Cả hai
    /// </summary>
    public string LoaiHinhDaoTao { get; set; }

    public string GhiChu { get; set; }

    /// <summary>
    /// 0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;
    /// </summary>
    public bool? TrangThai { get; set; }

    public string NguoiTao { get; set; }

    public string NguoiSua { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public string CacHangGplxduocDt { get; set; }

    public string CauTaoSuaChua { get; set; }

    public string DaoDucLaixe { get; set; }

    public string NghiepVuVanTai { get; set; }

    public string LuatGtdb { get; set; }

    public string KyThuatLaixe { get; set; }

    public virtual DmHangGplx HangGplxNavigation { get; set; }

    public virtual DmDvhc NoiCtMaDv { get; set; }
}
