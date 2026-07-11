using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Usuario
    {
        public Int64 idUsuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string correoElectronico { get; set; }
        public string telefono { get; set; }
        public string contraseña { get; set; }
        public string numeroVerificacion {  get; set; }
        public EnumRoles idRol { get; set; }
    }
}
