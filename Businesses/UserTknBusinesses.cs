using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ttlaixe.AutoConfig;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Exceptions;
using Ttlaixe.LibsStartup;
using Ttlaixe.Models;
using Ttlaixe.Providers;  // ITokenGenerator (của bạn)

namespace Ttlaixe.Businesses
{
    
    [ImplementBy(typeof(UserTknBusinesses))]
    public interface IUserTknBusinesses
    {
        Task<UserTknLoginResponse> LoginAsync(UserTknLoginRequest request);

        Task<UserTknResponse> CreateAsync(UserTknCreateRequest request);
        Task<UserTknResponse> UpdateAsync(string userName, UserTknUpdateRequest request);
        Task<bool> DeleteAsync(string userName);

        Task<List<UserTknResponse>> GetAllAsync();
        Task<UserTknResponse> GetByUserNameAsync(string userName);
    }

    public class UserTknBusinesses : IUserTknBusinesses
    {
        private readonly GplxCsdtContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAuthenInfo _authenInfo;

        public UserTknBusinesses(GplxCsdtContext context, ITokenGenerator tokenGenerator, IAuthenInfo authenInfo)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _authenInfo = authenInfo;
        }

        public async Task<UserTknLoginResponse> LoginAsync(UserTknLoginRequest request)
        {
            if (request == null) throw new BadRequestException("ThongTinNguoiHocLai không hợp lệ.");

            var userName = (request.UserName ?? "").Trim();
            var password = request.Password ?? "";

            if (string.IsNullOrWhiteSpace(userName))
                throw new BadRequestException("UserName không được trống.");

            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("Password không được trống.");

            var passHash = Utils.MD5Hash(password);

            var user = await _context.UserTkns
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == userName && x.PasswordHash == passHash)
                ?? throw new BadRequestException("Thông tin đăng nhập bị lỗi, vui lòng kiểm tra lại tên đăng nhập");

            var token = _tokenGenerator.GenerateToken(user.UserName).Token;

