using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Repositorios
{
    public class PalavraRepositorio : IPalavraRepositorio
    {
        private readonly MimicContext _contexto;

        public PalavraRepositorio(MimicContext contexto)
        {
            _contexto = contexto;
        }

        public void Atualizar(Palavra palavra)
        {
            _contexto.Palavras.Update(palavra);
            _contexto.SaveChanges();
        }

        public void Cadastrar(Palavra palavra)
        {
            _contexto.Palavras.Add(palavra);
            _contexto.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);

            palavra.Ativo = false;

            _contexto.Update(palavra);
            _contexto.SaveChanges();
        }

        public Palavra Obter(int id)
        {
            var objeto = _contexto.Palavras.AsNoTracking().FirstOrDefault(x => x.Id == id);
            return objeto;
        }

        public ListaPaginacao<Palavra> ObterTodas(PalavraUrlQueryString queryString)
        {
            var listaPaginacao = new ListaPaginacao<Palavra>();
            var item = _contexto.Palavras.AsNoTracking().AsQueryable();

            if (queryString.Data.HasValue)
                item = item.Where(x => x.Criado > queryString.Data.Value || x.Atualizado > queryString.Data.Value);

            if (queryString.Pagina.HasValue)
            {
                var qtdeTotalRegistros = item.Count();
                var qtdeRegistrosParaPular = (queryString.Pagina.Value - 1) * queryString.QtdeRegistros.Value;
                var qtdeTotalPaginas = (int)Math.Ceiling((double)qtdeTotalRegistros / queryString.QtdeRegistros.Value);

                item = item.Skip(qtdeRegistrosParaPular).Take(queryString.QtdeRegistros.Value);

                var paginacao = new Paginacao
                {
                    NumeroDaPagina = queryString.Pagina.Value,
                    RegistrosPorPagina = queryString.QtdeRegistros.Value,
                    TotalRegistros = qtdeTotalRegistros,
                    TotalPaginas = qtdeTotalPaginas
                };

                listaPaginacao.Paginacao = paginacao;
            }

            listaPaginacao.Palavras.AddRange(item.ToList());

            return listaPaginacao;
        }
    }
}
