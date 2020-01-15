using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Versao1.Models.Dto
{
    public class LinkDto
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Method { get; set; }

        public LinkDto(string rel, string hRef, string method)
        {
            Rel = rel;
            Href = hRef;
            Method = method;
        }
    }
}
