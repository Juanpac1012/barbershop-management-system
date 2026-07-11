using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Sesion
    {
        public Int64 idSesion {  get; set; } 
        public string sesion { get; set; }
        public Usuario usuario { get; set; }
        public string origen {  get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFinal { get; set; }
        public EnumEstadoSesion estado {  get; set; }
        public DateTime fechaActualizacion { get; set; }

        //enumRoles pero ver como lo manejamos desde base de datos y ya lo trae el usuario tambien
    }
}
