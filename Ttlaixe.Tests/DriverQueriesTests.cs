using System.Linq;
using Dapper;
using Ttlaixe.DriverApi;
using Xunit;

namespace Ttlaixe.Tests
{
    public class DriverQueriesTests
    {
        private static DriverSearchCriteria Criteria(int page = 1, int limit = 10, params (string, string)[] filters)
        {
            var criteria = new DriverSearchCriteria { Page = page, Limit = limit };
            foreach (var (key, value) in filters)
            {
                criteria.Filters[key] = value;
            }

            return criteria;
        }

        [Fact]
        public void Search_without_filters_only_keeps_active_records_sorted_by_MaDK()
        {
            var (data, count, _, parameters) = DriverQueries.BuildSearch(Criteria(3, 7));

            Assert.Contains("nl.TrangThai = 1", data);
            Assert.Contains("hs.TrangThai = 1", data);
            Assert.Contains("tt.TrangThai = 1", data);
            Assert.Contains("ORDER BY nl.MaDK ASC", data);
            Assert.Contains("OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY", data);
            Assert.DoesNotContain("OFFSET", count);
            Assert.Equal(14, parameters.Get<int>("offset"));
            Assert.Equal(7, parameters.Get<int>("limit"));
        }

        [Fact]
        public void Search_matches_filters_exactly_with_parameters()
        {
            var (data, count, countParameters, _) = DriverQueries.BuildSearch(
                Criteria(1, 10, ("maDK", "DK-1"), ("tenTT", "x'; DROP TABLE a;--")));

            Assert.Contains("nl.MaDK = @maDK", data);
            Assert.Contains("tt.TenTT = @tenTT", data);
            Assert.DoesNotContain("DROP TABLE", data);
            Assert.Contains("tt.TenTT = @tenTT", count);
            Assert.Equal(new[] { "maDK", "tenTT" }, countParameters.ParameterNames.OrderBy(x => x));
        }

        [Fact]
        public void Search_expands_several_hangGPLX_codes_to_IN()
        {
            var (data, _, _, parameters) = DriverQueries.BuildSearch(Criteria(1, 10, ("hangGPLX", "B1,B11")));

            Assert.Contains("hs.HangGPLX IN (@hangGPLX0, @hangGPLX1)", data);
            Assert.Contains("hangGPLX0", parameters.ParameterNames);
            Assert.Contains("hangGPLX1", parameters.ParameterNames);
        }

        [Fact]
        public void Search_uses_equality_for_a_single_hangGPLX_code()
        {
            var (data, _, _, _) = DriverQueries.BuildSearch(Criteria(1, 10, ("hangGPLX", "B1")));

            Assert.Contains("hs.HangGPLX = @hangGPLX", data);
        }

        [Fact]
        public void Search_q_is_a_diacritic_insensitive_substring_match_on_name_or_MaDK()
        {
            var (data, _, _, _) = DriverQueries.BuildSearch(Criteria(1, 10, ("q", "nguyen")));

            Assert.Contains("nl.HoVaTen COLLATE Latin1_General_100_CI_AI LIKE @q ESCAPE '\\'", data);
            Assert.Contains("nl.MaDK LIKE @q ESCAPE '\\'", data);
        }

        [Theory]
        [InlineData("a%b", "%a\\%b%")]
        [InlineData("a_b", "%a\\_b%")]
        [InlineData("a[b", "%a\\[b%")]
        [InlineData(@"a\b", @"%a\\b%")]
        [InlineData("plain", "%plain%")]
        public void ToContainsPattern_escapes_LIKE_wildcards(string value, string expected)
        {
            Assert.Equal(expected, DriverQueries.ToContainsPattern(value));
        }

        [Fact]
        public void Verify_compares_both_credentials_but_never_selects_the_identity_number()
        {
            var (query, parameters) = DriverQueries.BuildVerify("012345678901", "20021110");

            Assert.Contains("nl.SoCMT = @soCMT", query);
            Assert.Contains("nl.NgaySinh = @ngaySinh", query);
            Assert.Contains("SELECT TOP (20)", query);
            Assert.Contains("ORDER BY hs.NgayTao DESC, nl.MaDK ASC", query);
            Assert.DoesNotContain("AS soCMT", query);
            Assert.Equal(new[] { "ngaySinh", "soCMT" }, parameters.ParameterNames.OrderBy(x => x));
        }

        [Fact]
        public void Profiles_keeps_a_driver_without_an_active_HoSo()
        {
            var (query, parameters) = DriverQueries.BuildProfiles(new[] { "a", "b", "c" });

            Assert.Contains("LEFT JOIN dbo.NguoiLX_HoSo AS hs ON hs.MaDK = nl.MaDK AND hs.TrangThai = 1", query);
            Assert.Contains("nl.MaDK IN (@maDK0, @maDK1, @maDK2)", query);
            Assert.Equal(3, parameters.ParameterNames.Count());
        }
    }
}
