using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Versao1.Models;
using MimicAPI.Versao1.Models.Dto;
using MimicAPI.Versao1.Repositorios.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MimicAPI.Versao2.Controllers
{
    [ApiController]
    [ApiVersion("2.0")] // /api/v1/palavras
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")] // api/palavras? api-version=2
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepositorio _palavraRepositorio;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepositorio palavraRepositorio, IMapper mapper)
        {
            _palavraRepositorio = palavraRepositorio;
            _mapper = mapper;
        }

        //App
        //Rota -> site/api/palavras?data=2020-01-11 para cair no método abaixo.
        //Rota -> site/api/palavras?pagina=1 para cair no método abaixo.
        [Route("", Name = "ObterTodas")]
        [HttpGet]
        public string ObterTodas([FromQuery]PalavraUrlQueryString queryString)
        {
            return "Versão 2.0";
        }
    }
}
