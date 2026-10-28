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
    public class DirectPhotoPathResolverTests
    {
        private readonly DirectPhotoPathResolver _resolver = new DirectPhotoPathResolver();

        [Fact]
        public void Reads_an_absolute_path_as_stored_ignoring_surrounding_whitespace()
        {
            var file = Path.Combine(Path.GetTempPath(), "2026", "x.jp2");

            Assert.Equal(file, _resolver.Resolve("  " + file + " "));
        }

        [Fact]
        public void Reads_a_UNC_share_path_on_Windows()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            const string stored = @"\\192.168.100.248\B\2026\im_gplx\48012K26B0100\48012-20260304-103333.jp2";

            Assert.Equal(stored, _resolver.Resolve(stored));
        }

        [Theory]
        [InlineData("")]
        [InlineData("2026/x.jp2")]
        [InlineData("x.jp2")]
        public void Refuses_a_path_that_is_not_absolute(string stored)
        {
            Assert.Null(_resolver.Resolve(stored));
        }

        [Fact]
        public void Refuses_a_path_that_climbs_out_of_its_folder()
        {
            var escaping = Path.Combine(Path.GetTempPath(), "2026", "..", "..", "etc", "passwd");

            Assert.Null(_resolver.Resolve(escaping));
        }

        [Fact]
        public void Gives_no_photo_when_the_path_is_refused_or_the_file_is_missing()
        {
            var service = new DriverPhotoService(_resolver);

            Assert.Null(service.LoadAsync(null).Result);
            Assert.Null(service.LoadAsync("x.jp2").Result);
            Assert.Null(service.LoadAsync(Path.Combine(Path.GetTempPath(), "missing-photo.jp2")).Result);
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
