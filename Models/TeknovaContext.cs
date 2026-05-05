using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ttlaixe.Models;

public partial class TeknovaContext : DbContext
{
    public TeknovaContext()
    {
    }

    public TeknovaContext(DbContextOptions<TeknovaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<DmHocPhi> DmHocPhis { get; set; }

    public virtual DbSet<DmTaiKhoanKeToan> DmTaiKhoanKeToans { get; set; }

    public virtual DbSet<HoSoHocPhi> HoSoHocPhis { get; set; }

    public virtual DbSet<LichSuNopHocPhi> LichSuNopHocPhis { get; set; }

    public virtual DbSet<LichSuSoDu> LichSuSoDus { get; set; }

    public virtual DbSet<NhatKyChungTu> NhatKyChungTus { get; set; }

    public virtual DbSet<UserTkn> UserTkns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC0751694ECE");

            entity.ToTable("AuditLog");

            entity.Property(e => e.ActionType).HasMaxLength(20);
            entity.Property(e => e.KeyValue).HasMaxLength(500);
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(200);
            entity.Property(e => e.TableName).HasMaxLength(200);
        });

        modelBuilder.Entity<DmHocPhi>(entity =>
        {
            entity.HasKey(e => e.MaHangGplx).HasName("PK__DmHocPhi__3E27F55A18E351E4");

            entity.ToTable("DmHocPhi");

            entity.Property(e => e.MaHangGplx)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaHangGPLX");
            entity.Property(e => e.HocPhi).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.NgayChinhSuaCuoiCung).HasColumnType("datetime");
            entity.Property(e => e.NgayKhoiTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<DmTaiKhoanKeToan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__DmTaiKho__3214EC07EFA31CE2");

            entity.ToTable("DmTaiKhoanKeToan");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__DmTaiKho__AD7C6528E668E4AC").IsUnique();

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.MaLoaiTaiKhoan)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.MaTaiKhoanCha)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaTaiKhoanChaTrue)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayChinhSuaCuoiCung).HasColumnType("datetime");
            entity.Property(e => e.NgayKhoiTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenTaiKhoan)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<HoSoHocPhi>(entity =>
        {
            entity.HasKey(e => e.MaDk).HasName("PK__HoSoHocP__2725866C0BDC1C45");

            entity.ToTable("HoSoHocPhi");

            entity.HasIndex(e => e.MaHangGplx, "IX_HoSoHocPhi_MaHangGPLX");

            entity.HasIndex(e => e.MaKhoaHoc, "IX_HoSoHocPhi_MaKhoaHoc");

            entity.HasIndex(e => e.SoCmt, "IX_HoSoHocPhi_SoCMT");

            entity.Property(e => e.MaDk)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MaDK");
            entity.Property(e => e.BoHoc)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.DaHoanThanhHp)
                .IsRequired()
                .HasDefaultValueSql("('N')");
            entity.Property(e => e.GioiTinh)
                .IsRequired()
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HoVaTen)
                .IsRequired()
                .HasMaxLength(70);
            entity.Property(e => e.HocPhi).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaHangGplx)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MaHangGPLX");
            entity.Property(e => e.MaKhoaHoc)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.NgayChinhSuaCuoiCung).HasColumnType("datetime");
            entity.Property(e => e.NgayKhoiTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgaySinh)
                .IsRequired()
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.NoiCuTru).HasMaxLength(100);
            entity.Property(e => e.NoiThuongTru).HasMaxLength(100);
            entity.Property(e => e.SoCmt)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SoCMT");

            entity.HasOne(d => d.MaHangGplxNavigation).WithMany(p => p.HoSoHocPhis)
                .HasForeignKey(d => d.MaHangGplx)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoSoHocPhi_DmHocPhi");
        });

        modelBuilder.Entity<LichSuNopHocPhi>(entity =>
        {
            entity.HasKey(e => e.IdNopTien);

            entity.ToTable("LichSuNopHocPhi");

            entity.HasIndex(e => e.MaDk, "IX_LichSuNopHocPhi_MaDK");

            entity.HasIndex(e => e.NgayNop, "IX_LichSuNopHocPhi_NgayNop");

            entity.Property(e => e.GhiChu).HasMaxLength(250);
            entity.Property(e => e.HinhThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.MaDk)
                .IsRequired()
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("MaDK");
            entity.Property(e => e.NgayKhoiTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayNop)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoBienLai)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SoTienNop).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDkNavigation).WithMany(p => p.LichSuNopHocPhis)
                .HasForeignKey(d => d.MaDk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LichSuNopHocPhi_HoSoHocPhi");
        });

        modelBuilder.Entity<LichSuSoDu>(entity =>
        {
            entity.HasKey(e => new { e.Nam, e.MaTaiKhoan }).HasName("PK_LichSuNoCoNam");

            entity.ToTable("LichSuSoDu");

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Co).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.No).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TenTaiKhoan)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<NhatKyChungTu>(entity =>
        {
            entity.HasKey(e => e.IdChungTu).HasName("PK__NhatKyCh__F9F2A508D665A21C");

            entity.ToTable("NhatKyChungTu");

            entity.HasIndex(e => e.NgayLap, "IX_NhatKyChungTu_NgayLap");

            entity.HasIndex(e => e.SoChungTu, "IX_NhatKyChungTu_SoChungTu");

            entity.HasIndex(e => e.TaiKhoanCo, "IX_NhatKyChungTu_TaiKhoanCo");

            entity.HasIndex(e => e.TaiKhoanNo, "IX_NhatKyChungTu_TaiKhoanNo");

            entity.Property(e => e.DienGiai).HasMaxLength(250);
            entity.Property(e => e.GhiChu).HasMaxLength(250);
            entity.Property(e => e.NgayKhoiTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayLap).HasColumnType("date");
            entity.Property(e => e.SoChungTu)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaiKhoanCo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TaiKhoanNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.TaiKhoanCoNavigation).WithMany(p => p.NhatKyChungTuTaiKhoanCoNavigations)
                .HasForeignKey(d => d.TaiKhoanCo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhatKyChungTu_TaiKhoanCo");

            entity.HasOne(d => d.TaiKhoanNoNavigation).WithMany(p => p.NhatKyChungTuTaiKhoanNoNavigations)
                .HasForeignKey(d => d.TaiKhoanNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhatKyChungTu_TaiKhoanNo");
        });

        modelBuilder.Entity<UserTkn>(entity =>
        {
            entity.HasKey(e => e.UserName);

            entity.ToTable("UserTkn");

            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.GioiTinh)
                .IsRequired()
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HoTen)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.MaNguoiChinhSua).HasMaxLength(100);
            entity.Property(e => e.MaNguoiNhap).HasMaxLength(100);
            entity.Property(e => e.NgayChinhSuaCuoiCung).HasPrecision(0);
            entity.Property(e => e.NgayKhoiTao).HasPrecision(0);
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenNguoiChinhSua).HasMaxLength(150);
            entity.Property(e => e.TenNguoiNhap).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
