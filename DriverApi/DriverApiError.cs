using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ttlaixe.DriverApi
{
    /// <summary>400 body of the driver endpoints, shaped like the NestJS service's validation errors.</summary>
    public class DriverApiError
    {
        public DriverApiError(List<string> messages)
        {
            Message = messages;
        }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; } = 400;

        [JsonPropertyName("message")]
        public List<string> Message { get; }

        [JsonPropertyName("error")]
        public string Error { get; } = "Bad Request";
    }
}
