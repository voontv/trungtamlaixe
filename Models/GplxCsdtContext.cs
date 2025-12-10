using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ttlaixe.Models;

public partial class GplxCsdtContext : DbContext
{
    public GplxCsdtContext()
    {
    }

    public GplxCsdtContext(DbContextOptions<GplxCsdtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DmDiemSatHach> DmDiemSatHaches { get; set; }

    public virtual DbSet<DmDvhc> DmDvhcs { get; set; }

    public virtual DbSet<DmGiayTo> DmGiayTos { get; set; }

    public virtual DbSet<DmHangDt> DmHangDts { get; set; }

    public virtual DbSet<DmHangDtMonHoc> DmHangDtMonHocs { get; set; }

    public virtual DbSet<DmHangGplx> DmHangGplxes { get; set; }

    public virtual DbSet<DmHtcapGplx> DmHtcapGplxes { get; set; }

    public virtual DbSet<DmLoaiHso> DmLoaiHsos { get; set; }

    public virtual DbSet<DmLoaiHsoGiayTo> DmLoaiHsoGiayTos { get; set; }

    public virtual DbSet<DmPhanThiTkn> DmPhanThiTkns { get; set; }

    public virtual DbSet<DmQuocTich> DmQuocTiches { get; set; }

    public virtual DbSet<DmthiSatHachQuyTacTkn> DmthiSatHachQuyTacTkns { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<NguoiLx> NguoiLxes { get; set; }

    public virtual DbSet<NguoiLxHoSo> NguoiLxHoSos { get; set; }

    public virtual DbSet<NguoiLxhsGiayTo> NguoiLxhsGiayTos { get; set; }

    public virtual DbSet<ThiSatHachKetQuaChiTietTkn> ThiSatHachKetQuaChiTietTkns { get; set; }

    public virtual DbSet<ThiSatHachKetQuaPhanThiTkn> ThiSatHachKetQuaPhanThiTkns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DmDiemSatHach>(entity =>
        {
            entity.HasKey(e => e.Stt);

            entity.ToTable("DM_DiemSatHach");

            entity.Property(e => e.Stt)
                .ValueGeneratedNever()
                .HasColumnName("STT");
            entity.Property(e => e.Hang)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DmDvhc>(entity =>
        {
            entity.HasKey(e => new { e.MaDvhc, e.MaDvql });

            entity.ToTable("DM_DVHC");

            entity.HasIndex(e => e.MaDv, "UK_DM_DVHC_MaDv");

            entity.Property(e => e.MaDvhc)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.MaDvql)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaDVQL");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.LoaiDvhc)
                .IsRequired()
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.MaDv)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaDV");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.RowVersion)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("ROW_VERSION");
            entity.Property(e => e.TenDayDu).HasMaxLength(100);
            entity.Property(e => e.TenDvhc)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TenNganGon)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<DmGiayTo>(entity =>
        {
            entity.HasKey(e => e.MaGt);

            entity.ToTable("DM_GiayTo");

            entity.Property(e => e.MaGt).HasColumnName("MaGT");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.SoVbpl)
                .HasMaxLength(50)
                .HasColumnName("SoVBPL");
            entity.Property(e => e.TenGt)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("TenGT");
            entity.Property(e => e.TenGten)
                .HasMaxLength(150)
                .HasColumnName("TenGTEN");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<DmHangDt>(entity =>
        {
            entity.HasKey(e => e.MaHangDt);

            entity.ToTable("DM_HangDT");

            entity.Property(e => e.MaHangDt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Mã hạng đào tạo. Được ghi: A1; A2; ...; B1-B2; B2-C")
                .HasColumnName("MaHangDT");
            entity.Property(e => e.Dksh)
                .HasMaxLength(255)
                .HasComment("Điều kiện sát hạch")
                .HasColumnName("DKSH");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.HangGplx)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("Hạng GPLX")
                .HasColumnName("HangGPLX");
            entity.Property(e => e.KmLxat)
                .HasComment("Số Km lái xe an toàn")
                .HasColumnName("KmLXAT");
            entity.Property(e => e.MucTieuDt)
                .HasMaxLength(500)
                .HasComment("Mục tiêu đào tạo")
                .HasColumnName("MucTieuDT");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.SoVbpl)
                .HasMaxLength(30)
                .HasComment("Số VBPL quy định")
                .HasColumnName("SoVBPL");
            entity.Property(e => e.TenHangDt)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Tên hạng đào tạo")
                .HasColumnName("TenHangDT");
            entity.Property(e => e.ThamNien).HasComment("Thâm niên nâng hạng lái xe");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.TuoiHv)
                .HasComment("Tuổi nhỏ nhất của học viên được phép đào tạo")
                .HasColumnName("TuoiHV");
        });

        modelBuilder.Entity<DmHangDtMonHoc>(entity =>
        {
            entity.HasKey(e => new { e.MaMh, e.MaHangDt });

            entity.ToTable("DM_HangDT_MonHoc");

            entity.Property(e => e.MaMh).HasColumnName("MaMH");
            entity.Property(e => e.MaHangDt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaHangDT");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.TenMh)
                .HasMaxLength(100)
                .HasColumnName("TenMH");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");

            entity.HasOne(d => d.MaHangDtNavigation).WithMany(p => p.DmHangDtMonHocs)
                .HasForeignKey(d => d.MaHangDt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DM_HangDT_MonHoc_DM_HangDT");
        });

        modelBuilder.Entity<DmHangGplx>(entity =>
        {
            entity.HasKey(e => e.MaHang);

            entity.ToTable("DM_HangGPLX");

            entity.Property(e => e.MaHang)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("Mã hạng GPLX");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.HanSuDung).HasComment("Hạn sử dụng GPLX tính theo năm. 0=Vô thời hạn");
            entity.Property(e => e.HangDuocLai)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MaHangMoi).HasMaxLength(5);
            entity.Property(e => e.MoTaEn)
                .HasMaxLength(500)
                .HasComment("Nội dung ghi ở mặt sau của GPLX bằng tiếng Anh")
                .HasColumnName("MoTaEN");
            entity.Property(e => e.MoTaEncu)
                .HasMaxLength(500)
                .HasColumnName("MoTaENCu");
            entity.Property(e => e.MoTaVn)
                .HasMaxLength(500)
                .HasComment("Nội dung ghi ở mặt sau GPLX bằng tiếng Việt")
                .HasColumnName("MoTaVN");
            entity.Property(e => e.MoTaVncu)
                .HasMaxLength(500)
                .HasColumnName("MoTaVNCu");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.TenHang)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Tên hạng GPLX");
            entity.Property(e => e.TenHangMoi).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasComment("1 = Hiệu lực; 2 = Không hiệu lực.");
        });

        modelBuilder.Entity<DmHtcapGplx>(entity =>
        {
            entity.HasKey(e => e.MaHtcap);

            entity.ToTable("DM_HTCapGPLX");

            entity.Property(e => e.MaHtcap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaHTCap");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.TenHtcap)
                .HasMaxLength(70)
                .HasColumnName("TenHTCap");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<DmLoaiHso>(entity =>
        {
            entity.HasKey(e => e.MaLoaiHs);

            entity.ToTable("DM_LoaiHSo");

            entity.Property(e => e.MaLoaiHs).ValueGeneratedNever();
            entity.Property(e => e.DieuKien)
                .HasMaxLength(150)
                .HasComment("Điều kiện được cấp GPLX");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaHtcap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaHTCap");
            entity.Property(e => e.MaNoiDungSh).HasColumnName("MaNoiDungSH");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.Nhom)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SoVbpl)
                .HasMaxLength(50)
                .HasComment("Văn bản pháp luật quy định điều kiện cấp")
                .HasColumnName("SoVBPL");
            entity.Property(e => e.TenLoaiHs)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.ThoiHanTraKq)
                .HasComment("Thời hạn trả kết quả. Tính bằng ngày làm việc.")
                .HasColumnName("ThoiHanTraKQ");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.YeuCauDtshlai)
                .HasMaxLength(50)
                .HasComment("Yêu cầu đào tạo và sát hạch lại")
                .HasColumnName("YeuCauDTSHLai");
        });

