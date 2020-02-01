using AdvertApi.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiMapperProfile : Profile
    {
        public AdvertApiMapperProfile()
        {
            CreateMap<AdvertModel, CreateAdvertWebRequest>().ReverseMap();
            CreateMap<CreateAdvertResponse, CreateAdvertWebResponse>().ReverseMap();
            CreateMap<ConfirmAdvertWebRequest, ConfirmAdvertModel>().ReverseMap();
        }
    }
}
