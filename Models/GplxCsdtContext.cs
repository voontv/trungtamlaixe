
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

    public virtual DbSet<DmLoaiHsoGiayTo> DmLoaiHsoGiayTos { get; set; }

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
