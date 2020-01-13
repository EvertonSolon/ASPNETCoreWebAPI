using MimicAPI.Models;
using MimicAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class ListaPaginacao<T>
    {
        public List<T> Palavras { get; set; } = new List<T>();

        public Paginacao Paginacao { get; set; }

        public List<LinkDto> ListaLinks { get; set; } = new List<LinkDto>();
    }
}
