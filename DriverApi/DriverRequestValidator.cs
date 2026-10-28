using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Ttlaixe.DriverApi
{
    /// <summary>A validated GET /api/v1/drivers query.</summary>
    public class DriverSearchCriteria
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;

        /// <summary>Filter name (a <see cref="DriverQueries.Filters"/> key, or "q") to its value.</summary>
        public Dictionary<string, string> Filters { get; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Input rules of the driver endpoints. They mirror the NestJS service's ValidationPipe
    /// (whitelist + forbidNonWhitelisted): unknown fields and repeated query keys are errors.
    /// </summary>
    public static class DriverRequestValidator
    {
        public const int ProfilesMaxMaDK = 50;

        // [0-9] and \z, not \d and $: in .NET they would accept other scripts' digits and a trailing newline.
        private static readonly Regex Date8 = new Regex(@"^[0-9]{8}\z", RegexOptions.Compiled);
        private static readonly Regex HangGplxList = new Regex(@"^[^,\s]{1,3}(,[^,\s]{1,3}){0,9}\z", RegexOptions.Compiled);

        private static readonly HashSet<string> QueryKeys = new HashSet<string>(StringComparer.Ordinal)
        {
            "page", "limit", "soHoSo", "ttXuLy", "maDK", "hoVaTen", "ngaySinh", "hangGPLX", "hangDaoTao", "tenTT", "q",
        };

        public static DriverSearchCriteria ValidateSearch(IQueryCollection query, List<string> errors)
        {
            var criteria = new DriverSearchCriteria();

            foreach (var key in query.Keys)
            {
                if (!QueryKeys.Contains(key))
                {
                    errors.Add($"property {key} should not exist");
                }
                else if (query[key].Count > 1)
                {
                    errors.Add($"{key} must be a string");
                }
            }

            // Read the raw query: model binding turns an empty value into null, which must stay an error.
            string Raw(string name) => query.TryGetValue(name, out var values) ? values.ToString() : null;

            criteria.Page = ParseInteger("page", Raw("page"), 1, 21_474_837, 1, errors);
            criteria.Limit = ParseInteger("limit", Raw("limit"), 1, 100, 10, errors);

            AddText(criteria, "soHoSo", Raw("soHoSo"), 18, errors);
            AddText(criteria, "ttXuLy", Raw("ttXuLy"), 2, errors);
            AddText(criteria, "maDK", Raw("maDK"), 25, errors);
            AddText(criteria, "hoVaTen", Raw("hoVaTen"), 50, errors);
            AddPattern(criteria, "ngaySinh", Raw("ngaySinh"), Date8, errors);
            // One code of up to 3 characters, or up to 10 of them separated by commas.
            AddPattern(criteria, "hangGPLX", Raw("hangGPLX"), HangGplxList, errors);
            AddText(criteria, "hangDaoTao", Raw("hangDaoTao"), 20, errors);
            AddText(criteria, "tenTT", Raw("tenTT"), 50, errors);
            AddText(criteria, "q", Raw("q"), 50, errors);

            return criteria;
        }

        public static void ValidateVerify(DriverVerifyRequest request, List<string> errors)
        {
            AddExtra(request.Extra, errors);

            if (string.IsNullOrEmpty(request.SoCMT) || request.SoCMT.Length > 20)
            {
                errors.Add("soCMT must be a non-empty string of at most 20 characters");
            }

            if (request.NgaySinh == null || !Date8.IsMatch(request.NgaySinh))
            {
                errors.Add("ngaySinh must be in the format yyyyMMdd");
            }
        }

        public static void ValidateProfiles(DriverProfilesRequest request, List<string> errors)
        {
            AddExtra(request.Extra, errors);

            var maDKs = request.MaDKs;
            if (maDKs == null)
            {
                errors.Add($"maDKs must contain between 1 and {ProfilesMaxMaDK} MaDK");
                return;
            }

            if (maDKs.Count < 1 || maDKs.Count > ProfilesMaxMaDK)
            {
                errors.Add($"maDKs must contain between 1 and {ProfilesMaxMaDK} MaDK");
            }

            if (maDKs.Distinct(StringComparer.Ordinal).Count() != maDKs.Count)
            {
                errors.Add("maDKs's elements must be unique");
            }

            if (maDKs.Any(x => string.IsNullOrEmpty(x) || x.Length > 25))
            {
                errors.Add("each value in maDKs must be a non-empty string of at most 25 characters");
            }
        }

        private static void AddExtra(Dictionary<string, System.Text.Json.JsonElement> extra, List<string> errors)
        {
            if (extra == null)
            {
                return;
            }

            errors.AddRange(extra.Keys.Select(key => $"property {key} should not exist"));
        }

        private static int ParseInteger(
            string name, string raw, int min, int max, int defaultValue, List<string> errors)
        {
            if (raw == null)
            {
                return defaultValue;
            }

            if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
                || number != Math.Floor(number)
                || number < min
                || number > max)
            {
                errors.Add($"{name} must be an integer between {min} and {max}");
                return defaultValue;
            }

            return (int)number;
        }

        private static void AddText(
            DriverSearchCriteria criteria, string name, string value, int maxLength, List<string> errors)
        {
            if (value == null)
            {
                return;
            }

            if (value.Length == 0 || value.Length > maxLength)
            {
                errors.Add($"{name} must be a non-empty string of at most {maxLength} characters");
                return;
            }

            criteria.Filters[name] = value;
        }

        private static void AddPattern(
            DriverSearchCriteria criteria, string name, string value, Regex pattern, List<string> errors)
        {
            if (value == null)
            {
                return;
            }

            if (!pattern.IsMatch(value))
            {
                errors.Add($"{name} is not in the expected format");
                return;
            }

            criteria.Filters[name] = value;
        }
    }
}
