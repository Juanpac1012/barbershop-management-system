using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ReqActivarCuenta //: ReqBase?
    {
        public string numeroVerificacion { get; set; }
        public string correoElectronico { get; set; }

    }
}
