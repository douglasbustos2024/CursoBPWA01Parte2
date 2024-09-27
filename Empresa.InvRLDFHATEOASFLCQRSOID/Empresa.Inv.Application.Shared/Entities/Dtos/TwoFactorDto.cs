using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa.Inv.Application.Shared.Entities.Dtos
{
    public class TwoFactorDto
    {
        public string Username { get; set; }
        public string Code { get; set; }
    }
}
