using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Ttlaixe.DriverApi;
using Xunit;

namespace Ttlaixe.Tests
{
    public class DriverRequestValidatorTests
    {
        private static (DriverSearchCriteria Criteria, List<string> Errors) Search(params (string Key, string Value)[] pairs)
        {
            var query = new Dictionary<string, StringValues>();
            foreach (var (key, value) in pairs)
            {
                query[key] = query.TryGetValue(key, out var existing)
                    ? StringValues.Concat(existing, value)
                    : new StringValues(value);
            }

            var errors = new List<string>();
            var criteria = DriverRequestValidator.ValidateSearch(new QueryCollection(query), errors);
            return (criteria, errors);
        }

        [Fact]
        public void Search_without_parameters_uses_the_defaults()
        {
            var (criteria, errors) = Search();

            Assert.Empty(errors);
            Assert.Equal(1, criteria.Page);
            Assert.Equal(10, criteria.Limit);
            Assert.Empty(criteria.Filters);
        }

        [Theory]
        [InlineData("page", "0")]
        [InlineData("page", "21474838")]
        [InlineData("page", "abc")]
        [InlineData("page", "")]
        [InlineData("page", "1.5")]
        [InlineData("limit", "0")]
        [InlineData("limit", "101")]
        [InlineData("maDK", "")]
        [InlineData("maDK", "12345678901234567890123456")]
        [InlineData("soHoSo", "1234567890123456789")]
        [InlineData("ttXuLy", "123")]
        [InlineData("ngaySinh", "1234567")]
        [InlineData("ngaySinh", "1234567a")]
        [InlineData("ngaySinh", "٢٠٠٤٠٥١٩")]
        [InlineData("hangGPLX", "")]
        [InlineData("hangGPLX", "ABCD")]
        [InlineData("hangGPLX", "B1,,B11")]
        [InlineData("hangGPLX", "B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,B11")]
        [InlineData("q", "123456789012345678901234567890123456789012345678901")]
        [InlineData("unknown", "1")]
        [InlineData("MaDK", "x")]
        public void Search_rejects(string key, string value)
        {
            var (_, errors) = Search((key, value));

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void Search_rejects_a_repeated_key()
        {
            var (_, errors) = Search(("maDK", "a"), ("maDK", "b"));

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void Search_accepts_every_filter()
        {
            var (criteria, errors) = Search(
                ("page", "2"), ("limit", "100"), ("soHoSo", "004"), ("ttXuLy", "03"), ("maDK", "DK-1"),
                ("hoVaTen", "NGUYỄN"), ("ngaySinh", "20040519"), ("hangGPLX", "B1,B11"),
                ("hangDaoTao", "B.01"), ("tenTT", "x"), ("q", "nguyen"));

            Assert.Empty(errors);
            Assert.Equal(2, criteria.Page);
            Assert.Equal(100, criteria.Limit);
            Assert.Equal(9, criteria.Filters.Count);
            Assert.Equal("B1,B11", criteria.Filters["hangGPLX"]);
        }

        [Fact]
        public void Verify_requires_both_credentials()
        {
            var errors = new List<string>();
            DriverRequestValidator.ValidateVerify(new DriverVerifyRequest(), errors);

            Assert.Equal(2, errors.Count);
        }

        [Theory]
        [InlineData("012345678901", "20021110", true)]
        [InlineData("", "20021110", false)]
        [InlineData("123456789012345678901", "20021110", false)]
        [InlineData("1", "2002111", false)]
        [InlineData("1", "20021110\n", false)]
        public void Verify_validates_the_credentials(string soCmt, string ngaySinh, bool valid)
        {
            var errors = new List<string>();
            DriverRequestValidator.ValidateVerify(new DriverVerifyRequest { SoCMT = soCmt, NgaySinh = ngaySinh }, errors);

            Assert.Equal(valid, errors.Count == 0);
        }

        [Fact]
        public void Verify_rejects_unknown_properties()
        {
            var request = JsonSerializer.Deserialize<DriverVerifyRequest>(@"{""soCMT"":""1"",""ngaySinh"":""20021110"",""x"":1}");
            var errors = new List<string>();
            DriverRequestValidator.ValidateVerify(request, errors);

            Assert.Contains("property x should not exist", errors);
        }

        [Fact]
        public void Profiles_accepts_one_to_fifty_unique_codes()
        {
            var errors = new List<string>();
            DriverRequestValidator.ValidateProfiles(new DriverProfilesRequest { MaDKs = new List<string> { "a", "b" } }, errors);

            Assert.Empty(errors);
        }

        [Theory]
        [MemberData(nameof(InvalidProfiles))]
        public void Profiles_rejects(List<string> maDKs)
        {
            var errors = new List<string>();
            DriverRequestValidator.ValidateProfiles(new DriverProfilesRequest { MaDKs = maDKs }, errors);

            Assert.NotEmpty(errors);
        }

        public static IEnumerable<object[]> InvalidProfiles()
        {
            yield return new object[] { null };
            yield return new object[] { new List<string>() };
            yield return new object[] { new List<string> { "a", "a" } };
            yield return new object[] { new List<string> { "" } };
            yield return new object[] { new List<string> { null } };
            yield return new object[] { new List<string> { new string('x', 26) } };
            var tooMany = new List<string>();
            for (var i = 0; i < 51; i++)
            {
                tooMany.Add("m" + i);
            }

            yield return new object[] { tooMany };
        }
    }
}