            return new UserTknLoginResponse
            {
                Token = token,
                BearerToken = "Bearer " + token,

                UserName = user.UserName,
                HoTen = user.HoTen,
                GioiTinh = user.GioiTinh,
                SoDienThoai = user.SoDienThoai,

                QuyenAdmin = user.QuyenAdmin,
                QuyenKeToan = user.QuyenKeToan,
                QuyenNhapLieu = user.QuyenNhapLieu,
                QuyenThayGiao = user.QuyenThayGiao
            };
        }

        public async Task<List<UserTknResponse>> GetAllAsync()
        {
            var list = await _context.UserTkns
                .AsNoTracking()
                .OrderByDescending(x => x.NgayKhoiTao)
                .ToListAsync();

            return list.Select(x => new UserTknResponse
            {
                UserName = x.UserName,

                HoTen = x.HoTen,
                GioiTinh = x.GioiTinh,
                SoDienThoai = x.SoDienThoai,

                QuyenAdmin = x.QuyenAdmin,
                QuyenKeToan = x.QuyenKeToan,
                QuyenNhapLieu = x.QuyenNhapLieu,
                QuyenThayGiao = x.QuyenThayGiao,

                NgayKhoiTao = x.NgayKhoiTao,
                NgayChinhSuaCuoiCung = x.NgayChinhSuaCuoiCung,

                MaNguoiNhap = x.MaNguoiNhap,
                TenNguoiNhap = x.TenNguoiNhap,
                MaNguoiChinhSua = x.MaNguoiChinhSua,
                TenNguoiChinhSua = x.TenNguoiChinhSua
            }).ToList();
        }

        public async Task<UserTknResponse> GetByUserNameAsync(string userName)
        {
            userName = (userName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(userName)) throw new BadRequestException("UserName không hợp lệ.");

            var x = await _context.UserTkns.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == userName)
                ?? throw new BadRequestException("Không tìm thấy user.");

            return new UserTknResponse
            {
                UserName = x.UserName,

                HoTen = x.HoTen,
                GioiTinh = x.GioiTinh,
                SoDienThoai = x.SoDienThoai,

                QuyenAdmin = x.QuyenAdmin,
                QuyenKeToan = x.QuyenKeToan,
                QuyenNhapLieu = x.QuyenNhapLieu,
                QuyenThayGiao = x.QuyenThayGiao,

                NgayKhoiTao = x.NgayKhoiTao,
                NgayChinhSuaCuoiCung = x.NgayChinhSuaCuoiCung,

                MaNguoiNhap = x.MaNguoiNhap,
                TenNguoiNhap = x.TenNguoiNhap,
                MaNguoiChinhSua = x.MaNguoiChinhSua,
                TenNguoiChinhSua = x.TenNguoiChinhSua
            };
        }

        public async Task<UserTknResponse> CreateAsync(UserTknCreateRequest request)
        {
            if (request == null) throw new BadRequestException("ThongTinNguoiHocLai không hợp lệ.");

            request.UserName = (request.UserName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(request.UserName)) throw new BadRequestException("UserName không được trống.");
            if (string.IsNullOrWhiteSpace(request.Pass)) throw new BadRequestException("Pass không được trống.");
            if (string.IsNullOrWhiteSpace(request.HoTen)) throw new BadRequestException("Họ tên không được trống.");
            if (request.GioiTinh != "M" && request.GioiTinh != "F") throw new BadRequestException("Giới tính chỉ nhận M hoặc F.");

            if (!string.IsNullOrWhiteSpace(request.SoDienThoai) && request.SoDienThoai.Length != 10)
                throw new BadRequestException("Số điện thoại phải đúng 10 ký tự (VARCHAR(10)).");

            var exists = await _context.UserTkns.AnyAsync(x => x.UserName == request.UserName);
            if (exists) throw new BadRequestException("User đã tồn tại, vui lòng tạo user name khác.");

            var logged = _authenInfo.Get();
            var actor = await _context.UserTkns.FindAsync(logged.UserName);
            if (!actor.QuyenAdmin)
            {
                throw new BadRequestException("Chỉ có admin mới tạo được tài khoảng. ");
            }
            var now = DateTime.Now;

            var entity = new UserTkn
            {
                UserName = request.UserName,
                PasswordHash = Utils.MD5Hash(request.Pass),

                HoTen = request.HoTen?.Trim(),
                GioiTinh = request.GioiTinh,
                SoDienThoai = request.SoDienThoai,

                QuyenAdmin = request.QuyenAdmin,
                QuyenKeToan = request.QuyenKeToan,
                QuyenNhapLieu = request.QuyenNhapLieu,
                QuyenThayGiao = request.QuyenThayGiao,

                NgayKhoiTao = now,
                NgayChinhSuaCuoiCung = now,

                MaNguoiNhap = actor?.UserName,
                TenNguoiNhap = actor?.HoTen,
                MaNguoiChinhSua = actor?.UserName,
                TenNguoiChinhSua = actor?.HoTen
            };

            _context.UserTkns.Add(entity);
            await _context.SaveChangesAsync();

            return new UserTknResponse
            {
                UserName = entity.UserName,

                HoTen = entity.HoTen,
                GioiTinh = entity.GioiTinh,
                SoDienThoai = entity.SoDienThoai,

                QuyenAdmin = entity.QuyenAdmin,
                QuyenKeToan = entity.QuyenKeToan,
                QuyenNhapLieu = entity.QuyenNhapLieu,
                QuyenThayGiao = entity.QuyenThayGiao,

                NgayKhoiTao = entity.NgayKhoiTao,
                NgayChinhSuaCuoiCung = entity.NgayChinhSuaCuoiCung,

                MaNguoiNhap = entity.MaNguoiNhap,
                TenNguoiNhap = entity.TenNguoiNhap,
                MaNguoiChinhSua = entity.MaNguoiChinhSua,
                TenNguoiChinhSua = entity.TenNguoiChinhSua
            };
        }

        public async Task<UserTknResponse> UpdateAsync(string userName, UserTknUpdateRequest request)
        {
            userName = (userName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(userName)) throw new BadRequestException("UserName không hợp lệ.");
            if (request == null) throw new BadRequestException("ThongTinNguoiHocLai không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.HoTen)) throw new BadRequestException("Họ tên không được trống.");
            if (request.GioiTinh != "M" && request.GioiTinh != "F") throw new BadRequestException("Giới tính chỉ nhận M hoặc F.");

            if (!string.IsNullOrWhiteSpace(request.SoDienThoai) && request.SoDienThoai.Length != 10)
                throw new BadRequestException("Số điện thoại phải đúng 10 ký tự (VARCHAR(10)).");

            var entity = await _context.UserTkns.FirstOrDefaultAsync(x => x.UserName == userName)
                ?? throw new BadRequestException("Không tìm thấy user.");

            var logged = _authenInfo.Get();
            var actor = logged == null ? null : await _context.UserTkns.FindAsync(logged.UserName);

            entity.HoTen = request.HoTen?.Trim();
            entity.GioiTinh = request.GioiTinh;
            entity.SoDienThoai = request.SoDienThoai;

            entity.QuyenAdmin = request.QuyenAdmin;
            entity.QuyenKeToan = request.QuyenKeToan;
            entity.QuyenNhapLieu = request.QuyenNhapLieu;
            entity.QuyenThayGiao = request.QuyenThayGiao;

            if (!string.IsNullOrWhiteSpace(request.Pass))
                entity.PasswordHash = Utils.MD5Hash(request.Pass);

            entity.NgayChinhSuaCuoiCung = DateTime.Now;
            entity.MaNguoiChinhSua = actor?.UserName;
            entity.TenNguoiChinhSua = actor?.HoTen;

            await _context.SaveChangesAsync();

            return new UserTknResponse
            {
                UserName = entity.UserName,

                HoTen = entity.HoTen,
                GioiTinh = entity.GioiTinh,
                SoDienThoai = entity.SoDienThoai,

                QuyenAdmin = entity.QuyenAdmin,
                QuyenKeToan = entity.QuyenKeToan,
                QuyenNhapLieu = entity.QuyenNhapLieu,
                QuyenThayGiao = entity.QuyenThayGiao,

                NgayKhoiTao = entity.NgayKhoiTao,
                NgayChinhSuaCuoiCung = entity.NgayChinhSuaCuoiCung,

                MaNguoiNhap = entity.MaNguoiNhap,
                TenNguoiNhap = entity.TenNguoiNhap,
                MaNguoiChinhSua = entity.MaNguoiChinhSua,
                TenNguoiChinhSua = entity.TenNguoiChinhSua
            };
        }

        public async Task<bool> DeleteAsync(string userName)
        {
            userName = (userName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(userName)) return false;

            var entity = await _context.UserTkns.FirstOrDefaultAsync(x => x.UserName == userName);
            if (entity == null) return false;

            _context.UserTkns.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

