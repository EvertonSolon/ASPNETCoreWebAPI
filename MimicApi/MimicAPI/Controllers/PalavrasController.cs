using Microsoft.AspNetCore.Mvc;
using MimicAPI.Database;
using MimicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        //Rota -> site/api/palavras para cair no método abaixo.
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas()
        {
            return Ok(_banco.Palavras);
        }

        //Web
        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (id da palavra a ser buscada).
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            return Ok(_banco.Palavras.Find(id));
        }

        //Rota -> site/api/palavras para cair no método abaixo (POST: id, nome, ativo, pontuação, data da criação).
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);

            return Ok();
        }

        //Rota -> site/api/palavras/1 para cair no método abaixo com o parâmetro 1 (PUT: id, nome, ativo, pontuação, data da criação).
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, Palavra palavra)
        {
            _banco.Palavras.Update(palavra);

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            _banco.Palavras.Remove(_banco.Palavras.Find(id));

            return Ok();
        }
    }
}
