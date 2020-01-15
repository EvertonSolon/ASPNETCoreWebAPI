using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Models.Dto;
using MimicAPI.Repositorios.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
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
        public ActionResult ObterTodas([FromQuery]PalavraUrlQueryString queryString)
        {
            var listaObjetos = _palavraRepositorio.ObterTodas(queryString);

            if (listaObjetos.Palavras.Count == 0)
                return NotFound();

            var listaPalavraDto = CriarListaLinksPalavraDto(queryString, listaObjetos);

            return Ok(listaPalavraDto);
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

        //Web
        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (id da palavra a ser buscada).
        [HttpGet("{id}", Name = "Obter")]
        public ActionResult Obter(int id)
        {
            var objeto = _palavraRepositorio.Obter(id);

            if (objeto == null)
                return NotFound(); //Ou StatusCode(404);

            var palavraDto = _mapper.Map<Palavra, PalavraDto>(objeto);
            palavraDto.Links = new List<LinkDto>
            {
                new LinkDto("self", Url.Link("Obter", new { id = palavraDto.Id }), "GET"),
                new LinkDto("self", Url.Link("Atualizar", new { id = palavraDto.Id }), "PUT"),
                new LinkDto("self", Url.Link("Deletar", new { id = palavraDto.Id }), "DELETE")
            };

            return Ok(palavraDto);
        }

        //Rota -> site/api/palavras para cair no método abaixo (POST: id, nome, ativo, pontuação, data da criação).
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _palavraRepositorio.Cadastrar(palavra);

            //return Ok(palavra);
            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (PUT: id, nome, ativo, pontuação, data da criação).
        [HttpPut("{id}", Name = "Atualizar")]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var objeto = _palavraRepositorio.Obter(palavra.Id);

            if (objeto == null)
                return NotFound();

            palavra.Id = id;
            _palavraRepositorio.Atualizar(palavra);

            return Ok(palavra);
        }

        [HttpDelete("{id}", Name = "Deletar")]
        public ActionResult Deletar(int id)
        {
            var objeto = _palavraRepositorio.Obter(id);

            if (objeto == null)
                return NotFound();

            _palavraRepositorio.Deletar(id);

            return NoContent();
        }
    }
}
