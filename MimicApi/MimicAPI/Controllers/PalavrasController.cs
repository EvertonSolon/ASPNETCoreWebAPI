using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;

        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        //App
        //Rota -> site/api/palavras?data=2020-01-11 para cair no método abaixo.
        //Rota -> site/api/palavras?pagina=1 para cair no método abaixo.
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQueryString queryString)
        {
            var item = _banco.Palavras.AsQueryable();

            if (queryString.Data.HasValue)
                item = item.Where(x => x.Criado > queryString.Data.Value || x.Atualizado > queryString.Data.Value);

            if(queryString.Pagina.HasValue)
            {
                var qtdeTotalRegistros = item.Count();
                var qtdeRegistrosParaPular = (queryString.Pagina.Value - 1) * queryString.QtdeRegistros.Value;
                var qtdeTotalPaginas = (int)Math.Ceiling((double)qtdeTotalRegistros / queryString.QtdeRegistros.Value);

                item = item.Skip(qtdeRegistrosParaPular).Take(queryString.QtdeRegistros.Value);

                var paginacao = new Paginacao
                {
                    NumeroPagina = queryString.Pagina.Value,
                    RegistroPorPagina = queryString.QtdeRegistros.Value,
                    TotalRegistros = qtdeTotalRegistros,
                    TotalPaginas = qtdeTotalPaginas
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if (queryString.Pagina.Value > paginacao.TotalPaginas)
                    return NotFound();
            }



            return Ok(item);
        }

        //Web
        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (id da palavra a ser buscada).
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var objeto = _banco.Palavras.Find(id);

            if (objeto != null)
                return Ok(objeto);

            return NotFound(); //Ou StatusCode(404);
        }

        //Rota -> site/api/palavras para cair no método abaixo (POST: id, nome, ativo, pontuação, data da criação).
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();

            //return Ok(palavra);
            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (PUT: id, nome, ativo, pontuação, data da criação).
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var objeto = _banco.Palavras.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (objeto == null)
                return NotFound();

            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok(palavra);
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);

            if (palavra == null)
                return NotFound();

            //_banco.Palavras.Remove(obj);
            palavra.Ativo = false;
            _banco.Update(palavra);
            _banco.SaveChanges();

            return NoContent();
        }
    }
}
