using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Ttlaixe.Providers;

namespace Ttlaixe.DriverApi
{
    public interface IDriversBusiness
    {
        Task<DriverPageResponse> FindAsync(DriverSearchCriteria criteria);

        Task<DriverVerifyResponse> VerifyAsync(DriverVerifyRequest request);

        Task<DriverProfilesResponse> GetProfilesAsync(IReadOnlyList<string> maDKs);
    }

    /// <summary>Read-only driver queries. The database follows the x-loai-xe header like the rest of the API.</summary>
    public class DriversBusiness : IDriversBusiness
    {
        private const int PhotoConcurrency = 4;

        private readonly IGplxTenantProvider _tenant;
        private readonly IDriverPhotoService _photos;

        public DriversBusiness(IGplxTenantProvider tenant, IDriverPhotoService photos)
        {
            _tenant = tenant;
            _photos = photos;
        }

        public async Task<DriverPageResponse> FindAsync(DriverSearchCriteria criteria)
        {
            var queries = DriverQueries.BuildSearch(criteria);

            await using var connection = new SqlConnection(_tenant.GetConnectionString());
            var total = await connection.ExecuteScalarAsync<long>(queries.CountQuery, queries.CountParameters);
            var data = await connection.QueryAsync<DriverRecord>(queries.DataQuery, queries.DataParameters);

            return new DriverPageResponse
            {
                Data = data.ToList(),
                Page = criteria.Page,
                Limit = criteria.Limit,
                Total = total,
                TotalPages = (long)Math.Ceiling(total / (double)criteria.Limit),
            };
        }

        public async Task<DriverVerifyResponse> VerifyAsync(DriverVerifyRequest request)
        {
            var (query, parameters) = DriverQueries.BuildVerify(request.SoCMT, request.NgaySinh);

            await using var connection = new SqlConnection(_tenant.GetConnectionString());
            var data = await connection.QueryAsync<VerifiedDriver>(query, parameters);

            return new DriverVerifyResponse { Data = data.ToList() };
        }

        public async Task<DriverProfilesResponse> GetProfilesAsync(IReadOnlyList<string> maDKs)
        {
            var (query, parameters) = DriverQueries.BuildProfiles(maDKs);

            List<ProfileRow> rows;
            await using (var connection = new SqlConnection(_tenant.GetConnectionString()))
            {
                rows = (await connection.QueryAsync<ProfileRow>(query, parameters)).ToList();
            }

            var data = new DriverProfile[rows.Count];
            await Parallel.ForEachAsync(
                Enumerable.Range(0, rows.Count),
                new ParallelOptions { MaxDegreeOfParallelism = PhotoConcurrency },
                async (index, _) =>
                {
                    var row = rows[index];
                    data[index] = new DriverProfile
                    {
                        MaDK = row.MaDK,
                        NgaySinh = row.NgaySinh,
                        SoCMT = row.SoCMT,
                        AnhBase64 = await _photos.LoadAsync(row.DuongDanAnh),
                    };
                });

            return new DriverProfilesResponse { Data = data.ToList() };
        }

        private class ProfileRow
        {
            public string MaDK { get; set; }
            public string NgaySinh { get; set; }
            public string SoCMT { get; set; }
            public string DuongDanAnh { get; set; }
        }
    }
}
