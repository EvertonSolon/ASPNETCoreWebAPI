using MimicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimicAPI.Helpers;

namespace MimicAPI.Repositorios.Interfaces
{
    public interface IPalavraRepositorio
    {
        ListaPaginacao<Palavra> ObterTodas(PalavraUrlQueryString queryString);

        Palavra Obter(int id);

        void Cadastrar(Palavra palavra);

        void Atualizar(Palavra palavra);

        void Deletar(int id);
    }
}
