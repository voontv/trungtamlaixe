using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ttlaixe.DriverApi
{
    /// <summary>Writes a timestamp like JavaScript's Date#toJSON (UTC, milliseconds, trailing Z).</summary>
    public class IsoUtcDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : reader.GetDateTime();

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var utc = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
            writer.WriteStringValue(utc.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture));
        }
    }

    public class DriverRecord
    {
        [JsonPropertyName("soHoSo")] public string SoHoSo { get; set; }
        [JsonPropertyName("ttXuLy")] public string TtXuLy { get; set; }
        [JsonPropertyName("maDK")] public string MaDK { get; set; }
        [JsonPropertyName("hoVaTen")] public string HoVaTen { get; set; }
        [JsonPropertyName("ngaySinh")] public string NgaySinh { get; set; }
        [JsonPropertyName("hangGPLX")] public string HangGPLX { get; set; }
        [JsonPropertyName("hangDaoTao")] public string HangDaoTao { get; set; }

        /// <summary>The date the file was received, as yyyy-MM-dd.</summary>
        [JsonPropertyName("ngayNhanHSo")] public string NgayNhanHSo { get; set; }

        [JsonPropertyName("tenTT")] public string TenTT { get; set; }
    }

    public class DriverPageResponse
    {
        [JsonPropertyName("data")] public List<DriverRecord> Data { get; set; }
        [JsonPropertyName("page")] public int Page { get; set; }
        [JsonPropertyName("limit")] public int Limit { get; set; }
        [JsonPropertyName("total")] public long Total { get; set; }
        [JsonPropertyName("totalPages")] public long TotalPages { get; set; }
    }

    public class VerifiedDriver
    {
        [JsonPropertyName("soHoSo")] public string SoHoSo { get; set; }
        [JsonPropertyName("ttXuLy")] public string TtXuLy { get; set; }
        [JsonPropertyName("maDK")] public string MaDK { get; set; }
        [JsonPropertyName("hoVaTen")] public string HoVaTen { get; set; }
        [JsonPropertyName("ngaySinh")] public string NgaySinh { get; set; }
        [JsonPropertyName("hangGPLX")] public string HangGPLX { get; set; }
        [JsonPropertyName("hangDaoTao")] public string HangDaoTao { get; set; }
        [JsonPropertyName("tenTT")] public string TenTT { get; set; }

        [JsonPropertyName("ngayTao")]
        [JsonConverter(typeof(IsoUtcDateTimeConverter))]
        public DateTime? NgayTao { get; set; }
    }

    public class DriverVerifyResponse
    {
        [JsonPropertyName("data")] public List<VerifiedDriver> Data { get; set; }
    }

    public class DriverProfile
    {
        [JsonPropertyName("maDK")] public string MaDK { get; set; }

        /// <summary>yyyyMMdd</summary>
        [JsonPropertyName("ngaySinh")] public string NgaySinh { get; set; }

        [JsonPropertyName("soCMT")] public string SoCMT { get; set; }

        /// <summary>Portrait resized for the exam report as base64 JPEG; null when there is no usable photo.</summary>
        [JsonPropertyName("anhBase64")] public string AnhBase64 { get; set; }
    }

    public class DriverProfilesResponse
    {
        [JsonPropertyName("data")] public List<DriverProfile> Data { get; set; }
    }
}
