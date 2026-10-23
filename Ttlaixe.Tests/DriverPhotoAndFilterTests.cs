using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Ttlaixe.DriverApi;
using Xunit;

namespace Ttlaixe.Tests
{
    public class DriverPhotoServiceTests
    {
        private static readonly string Mount = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "photos"));

        [Fact]
        public void MapPhotoPath_maps_a_share_path_below_the_mount_directory()
        {
            var mapped = DriverPhotoService.MapPhotoPath(@"\\192.168.100.248\B\2026\x.jp2", @"\\192.168.100.248\B", Mount);

            Assert.Equal(Path.Combine(Mount, "2026", "x.jp2"), mapped);
        }

        [Fact]
        public void MapPhotoPath_ignores_case_and_surrounding_whitespace()
        {
            var mapped = DriverPhotoService.MapPhotoPath(@"  \\SERVER\b\x.jp2 ", @"\\server\B\", Mount);

            Assert.Equal(Path.Combine(Mount, "x.jp2"), mapped);
        }

        [Theory]
        [InlineData(@"\\other\B\x.jp2")]
        [InlineData(@"\\192.168.100.248\Bogus\x.jp2")]
        [InlineData(@"\\192.168.100.248\B")]
        [InlineData(@"\\192.168.100.248\B\..\..\etc\passwd")]
        [InlineData(@"\\192.168.100.248\B\")]
        public void MapPhotoPath_refuses_paths_outside_the_share(string stored)
        {
            Assert.Null(DriverPhotoService.MapPhotoPath(stored, @"\\192.168.100.248\B", Mount));
        }

        [Fact]
        public void Locate_uses_the_first_matching_map_and_unreadable_files_give_no_photo()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Photo:PathMaps"] = $@"\\a\B={Mount}/b,\\a\d={Mount}/d",
                })
                .Build();
            var service = new DriverPhotoService(config);

            Assert.Null(service.LoadAsync(null).Result);
            Assert.Null(service.LoadAsync(@"\\nowhere\x.jp2").Result);
            Assert.Null(service.LoadAsync(@"\\a\d\missing.jp2").Result);
        }
    }

    public class DriverApiIpFilterTests
    {
        private static int? Status(string allowed, string realIp, string remote)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { ["DriverApi:AllowedIps"] = allowed })
                .Build();
            var http = new DefaultHttpContext();
            if (realIp != null)
            {
                http.Request.Headers["X-Real-IP"] = realIp;
            }

            http.Connection.RemoteIpAddress = remote == null ? null : IPAddress.Parse(remote);
            var context = new AuthorizationFilterContext(
                new ActionContext(http, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());

            new DriverApiIpFilter(config).OnAuthorization(context);

            return (context.Result as StatusCodeResult)?.StatusCode;
        }

        [Fact]
        public void Allows_a_listed_X_Real_IP()
        {
            Assert.Null(Status("1.1.1.1; 2.2.2.2", "2.2.2.2", "10.0.0.9"));
        }

        [Fact]
        public void X_Real_IP_wins_over_the_connection_address()
        {
            Assert.Equal(403, Status("10.0.0.9", "2.2.2.2", "10.0.0.9"));
        }

        [Fact]
        public void Falls_back_to_the_remote_address_including_IPv4_mapped_ones()
        {
            Assert.Null(Status("10.0.0.9", null, "::ffff:10.0.0.9"));
        }

        [Theory]
        [InlineData("", "1.1.1.1")]
        [InlineData(null, "1.1.1.1")]
        [InlineData(";", "1.1.1.1")]
        [InlineData("1.1.1.1", "1.1.1.10")]
        [InlineData("1.1.1.1", null)]
        public void Rejects_everyone_else_and_fails_closed(string allowed, string realIp)
        {
            Assert.Equal(403, Status(allowed, realIp, null));
        }
    }

    public class IsoUtcDateTimeConverterTests
    {
        private class Holder
        {
            [System.Text.Json.Serialization.JsonConverter(typeof(IsoUtcDateTimeConverter))]
            public DateTime? At { get; set; }
        }

        [Fact]
        public void Writes_milliseconds_and_a_trailing_Z_like_JavaScript()
        {
            var json = JsonSerializer.Serialize(new Holder { At = new DateTime(2026, 3, 4, 9, 30, 15, 120) });

            Assert.Equal(@"{""At"":""2026-03-04T09:30:15.120Z""}", json);
        }

        [Fact]
        public void Writes_null()
        {
            Assert.Equal(@"{""At"":null}", JsonSerializer.Serialize(new Holder()));
        }
    }
}
