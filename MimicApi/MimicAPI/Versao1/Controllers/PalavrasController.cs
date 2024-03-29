﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Versao1.Models;
using MimicAPI.Versao1.Models.Dto;
using MimicAPI.Versao1.Repositorios.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MimicAPI.Versao1.Controllers
{
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)] // /api/v1/palavras
    [ApiVersion("1.1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")] //passando a versão na queryString de forma diferente api/palavras?api-version=1
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepositorio _palavraRepositorio;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepositorio palavraRepositorio, IMapper mapper)
        {
            _palavraRepositorio = palavraRepositorio;
            _mapper = mapper;
        }

        /// <summary>
        /// Operação que pega do banco de dados todas as palavras existentes.
        /// </summary>
        /// <param name="queryString">Filtros de pesquisa</param>
        /// <returns>Listagem de palavras</returns>
        //App
        //Rota -> site/api/palavras?data=2020-01-11 para cair no método abaixo.
        //Rota -> site/api/palavras?pagina=1 para cair no método abaixo.
        [Route("", Name = "ObterTodas")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQueryString queryString)
        {
            var listaObjetos = _palavraRepositorio.ObterTodas(queryString);

            if (listaObjetos.Palavras.Count == 0)
                return NotFound();

            var listaPalavraDto = CriarListaLinksPalavraDto(queryString, listaObjetos);

            return Ok(listaPalavraDto);
        }

        /// <summary>
        /// Operação que pega uma única palavra da base de dados.
        /// </summary>
        /// <param name="id">Código identificador da palavra</param>
        /// <returns>Um objeto de palavra</returns>
        //Web
        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (id da palavra a ser buscada).
        [HttpGet("{id}", Name = "Obter")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        public ActionResult Obter(int id)
        {
            var objeto = _palavraRepositorio.Obter(id);

            if (objeto == null)
                return NotFound(); //Ou StatusCode(404);

            var palavraDto = _mapper.Map<Palavra, PalavraDto>(objeto);

            GerarListaLinkDto(palavraDto, VerbosHttpConstante.Get);

            return Ok(palavraDto);
        }

        
        /// <summary>
        /// Operação de realiza o cadastro da palavra
        /// </summary>
        /// <param name="palavra">Um objeto palavra</param>
        /// <returns>Um objeto palavra com seu Id</returns>
        //Rota -> site/api/palavras para cair no método abaixo (POST: id, nome, ativo, pontuação, data da criação).
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;

            _palavraRepositorio.Cadastrar(palavra);

            var palavraDto = _mapper.Map<Palavra, PalavraDto>(palavra);

            GerarListaLinkDto(palavraDto, VerbosHttpConstante.Get);

            return Created($"/api/palavras/{palavra.Id}", palavraDto);
        }

        /// <summary>
        /// Operação que realiza a substituição de dados de uma palavra específica.
        /// </summary>
        /// <param name="id">Código identificador da palavra a ser alterada</param>
        /// <param name="palavra">Objeto palavra com dados para alteração</param>
        /// <returns>Retorna o código do status Http</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (PUT: id, nome, ativo, pontuação, data da criação).
        [HttpPut("{id}", Name = "Atualizar")]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {

            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var objeto = _palavraRepositorio.Obter(palavra.Id);

            if (objeto == null)
                return NotFound();

            palavra.Id = id;
            palavra.Ativo = objeto.Ativo;
            palavra.Criado = objeto.Criado;
            palavra.Atualizado = DateTime.Now;

            _palavraRepositorio.Atualizar(palavra);

            var palavraDto = _mapper.Map<Palavra, PalavraDto>(palavra);

            GerarListaLinkDto(palavraDto, VerbosHttpConstante.Put);

            return Ok(palavraDto);
        }
        /// <summary>
        /// Operaçã que desativa uma palavra do sistema
        /// </summary>
        /// <param name="id">Código identifcador da palavra</param>
        /// <returns>Retorna o status do código Http</returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{id}", Name = "Deletar")]
        public ActionResult Deletar(int id)
        {
            var objeto = _palavraRepositorio.Obter(id);

            if (objeto == null)
                return NotFound();

            _palavraRepositorio.Deletar(id);

            return NoContent();
        }

        #region Métodos privados
        private void GerarListaLinkDto(PalavraDto palavraDto, string verboHttp)
        {
            switch (verboHttp)
            {
                case VerbosHttpConstante.Get:
                    palavraDto.Links.Add(new LinkDto("self", Url.Link("Obter", new { id = palavraDto.Id }), VerbosHttpConstante.Get));
                    break;
                case VerbosHttpConstante.Put:
                    palavraDto.Links.Add(new LinkDto("update", Url.Link("Atualizar", new { id = palavraDto.Id }), VerbosHttpConstante.Put));
                    break;
                case VerbosHttpConstante.Delete:
                    palavraDto.Links.Add(new LinkDto("delete", Url.Link("Deletar", new { id = palavraDto.Id }), VerbosHttpConstante.Delete));
                    break;
                default:
                    break;
            }
        }

        private ListaPaginacao<PalavraDto> CriarListaLinksPalavraDto(PalavraUrlQueryString queryString, ListaPaginacao<Palavra> listaObjetos)
        {
            var listaPalavraDto = _mapper.Map<ListaPaginacao<Palavra>, ListaPaginacao<PalavraDto>>(listaObjetos);

            foreach (var palavra in listaPalavraDto.Palavras)
            {
                palavra.Links = new List<LinkDto>
                {
                    new LinkDto("self", Url.Link("Obter", new { id = palavra.Id }), "GET")
                };
            }

            listaPalavraDto.ListaLinks.Add(
                new LinkDto("self", Url.Link("ObterTodas", queryString), "GET")
                );

            if (listaObjetos.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(listaObjetos.Paginacao));

                if (queryString.Pagina + 1 < listaObjetos.Paginacao.TotalPaginas)
                {
                    var novaQueryString = new PalavraUrlQueryString
                    {
                        Pagina = queryString.Pagina + 1,
                        QtdeRegistros = queryString.QtdeRegistros,
                        Data = queryString.Data
                    };

                    listaPalavraDto.ListaLinks.Add(
                    new LinkDto("proximaPagina", Url.Link("ObterTodas", novaQueryString), "GET")
                    );
                }

                if (queryString.Pagina - 1 > 0 || queryString.Pagina + 1 == listaObjetos.Paginacao.TotalPaginas)
                {
                    var novaQueryString = new PalavraUrlQueryString
                    {
                        Pagina = queryString.Pagina - 1,
                        QtdeRegistros = queryString.QtdeRegistros,
                        Data = queryString.Data
                    };

                    listaPalavraDto.ListaLinks.Add(
                    new LinkDto("paginaAnterior", Url.Link("ObterTodas", novaQueryString), "GET")
                    );
                }

            }

            return listaPalavraDto;
        }
        #endregion
    }
}
