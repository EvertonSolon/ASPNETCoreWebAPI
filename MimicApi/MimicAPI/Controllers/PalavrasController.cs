using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositorios.Interfaces;
using Newtonsoft.Json;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        //private readonly MimicContext _banco;
        private readonly IPalavraRepositorio _palavraRepositorio;

        public PalavrasController(IPalavraRepositorio palavraRepositorio)
        {
            _palavraRepositorio = palavraRepositorio;
        }

        //App
        //Rota -> site/api/palavras?data=2020-01-11 para cair no método abaixo.
        //Rota -> site/api/palavras?pagina=1 para cair no método abaixo.
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQueryString queryString)
        {
            

            var listaObjetos = _palavraRepositorio.ObterTodas(queryString);

            if (queryString.Pagina.Value > listaObjetos.Paginacao.TotalPaginas)
                return NotFound();

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(listaObjetos.Paginacao));

            return Ok(listaObjetos);
        }

        //Web
        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (id da palavra a ser buscada).
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var objeto = _palavraRepositorio.Obter(id);

            if (objeto != null)
                return Ok(objeto);

            return NotFound(); //Ou StatusCode(404);
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
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var objeto = _palavraRepositorio.Obter(palavra.Id);

            if (objeto == null)
                return NotFound();

            palavra.Id = id;
            _palavraRepositorio.Atualizar(palavra);

            return Ok(palavra);
        }

        [Route("{id}")]
        [HttpDelete]
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
