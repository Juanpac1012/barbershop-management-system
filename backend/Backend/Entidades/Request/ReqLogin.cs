using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ReqLogin
    {
        //revisar y lo de la sesion
        public string correoElectronico { get; set; }
        public string contraseña { get; set; }

    }
}
