using AutoMapper;
using MimicAPI.Versao1.Models;
using MimicAPI.Versao1.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class DtoAutoMapperProfile : Profile
    {
        public DtoAutoMapperProfile()
        {
            CreateMap<Palavra, PalavraDto>();
            CreateMap<ListaPaginacao<Palavra>, ListaPaginacao<PalavraDto>>();
        }
        
    }
}
