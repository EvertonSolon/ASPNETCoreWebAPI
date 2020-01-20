using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Versao1.Models
{
    /// <summary>
    /// As validações foram incluídas nesta entidade mas sei que poderia existir uma view model e incluir lá.
    /// </summary>
    public class Palavra
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        public int Pontuacao { get; set; }

        public bool Ativo { get; set; }
        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}
