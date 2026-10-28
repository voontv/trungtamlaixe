using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Ttlaixe.DriverApi
{
    /// <summary>
    /// Query string of GET /api/v1/drivers, here for the Swagger description only: the validator reads the raw
    /// query so it can reject malformed input the way the NestJS service does. Every filter is optional.
    /// </summary>
    public class DriversQueryRequest
    {
        [FromQuery(Name = "page")] public string Page { get; set; }
        [FromQuery(Name = "limit")] public string Limit { get; set; }
        [FromQuery(Name = "soHoSo")] public string SoHoSo { get; set; }
        [FromQuery(Name = "ttXuLy")] public string TtXuLy { get; set; }
        [FromQuery(Name = "maDK")] public string MaDK { get; set; }
        [FromQuery(Name = "hoVaTen")] public string HoVaTen { get; set; }
        [FromQuery(Name = "ngaySinh")] public string NgaySinh { get; set; }
        [FromQuery(Name = "hangGPLX")] public string HangGPLX { get; set; }
        [FromQuery(Name = "hangDaoTao")] public string HangDaoTao { get; set; }
        [FromQuery(Name = "tenTT")] public string TenTT { get; set; }
        [FromQuery(Name = "q")] public string Q { get; set; }
    }

    public class DriverVerifyRequest
    {
        [JsonPropertyName("soCMT")]
        public string SoCMT { get; set; }

        [JsonPropertyName("ngaySinh")]
        public string NgaySinh { get; set; }

        /// <summary>Unknown properties, kept only so they can be rejected.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public class DriverProfilesRequest
    {
        [JsonPropertyName("maDKs")]
        public List<string> MaDKs { get; set; }

        /// <summary>Unknown properties, kept only so they can be rejected.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }
}
