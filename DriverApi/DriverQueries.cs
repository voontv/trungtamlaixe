using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Ttlaixe.DriverApi
{
    /// <summary>
    /// Explicit parameterized SQL over the CSDT database, the same statements as the NestJS service.
    /// DM_TrangThai is not part of the EF model, hence Dapper instead of the DbContext.
    /// </summary>
    public static class DriverQueries
    {
        private record FilterDefinition(string Name, string Column, bool Ansi, int Length);

        // Order matters: it is the order of the predicates in the WHERE clause.
        private static readonly FilterDefinition[] Filters =
        {
            new("soHoSo", "hs.SoHoSo", true, 18),
            new("ttXuLy", "hs.TT_XuLy", true, 2),
            new("maDK", "nl.MaDK", true, 25),
            new("hoVaTen", "nl.HoVaTen", false, 50),
            new("ngaySinh", "nl.NgaySinh", true, 8),
            new("hangGPLX", "hs.HangGPLX", true, 3),
            new("hangDaoTao", "hs.HangDaoTao", true, 20),
            new("tenTT", "tt.TenTT", false, 50),
        };

        private const int QParameterLength = 200;

        public const int VerifyMaxMatches = 20;

        private const string FromAndWhere = @"
FROM dbo.NguoiLX AS nl
INNER JOIN dbo.NguoiLX_HoSo AS hs ON nl.MaDK = hs.MaDK
INNER JOIN dbo.DM_TrangThai AS tt ON hs.TT_XuLy = tt.MaTT
WHERE nl.TrangThai = 1
  AND hs.TrangThai = 1
  AND tt.TrangThai = 1";

        /// <summary>Escapes LIKE wildcards so user input is matched literally.</summary>
        public static string ToContainsPattern(string value)
        {
            var escaped = new System.Text.StringBuilder("%");
            foreach (var c in value)
            {
                if (c == '\\' || c == '%' || c == '_' || c == '[')
                {
                    escaped.Append('\\');
                }

                escaped.Append(c);
            }

            return escaped.Append('%').ToString();
        }

        public static (string DataQuery, string CountQuery, DynamicParameters CountParameters, DynamicParameters DataParameters)
            BuildSearch(DriverSearchCriteria criteria)
        {
            var filterParameters = new DynamicParameters();
            var predicates = new List<string>();

            foreach (var definition in Filters)
            {
                if (!criteria.Filters.TryGetValue(definition.Name, out var value))
                {
                    continue;
                }

                // hangGPLX takes one value or several separated by commas ("B1,B11").
                var values = definition.Name == "hangGPLX" ? value.Split(',') : new[] { value };

                if (values.Length == 1)
                {
                    predicates.Add($"{definition.Column} = @{definition.Name}");
                    filterParameters.Add(definition.Name, Text(value, definition.Ansi, definition.Length));
                    continue;
                }

                var names = values.Select((_, index) => $"{definition.Name}{index}").ToList();
                predicates.Add($"{definition.Column} IN ({string.Join(", ", names.Select(n => "@" + n))})");
                for (var index = 0; index < values.Length; index++)
                {
                    filterParameters.Add(names[index], Text(values[index], definition.Ansi, definition.Length));
                }
            }

            if (criteria.Filters.TryGetValue("q", out var q))
            {
                predicates.Add(
                    "(nl.HoVaTen COLLATE Latin1_General_100_CI_AI LIKE @q ESCAPE '\\' OR nl.MaDK LIKE @q ESCAPE '\\')");
                filterParameters.Add("q", Text(ToContainsPattern(q), false, QParameterLength));
            }

            var where = predicates.Count == 0 ? "" : "\n  AND " + string.Join("\n  AND ", predicates);

            var dataParameters = new DynamicParameters(filterParameters);
            dataParameters.Add("offset", (criteria.Page - 1) * criteria.Limit);
            dataParameters.Add("limit", criteria.Limit);

            var dataQuery = $@"
SELECT
  hs.SoHoSo AS soHoSo,
  hs.TT_XuLy AS ttXuLy,
  nl.MaDK AS maDK,
  nl.HoVaTen AS hoVaTen,
  nl.NgaySinh AS ngaySinh,
  hs.HangGPLX AS hangGPLX,
  hs.HangDaoTao AS hangDaoTao,
  CONVERT(char(10), hs.NgayNhanHSo, 23) AS ngayNhanHSo,
  tt.TenTT AS tenTT
{FromAndWhere}{where}
ORDER BY nl.MaDK ASC
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";

            var countQuery = $@"
SELECT COUNT_BIG(1) AS total
{FromAndWhere}{where};";

            return (dataQuery, countQuery, filterParameters, dataParameters);
        }

        /// <summary>
        /// Active drivers matching both credentials (identity number and birth date), newest HoSo first.
        /// SoCMT is only compared in SQL; it is never selected.
        /// </summary>
        public static (string Query, DynamicParameters Parameters) BuildVerify(string soCmt, string ngaySinh)
        {
            var parameters = new DynamicParameters();
            parameters.Add("soCMT", Text(soCmt, true, 20));
            parameters.Add("ngaySinh", Text(ngaySinh, true, 8));

            return ($@"
SELECT TOP ({VerifyMaxMatches})
  hs.SoHoSo AS soHoSo,
  hs.TT_XuLy AS ttXuLy,
  nl.MaDK AS maDK,
  nl.HoVaTen AS hoVaTen,
  nl.NgaySinh AS ngaySinh,
  hs.HangGPLX AS hangGPLX,
  hs.HangDaoTao AS hangDaoTao,
  tt.TenTT AS tenTT,
  hs.NgayTao AS ngayTao
FROM dbo.NguoiLX AS nl
INNER JOIN dbo.NguoiLX_HoSo AS hs ON nl.MaDK = hs.MaDK
INNER JOIN dbo.DM_TrangThai AS tt ON hs.TT_XuLy = tt.MaTT
WHERE nl.TrangThai = 1
  AND hs.TrangThai = 1
  AND tt.TrangThai = 1
  AND nl.SoCMT = @soCMT
  AND nl.NgaySinh = @ngaySinh
ORDER BY hs.NgayTao DESC, nl.MaDK ASC;", parameters);
        }

        /// <summary>
        /// Active drivers by MaDK together with the photo path of their active HoSo.
        /// A driver without a HoSo is still returned, just without a photo path.
        /// </summary>
        public static (string Query, DynamicParameters Parameters) BuildProfiles(IReadOnlyList<string> maDKs)
        {
            var parameters = new DynamicParameters();
            for (var index = 0; index < maDKs.Count; index++)
            {
                parameters.Add($"maDK{index}", Text(maDKs[index], true, 25));
            }

            var names = string.Join(", ", Enumerable.Range(0, maDKs.Count).Select(i => $"@maDK{i}"));

            return ($@"
SELECT
  nl.MaDK AS maDK,
  nl.NgaySinh AS ngaySinh,
  nl.SoCMT AS soCMT,
  hs.DuongDanAnh AS duongDanAnh
FROM dbo.NguoiLX AS nl
LEFT JOIN dbo.NguoiLX_HoSo AS hs ON hs.MaDK = nl.MaDK AND hs.TrangThai = 1
WHERE nl.TrangThai = 1
  AND nl.MaDK IN ({names})
ORDER BY nl.MaDK ASC;", parameters);
        }

        private static DbString Text(string value, bool ansi, int length)
            => new DbString { Value = value, IsAnsi = ansi, Length = length };
    }
}
