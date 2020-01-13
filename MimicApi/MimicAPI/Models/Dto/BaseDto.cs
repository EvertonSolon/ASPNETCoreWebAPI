using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Models.Dto
{
    public abstract class BaseDto
    {
        public List<LinkDto> Links { get; set; }
    }
}
