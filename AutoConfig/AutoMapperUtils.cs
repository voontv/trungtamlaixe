using AutoMapper;
using Ttlaixe.DTO.request;
using Ttlaixe.DTO.response;
using Ttlaixe.Models;
using System;
using System.Linq;

namespace Ttlaixe.AutoConfig
{
    public static class AutoMapperUtils
    {
        private static void ScanAssembly(IProfileExpression mapperConfigurationExpression)
        {
            var classes = from @class in typeof(AutoMapperUtils).Assembly.GetTypes()
                          where @class.IsClass && !@class.IsAbstract
                          select @class;

            foreach (var @class in classes)
            {
                foreach (ConvertToAttribute attr in @class.GetCustomAttributes(typeof(ConvertToAttribute), false))
                {
                    mapperConfigurationExpression.CreateMap(@class, attr.ToType);
                }

                foreach (ConvertFromAttribute attr in @class.GetCustomAttributes(typeof(ConvertFromAttribute), false))
                {
                    mapperConfigurationExpression.CreateMap(attr.FromType, @class);
                }
            }
        }

        private static void Config(IMapperConfigurationExpression mapperConfigurationExpression)
        {
            ScanAssembly(mapperConfigurationExpression);
        }

        private static readonly MapperConfiguration mapperConfiguration = new MapperConfiguration(x =>
        {
            x.CreateMap<KhoaHocResponse, KhoaHoc>();
            x.CreateMap<KhoaHoc, KhoaHocResponse>();
            x.CreateMap<KhoaHocCreateRequest, KhoaHoc>();
            x.CreateMap<KhoaHoc, KhoaHocCreateRequest>();
            x.CreateMap<DmDvhcResponse, DmDvhc>();
            x.CreateMap<DmDvhc, DmDvhcResponse>();
            x.CreateMap<NguoiLxCreateRequest, NguoiLx>();
            x.CreateMap<NguoiLx, NguoiLxCreateRequest>();
            x.CreateMap<NguoiLxResponse, NguoiLxCreateRequest>();
            x.CreateMap<NguoiLxCreateRequest, NguoiLxResponse>();
            
        });

        private static readonly IMapper mapper = mapperConfiguration.CreateMapper();

        public static T ConvertTo<T>(this object source)
        {
            return source switch
            {
                null => throw new Exception("Nullpoint exception occurs!"),
                T objT => objT,
                _ => mapper.Map<T>(source)
            };
        }

        public static void Patch<TF, TT>(this TF source, TT to)
        {
            if (source == null || to == null)
            {
                throw new Exception("Nullpoint exception occurs!");
            }

            mapper.Map(source, to);
        }
    }
}