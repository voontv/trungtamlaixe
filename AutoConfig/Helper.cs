// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Ttlaixe.Models;

// namespace Ttlaixe.Helpers
// {
//     public static class Helper
//     {
//         /// Lấy UserId từ mã NldId
//         public static async Task<int?> GetUserIdByNldIdAsync(QlcongVanContext context, string? nldId)
//         {
//             if (string.IsNullOrWhiteSpace(nldId))
//                 return null;

//             var user = await context.Users
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(u => u.NldId == nldId);

//             return user?.UserId;
//         }
//         /// Lấy danh sách UserId từ danh sách mã NldId
//         public static async Task<List<int>> GetUserIdsFromNldIdsAsync(QlcongVanContext context, List<string>? nldIds)
//         {
//             if (nldIds == null || !nldIds.Any())
//                 return new List<int>();

//             return await context.Users
//                 .Where(u => nldIds.Contains(u.NldId))
//                 .Select(u => u.UserId)
//                 .ToListAsync();
//         }
//         /// Lấy mã NldId từ UserId
//         public static async Task<string?> GetNldIdByUserIdAsync(QlcongVanContext context, int? userId)
//         {
//             if (!userId.HasValue)
//                 return null;

//             var user = await context.Users
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(u => u.UserId == userId.Value);

//             return user?.NldId;
//         }
//         /// Lấy danh sách mã NldId từ danh sách UserId
//         public static async Task<List<string>> GetNldIdsByUserIdsAsync(QlcongVanContext context, List<int>? userIds)
//         {
//             if (userIds == null || !userIds.Any())
//                 return new List<string>();

//             return await context.Users
//                 .Where(u => userIds.Contains(u.UserId))
//                 .Select(u => u.NldId)
//                 .ToListAsync();
//         }
//         // Lấy PhongBanId từ mã phòng ban (MaPhongban)
//         public static async Task<int?> GetPhongBanIdByCodeAsync(QlcongVanContext context, string? maPhongBan)
//         {
//             if (string.IsNullOrWhiteSpace(maPhongBan))
//                 return null;

//             var pb = await context.PhongBans
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(p => p.MaPhongban == maPhongBan);

//             return pb?.PhongBanId;
//         }
//         // Lấy danh sách PhongBanId từ danh sách mã phòng ban
//         public static async Task<List<int>> GetPhongBanIdsByCodesAsync(QlcongVanContext context, List<string>? maPhongBans)
//         {
//             if (maPhongBans == null || !maPhongBans.Any())
//                 return new List<int>();

//             return await context.PhongBans
//                 .Where(p => maPhongBans.Contains(p.MaPhongban))
//                 .Select(p => p.PhongBanId)
//                 .ToListAsync();
//         }
//         // Lấy mã phòng ban từ PhongBanId
//         public static async Task<string?> GetPhongBanCodeByIdAsync(QlcongVanContext context, int? phongBanId)
//         {
//             if (!phongBanId.HasValue)
//                 return null;

//             var pb = await context.PhongBans
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(p => p.PhongBanId == phongBanId.Value);

//             return pb?.MaPhongban;
//         }
//         // Lấy danh sách mã phòng ban từ danh sách PhongBanId
//         public static async Task<List<string>> GetPhongBanCodesByIdsAsync(QlcongVanContext context, List<int>? phongBanIds)
//         {
//             if (phongBanIds == null || !phongBanIds.Any())
//                 return new List<string>();

//             return await context.PhongBans
//                 .Where(p => phongBanIds.Contains(p.PhongBanId))
//                 .Select(p => p.MaPhongban)
//                 .ToListAsync();
//         }
//         // Lấy NoiPhatHanhId từ mã nơi phát hành
//         public static async Task<int?> GetNoiPhatHanhIdByCodeAsync(QlcongVanContext context, string? maNoiPhatHanh)
//         {
//             if (string.IsNullOrWhiteSpace(maNoiPhatHanh))
//                 return null;

//             var nph = await context.NoiPhatHanh
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(n => n.MaNoiPhatHanh == maNoiPhatHanh);

//             return nph?.NoiPhatHanhId;
//         }
//         // Lấy mã nơi phát hành từ ID
//         public static async Task<string?> GetNoiPhatHanhCodeByIdAsync(QlcongVanContext context, int? noiPhatHanhId)
//         {
//             if (!noiPhatHanhId.HasValue)
//                 return null;

//             var nph = await context.NoiPhatHanh
//                 .AsNoTracking()
//                 .FirstOrDefaultAsync(n => n.NoiPhatHanhId == noiPhatHanhId.Value);

//             return nph?.MaNoiPhatHanh;
//         }
//     }
// }