        modelBuilder.Entity<DmLoaiHsoGiayTo>(entity =>
        {
            entity.HasKey(e => new { e.MaGt, e.MaLoaiHs, e.MaHangGplx });

            entity.ToTable("DM_LoaiHSo_GiayTo");

            entity.Property(e => e.MaGt).HasColumnName("MaGT");
            entity.Property(e => e.MaLoaiHs).HasColumnName("MaLoaiHS");
            entity.Property(e => e.MaHangGplx)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("MaHangGPLX");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.TenGt)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("TenGT");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<DmPhanThiTkn>(entity =>
        {
            entity.HasKey(e => e.MaPhanThi);

            entity.ToTable("DmPhanThiTkn");

            entity.Property(e => e.HangDaoTao)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenPhanThi)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<DmQuocTich>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DM_QuocTich");

            entity.Property(e => e.Ma)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(300);
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.TenEn)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("TenEN");
            entity.Property(e => e.TenVn)
                .HasMaxLength(200)
                .HasColumnName("TenVN");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasComment("0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;");
        });

        modelBuilder.Entity<DmthiSatHachQuyTacTkn>(entity =>
        {
            entity.HasKey(e => e.IdQuyTac).HasName("PK__DMThiSat__9F0652C32766EBD3");

            entity.ToTable("DMThiSatHach_QuyTacTkn");

            entity.Property(e => e.NoiDung)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(d => d.MaPhanThiNavigation).WithMany(p => p.DmthiSatHachQuyTacTkns)
                .HasForeignKey(d => d.MaPhanThi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DMThiSatHach_QuyTacTkn_DmPhanThiTkn");
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaKh);

            entity.ToTable("KhoaHoc");

            entity.HasIndex(e => new { e.MaKh, e.MaCsdt, e.MaSoGtvt }, "UK_KhoaHoc").IsUnique();

            entity.Property(e => e.MaKh)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasComment("Mã khóa học = <MaSoGTVT><MaCSDT>-<'K-YY-(01-99)>")
                .HasColumnName("MaKH");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(255)
                .HasComment("Ghi chú");
            entity.Property(e => e.HangDt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Hạng đào tạo lái xe. Lưu theo định dạng: A1|A2|A3|...")
                .HasColumnName("HangDT");
            entity.Property(e => e.HangGplx)
                .IsRequired()
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("HangGPLX");
            entity.Property(e => e.MaCsdt)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("Mã Cơ sở đào tạo")
                .HasColumnName("MaCSDT");
            entity.Property(e => e.MaSoGtvt)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("Mã Sở GTVT")
                .HasColumnName("MaSoGTVT");
            entity.Property(e => e.MucTieuDt)
                .HasMaxLength(1000)
                .HasColumnName("MucTieuDT");
            entity.Property(e => e.NgayBg)
                .HasColumnType("datetime")
                .HasColumnName("NgayBG");
            entity.Property(e => e.NgayKg)
                .HasColumnType("datetime")
                .HasColumnName("NgayKG");
            entity.Property(e => e.NgayQdKhaiGiang)
                .HasComment("Ngày ra Quyết định khai giảng khóa học")
                .HasColumnType("datetime")
                .HasColumnName("NgayQD_KhaiGiang");
            entity.Property(e => e.NgaySh)
                .HasColumnType("datetime")
                .HasColumnName("NgaySH");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayThi)
                .HasComment("Ngày thi")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.SoHvduocCapGplx).HasColumnName("SoHVDuocCapGPLX");
            entity.Property(e => e.SoHvtotNghiep).HasColumnName("SoHVTotNghiep");
            entity.Property(e => e.SoNgayNghiLe).HasComment("Số ngày nghỉ lễ, khai, bế giảng");
            entity.Property(e => e.SoNgayOnKt)
                .HasComment("Số ngày ôn và kiểm tra kết thúc khóa học")
                .HasColumnName("SoNgayOnKT");
            entity.Property(e => e.SoNgayThucHoc).HasComment("Số ngày thực học");
            entity.Property(e => e.SoQdKhaiGiang)
                .HasMaxLength(20)
                .HasComment("Số Quyết định khai giảng khóa học")
                .HasColumnName("SoQD_KhaiGiang");
            entity.Property(e => e.TenKh)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Tên khóa học")
                .HasColumnName("TenKH");
            entity.Property(e => e.ThoiGianDt)
                .HasComment("Thời gian đào tạo, tính theo tháng.")
                .HasColumnName("ThoiGianDT");
            entity.Property(e => e.TongSoHv)
                .HasComment("Tổng số học viên của khóa học")
                .HasColumnName("TongSoHV");
            entity.Property(e => e.TongSoNgay).HasComment("Cộng số ngày/Khóa đào tạo");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasComment("0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;");
            entity.Property(e => e.TtXuly).HasColumnName("TT_Xuly");

            entity.HasOne(d => d.HangDtNavigation).WithMany(p => p.KhoaHocs)
                .HasForeignKey(d => d.HangDt)
                .HasConstraintName("FK_KhoaHoc_DM_HangDT");

            entity.HasOne(d => d.HangGplxNavigation).WithMany(p => p.KhoaHocs)
                .HasForeignKey(d => d.HangGplx)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhoaHoc_DM_HangGPLX");
        });

        modelBuilder.Entity<NguoiLx>(entity =>
        {
            entity.HasKey(e => e.MaDk);

            entity.ToTable("NguoiLX");

            entity.Property(e => e.MaDk)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasComment("Mã đăng ký = <Mã VPĐK>-<YYYYMMDD>-<HH24MISSMS>")
                .HasColumnName("MaDK");
            entity.Property(e => e.DonViNhanHso)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("DonViNhanHSo");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GioiTinh)
                .IsRequired()
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasComment("Giới tính. M=Nam; F=Nữ; U=Không xác định");
            entity.Property(e => e.HoDemNlx)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("Họ và tên đệm của Người lái xe")
                .HasColumnName("HoDemNLX");
            entity.Property(e => e.HoVaTen)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Họ và tên của Người lái xe");
            entity.Property(e => e.HoVaTenIn)
                .IsRequired()
                .HasMaxLength(25);
            entity.Property(e => e.HosoDvcc4).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaQuocTich)
                .IsRequired()
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("Mã quốc gia theo bảng mã quốc tế ISO 3166-1 alpha-3. Ghi mã Quốc gia trong bảng DM_QuocTich");
            entity.Property(e => e.NgayCapCmt)
                .HasComment("Ngày cấp CMT/Hộ chiếu")
                .HasColumnType("datetime")
                .HasColumnName("NgayCapCMT");
            entity.Property(e => e.NgaySinh)
                .IsRequired()
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasComment("Ngày sinh, được ghi theo định dạng: YYYYMMDD.");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.NoiCapCmt)
                .HasMaxLength(50)
                .HasComment("Nơi cấp CMT/Hộ chiếu")
                .HasColumnName("NoiCapCMT");
            entity.Property(e => e.NoiCt)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Nơi cư trú. Ghi chi tiết Số nhà/Đường phố/Thôn/Xóm")
                .HasColumnName("NoiCT");
            entity.Property(e => e.NoiCtMaDvhc)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("Mã Xã/Phường/Thị Trấn/Quận/Huyện/Thị xã/Thành phố. Ghi MaDvhc trong bảng DM_DVHC")
                .HasColumnName("NoiCT_MaDVHC");
            entity.Property(e => e.NoiCtMaDvql)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("Mã Quận/Huyện/Thị xã/Thành phố Nơi cư trú. Ghi MaDVQL trong bảng DM_DVHC")
                .HasColumnName("NoiCT_MaDVQL");
            entity.Property(e => e.NoiTt)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Nơi đăng ký hộ khẩu thường trú. Ghi chi tiết: Số nhà/Đường phố/Thôn xóm")
                .HasColumnName("NoiTT");
            entity.Property(e => e.NoiTtMaDvhc)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("Mã đơn vị Xã/Phường/Quận/Huyện của Nơi đăng ký thường trú. Ghi MaDVHC trong bảng DM_DVHC")
                .HasColumnName("NoiTT_MaDVHC");
            entity.Property(e => e.NoiTtMaDvql)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("Mã Huyện/Quận/Thành phố/Thị xã của Noi đăng ký hộ khẩu thường trú. Ghi MaDVQL trong bảng DM_DVHC")
                .HasColumnName("NoiTT_MaDVQL");
            entity.Property(e => e.SoCmndCu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SO_CMND_CU");
            entity.Property(e => e.SoCmt)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Số CMT/Hộ chiếu")
                .HasColumnName("SoCMT");
            entity.Property(e => e.TenNlx)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("Tên của người lái xe")
                .HasColumnName("TenNLX");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasComment("0 = khong hieu luc; 1 = co hieu luc; mac dinh la 1;");

            entity.HasOne(d => d.MaQuocTichNavigation).WithMany(p => p.NguoiLxes)
                .HasForeignKey(d => d.MaQuocTich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiLX_DM_QuocTich");

            entity.HasOne(d => d.NoiCtMaDv).WithMany(p => p.NguoiLxNoiCtMaDvs)
                .HasForeignKey(d => new { d.NoiCtMaDvhc, d.NoiCtMaDvql })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiLX_DM_DVHC");

            entity.HasOne(d => d.NoiTtMaDv).WithMany(p => p.NguoiLxNoiTtMaDvs)
                .HasForeignKey(d => new { d.NoiTtMaDvhc, d.NoiTtMaDvql })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiLX_DM_DVHC1");
        });

        modelBuilder.Entity<NguoiLxHoSo>(entity =>
        {
            entity.HasKey(e => e.MaDk).HasName("PK_NguoiLX_HoSo_1");

            entity.ToTable("NguoiLX_HoSo", tb => tb.HasComment("Lưu thông tin chính về Biên bản kiểm tra hồ sơ lái xe"));

            entity.HasIndex(e => e.MaDk, "UK_NguoiLX_HoSo_MaDK").IsUnique();

            entity.Property(e => e.MaDk)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasComment("Mã đăng ký = <Mã VPĐK>-<YYYYMMDD>-<HH24MISSMS>")
                .HasColumnName("MaDK");
            entity.Property(e => e.Bc1ThamNien)
                .HasDefaultValueSql("((1))")
                .HasComment("Kiểm tra thâm niên lái xe đối với nâng hạng. 0=Không đủ thâm niên; 1=Đủ thâm niên. Mặc định = 1")
                .HasColumnName("BC1_ThamNien");
            entity.Property(e => e.Bc1TuoiTs)
                .HasDefaultValueSql("((1))")
                .HasComment("Kiểm tra tuổi tuyển sinh của người lái xe. 0=Không đủ tuổi; 1=Đủ tuổi. Mặc định là 1")
                .HasColumnName("BC1_TuoiTS");
            entity.Property(e => e.ChonInGplx)
                .HasDefaultValueSql("((0))")
                .HasColumnName("CHON_IN_GPLX");
            entity.Property(e => e.ChuKy).HasMaxLength(255);
            entity.Property(e => e.CoQuanQuanLyGplx)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CoQuanQuanLyGPLX");
            entity.Property(e => e.DonViCapGplxdaCo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Đơn vị cấp GPLX hiện có của Người lái xe")
                .HasColumnName("DonViCapGPLXDaCo");
            entity.Property(e => e.DonViHocLx)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("Nơi học lái xe trước đây. Ghi mã CSĐT trong bảng DM_DonViGTVT")
                .HasColumnName("DonViHocLX");
            entity.Property(e => e.DuongDanAnh).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GhiChuKqdstw)
                .HasMaxLength(255)
                .HasColumnName("GhiChuKQDSTW");
            entity.Property(e => e.GiayCnsk)
                .HasDefaultValueSql("((0))")
                .HasComment("Giấy chứng nhận sức khỏe. Ghi 0=Không hợp lệ; 1=Hợp lệ. Mặc định = 0.")
                .HasColumnName("GiayCNSK");
            entity.Property(e => e.HangDaoTao)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("Hạng đào tạo lái xe. Ghi Mã hạng đào tạo trong bảng DM_HangDT");
            entity.Property(e => e.HangGplx)
                .IsRequired()
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("Hạng GPLX đề nghị cấp. Tham chiếu đến bảng DM_HangGPLX")
                .HasColumnName("HangGPLX");
            entity.Property(e => e.HangGplxdaCo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Hạng GPLX hiện có của Người lái xe")
                .HasColumnName("HangGPLXDaCo");
            entity.Property(e => e.HosoDvcc4).HasDefaultValueSql("((0))");
            entity.Property(e => e.Ids)
                .ValueGeneratedOnAdd()
                .HasColumnName("IDs");
            entity.Property(e => e.KetQuaBc2).HasColumnName("KetQuaBC2");
            entity.Property(e => e.KetQuaDoiSanhTw)
                .HasComment("Kết quả đối sánh với TW. 0=Hợp lệ; 1=Không hợp lệ")
                .HasColumnName("KetQuaDoiSanhTW");
            entity.Property(e => e.KetQuaDuong)
                .HasDefaultValueSql("((0))")
                .HasComment("Kết quả sát hạch Đường trường")
                .HasColumnName("KetQua_Duong");
            entity.Property(e => e.KetQuaHinh)
                .HasDefaultValueSql("((0))")
                .HasComment("Kết quả sát hạch Hình")
                .HasColumnName("KetQua_Hinh");
            entity.Property(e => e.KetQuaLyThuyet)
                .HasDefaultValueSql("((0))")
                .HasComment("Kết quả sát hạch lý thuyết")
                .HasColumnName("KetQua_LyThuyet");
            entity.Property(e => e.KetQuaSh)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("((1))")
                .HasComment("Kết quả kỳ sát hạch. DA=Đạt; RO=Rớt; VA=Vắng; KH=Không dự hết sát hạch")
                .HasColumnName("KetQuaSH");
            entity.Property(e => e.KetQuaShm)
                .HasDefaultValueSql("((0))")
                .HasColumnName("KetQuaSHM");
            entity.Property(e => e.KqBc1).HasColumnName("KQ_BC1");
            entity.Property(e => e.KqBc1GhiChu)
                .HasMaxLength(50)
                .HasColumnName("KQ_BC1_GhiChu");
            entity.Property(e => e.LanSh)
                .HasDefaultValueSql("((1))")
                .HasComment("Lần sát hạch thứ 1 hay thứ 2. Mặc định là 1")
                .HasColumnName("LanSH");
            entity.Property(e => e.LyDoCapDoi)
                .HasMaxLength(50)
                .HasComment("Lý do cấp đổi, cấp lại GPLX");
            entity.Property(e => e.MaBc1)
                .HasMaxLength(18)
                .IsUnicode(false)
                .HasComment("Mã Báo cáo 1. Ghi Mã Báo cáo 1 trong bảng dbo.BaoCaoI theo định dạng: <Mã Khóa học><BCI><01-99>")
                .HasColumnName("MaBC1");
            entity.Property(e => e.MaBc2)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasComment("Mã Báo cáo 2. Ghi Mã Báo cáo 2 trong bảng dbo.BaoCao2 theo định dạng: <Mã CSĐT><BCII><YY><01-99>")
                .HasColumnName("MaBC2");
            entity.Property(e => e.MaCsdt)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("Mã Cơ sở đào tạo lái xe. Ghi Mã CSDT trong bảng DM_DonViGTVT")
                .HasColumnName("MaCSDT");
            entity.Property(e => e.MaDvnhanHso)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("Nơi nhận hồ sơ = Mã Văn phòng đăng ký trong bảng DM_DonViGTVT")
                .HasColumnName("MaDVNhanHSo");
            entity.Property(e => e.MaHtcap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("Mã hình thức cấp. Ghi mã Hình thức cấp GPLX trong bảng DM_HTCapGPLX")
                .HasColumnName("MaHTCap");
            entity.Property(e => e.MaIn).HasMaxLength(255);
            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasComment("Mã khóa đào tạo, tham chiếu đến trường MaKH của bảng KhoaHoc");
            entity.Property(e => e.MaKySh)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasComment("Mã Kỳ sát hạch = <Mã TTSH><K><YY><01-99>")
                .HasColumnName("MaKySH");
            entity.Property(e => e.MaLoaiHs).HasComment("Loại hồ sơ. Tham chiếu đến bảng DM_LoaiHS");
            entity.Property(e => e.MaLyDoTcbc2)
                .HasComment("Mã Lý do từ chối Báo cáo 2. Ghi mã Lý do từ chối báo cáo 2 trong bảng dbo.DM_LyDoTCBC2")
                .HasColumnName("MaLyDoTCBC2");
            entity.Property(e => e.MaSoGtvt)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("Mã Sở GTVT. Ghi mã Sở GTVT trong bảng DM_DonViGTVT")
                .HasColumnName("MaSoGTVT");
            entity.Property(e => e.MaTrichNgang).HasMaxLength(30);
            entity.Property(e => e.MucDichCapDoi)
                .HasMaxLength(50)
                .HasComment("Mục đích cấp đổi, cấp lại GPLX");
            entity.Property(e => e.NamHocLx)
                .HasComment("Năm học lái xe trước đây. Ghi theo định dạng YYYY")
                .HasColumnName("NamHocLX");
            entity.Property(e => e.NamcapLandau)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.NgayCapCcn)
                .HasColumnType("datetime")
                .HasColumnName("NgayCapCCN");
            entity.Property(e => e.NgayCapGplxdaCo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Ngày cấp giấy phép lái xe hiện có của người lái xe")
                .HasColumnName("NgayCapGPLXDaCo");
            entity.Property(e => e.NgayHenTra)
                .HasComment("Ngày hẹn trả kết quả đối với cấp, đổi.")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayHhgplxdaCo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Ngày hết hạn GPLX hiện có của NGười lái xe")
                .HasColumnName("NgayHHGPLXDaCo");
            entity.Property(e => e.NgayInGiayTn)
                .HasColumnType("datetime")
                .HasColumnName("NgayInGiayTN");
            entity.Property(e => e.NgayKtbc1)
                .HasColumnType("datetime")
                .HasColumnName("NgayKTBC1");
            entity.Property(e => e.NgayKtbc2)
                .HasColumnType("datetime")
                .HasColumnName("NgayKTBC2");
            entity.Property(e => e.NgayNhanHso)
                .HasComment("Ngày nhận hồ sơ xin cấp, đổi GPLX")
                .HasColumnType("datetime")
                .HasColumnName("NgayNhanHSo");
            entity.Property(e => e.NgayQdsh)
                .HasComment("Ngày QĐ tổ chức kỳ sát hạch")
                .HasColumnType("datetime")
                .HasColumnName("NgayQDSH");
            entity.Property(e => e.NgayQdtt)
                .HasComment("Ngày QĐ công nhận trúng tuyển sát hạch")
                .HasColumnType("datetime")
                .HasColumnName("NgayQDTT");
            entity.Property(e => e.NgayRaQdtn)
                .HasColumnType("datetime")
                .HasColumnName("NgayRaQDTN");
            entity.Property(e => e.NgaySua)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayThuNhanAnh).HasColumnType("datetime");
            entity.Property(e => e.NgayTtgplxdaCo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NgayTTGPLXDaCo");
            entity.Property(e => e.NgayVaoSoCnn)
                .HasColumnType("datetime")
                .HasColumnName("NgayVaoSoCNN");
            entity.Property(e => e.NgayVaoSoTn)
                .HasColumnType("datetime")
                .HasColumnName("NgayVaoSoTN");
            entity.Property(e => e.NguoiKtbc1)
                .HasMaxLength(50)
                .HasColumnName("NguoiKTBC1");
            entity.Property(e => e.NguoiKtbc2)
                .HasMaxLength(50)
                .HasColumnName("NguoiKTBC2");
            entity.Property(e => e.NguoiKy)
                .HasMaxLength(50)
                .HasComment("Người ký quyết định cấp GPLX.");
            entity.Property(e => e.NguoiNhanHso)
                .HasMaxLength(50)
                .HasComment("Cán bộ nhận hồ sơ. Ghi đầy đủ họ và tên cán bộ")
                .HasColumnName("NguoiNhanHSo");
            entity.Property(e => e.NguoiSua).HasMaxLength(30);
            entity.Property(e => e.NguoiTao).HasMaxLength(30);
            entity.Property(e => e.NguoiThuNhanAnh).HasMaxLength(50);
            entity.Property(e => e.NhanXetDuong)
                .HasMaxLength(50)
                .HasComment("Nhận xét của Sát hạch viên Đường trường")
                .HasColumnName("NhanXet_Duong");
            entity.Property(e => e.NhanXetHinh)
                .HasMaxLength(50)
                .HasComment("Nhận xét của Sát hạch viên thi Hình")
                .HasColumnName("NhanXet_Hinh");
            entity.Property(e => e.NhanXetLyThuyet)
                .HasMaxLength(50)
                .HasComment("Nhận xét của Sát hạch viên Lý thuyết")
                .HasColumnName("NhanXet_LyThuyet");
            entity.Property(e => e.NhanXetMoPhong)
                .HasMaxLength(50)
                .HasColumnName("NhanXet_MoPhong");
            entity.Property(e => e.NoiCapGplxdaCo)
                .HasMaxLength(500)
                .HasColumnName("NoiCapGPLXDaCo");
            entity.Property(e => e.NoiDungSh)
                .HasComment("Nội dung sát hạch. Ghi mã trong bảng DM_NoiDungSH")
                .HasColumnName("NoiDungSH");
            entity.Property(e => e.SoBd)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("Số báo danh của thí sinh dự sát hạch lái xe =<001-999>")
                .HasColumnName("SoBD");
            entity.Property(e => e.SoCcn)
                .HasMaxLength(20)
                .HasComment("Số chứng chỉ nghề")
                .HasColumnName("SoCCN");
            entity.Property(e => e.SoGiayCntn)
                .HasMaxLength(20)
                .HasComment("Số giấy chứng nhận tốt nghiệp")
                .HasColumnName("SoGiayCNTN");
            entity.Property(e => e.SoGplxdaCo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Số GPLX hiện có của người lái xe.")
                .HasColumnName("SoGPLXDaCo");
            entity.Property(e => e.SoGplxtmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SoGPLXTmp");
            entity.Property(e => e.SoHoSo)
                .IsRequired()
                .HasMaxLength(18)
                .IsUnicode(false)
                .HasComment("Số hồ sơ = <HangGPLX><Số tự tăng theo hạng từ 01-99>");
            entity.Property(e => e.SoKmLxanToan)
                .HasComment("Số Km lái xe an toàn")
                .HasColumnName("SoKmLXAnToan");
            entity.Property(e => e.SoNamLx)
                .HasComment("Số năm lái xe")
                .HasColumnName("SoNamLX");
            entity.Property(e => e.SoQdsh)
                .HasMaxLength(20)
                .HasComment("Số quyết định tổ chức kỳ sát hạch")
                .HasColumnName("SoQDSH");
            entity.Property(e => e.SoQdtt)
                .HasMaxLength(20)
                .HasComment("Số QĐ công nhận trúng tuyển")
                .HasColumnName("SoQDTT");
            entity.Property(e => e.SoQuyetDinhTn)
                .HasMaxLength(50)
                .HasColumnName("SoQuyetDinhTN");
            entity.Property(e => e.SoSoTn)
                .HasMaxLength(50)
                .HasColumnName("SoSoTN");
            entity.Property(e => e.TrangThai).HasDefaultValueSql("((0))");
            entity.Property(e => e.TransferFlag).HasColumnName("Transfer_flag");
            entity.Property(e => e.TtXuLy)
                .IsRequired()
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasComment("Trạng thái xử lý hồ sơ cấp, đổi GPLX. Ghi mã trạng thái xử lý trong bảng DM_TrangThai")
                .HasColumnName("TT_XuLy");
            entity.Property(e => e.TtXuLyOld)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("TT_XuLy_Old");
            entity.Property(e => e.VaoSoCnnso)
                .HasMaxLength(150)
                .HasColumnName("VaoSoCNNSo");
            entity.Property(e => e.XepLoaiTotNghiep).HasMaxLength(150);

            entity.HasOne(d => d.HangDaoTaoNavigation).WithMany(p => p.NguoiLxHoSos)
                .HasForeignKey(d => d.HangDaoTao)
                .HasConstraintName("FK_NguoiLX_HoSo_DM_HangDT");

            entity.HasOne(d => d.HangGplxNavigation).WithMany(p => p.NguoiLxHoSos)
                .HasForeignKey(d => d.HangGplx)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiLX_HoSo_DM_HangGPLX");

            entity.HasOne(d => d.MaDkNavigation).WithOne(p => p.NguoiLxHoSo)
                .HasForeignKey<NguoiLxHoSo>(d => d.MaDk)
                .HasConstraintName("FK_NguoiLX_HoSo_NguoiLX");

            entity.HasOne(d => d.MaHtcapNavigation).WithMany(p => p.NguoiLxHoSos)
                .HasForeignKey(d => d.MaHtcap)
                .HasConstraintName("FK_NguoiLX_HoSo_DM_HTCapGPLX");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.NguoiLxHoSos)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NguoiLX_HoSo_KhoaHoc");

            entity.HasOne(d => d.MaLoaiHsNavigation).WithMany(p => p.NguoiLxHoSos)
                .HasForeignKey(d => d.MaLoaiHs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiLX_HoSo_DM_LoaiHSo");
        });

        modelBuilder.Entity<NguoiLxhsGiayTo>(entity =>
        {
            entity.HasKey(e => new { e.MaGt, e.MaDk });

            entity.ToTable("NguoiLXHS_GiayTo");

            entity.Property(e => e.MaGt).HasColumnName("MaGT");
            entity.Property(e => e.MaDk)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MaDK");
            entity.Property(e => e.SoHoSo)
                .IsRequired()
                .HasMaxLength(18)
                .IsUnicode(false);
            entity.Property(e => e.TenGt)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("TenGT");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasDefaultValueSql("((1))");

            entity.HasOne(d => d.MaDkNavigation).WithMany(p => p.NguoiLxhsGiayTos)
                .HasForeignKey(d => d.MaDk)
                .HasConstraintName("FK_NguoiLXHS_GiayTo_NguoiLX_HoSo");

            entity.HasOne(d => d.MaGtNavigation).WithMany(p => p.NguoiLxhsGiayTos)
                .HasForeignKey(d => d.MaGt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NguoiLXHS_GiayTo_DM_GiayTo");
        });

        modelBuilder.Entity<ThiSatHachKetQuaChiTietTkn>(entity =>
        {
            entity.HasKey(e => new { e.MaKySh, e.MaDk, e.MaPhanThi, e.IdQuyTac });

            entity.ToTable("ThiSatHach_KetQuaChiTietTkn");

            entity.Property(e => e.MaKySh)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDk)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdQuyTacNavigation).WithMany(p => p.ThiSatHachKetQuaChiTietTkns)
                .HasForeignKey(d => d.IdQuyTac)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThiSatHac__IdQuy__704434BB");

            entity.HasOne(d => d.MaPhanThiNavigation).WithMany(p => p.ThiSatHachKetQuaChiTietTkns)
                .HasForeignKey(d => d.MaPhanThi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThiSatHac__MaPha__713858F4");
        });

        modelBuilder.Entity<ThiSatHachKetQuaPhanThiTkn>(entity =>
        {
            entity.HasKey(e => new { e.MaKySh, e.MaDk, e.MaPhanThi });

            entity.ToTable("ThiSatHach_KetQuaPhanThiTkn");

            entity.Property(e => e.MaKySh)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDk)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.HangDaoTao)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiCham)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaPhanThiNavigation).WithMany(p => p.ThiSatHachKetQuaPhanThiTkns)
                .HasForeignKey(d => d.MaPhanThi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThiSatHac__MaPha__75FD0E11");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
