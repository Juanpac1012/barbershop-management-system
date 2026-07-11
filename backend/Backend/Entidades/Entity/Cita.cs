using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Cita
    {
        public Int64 idCita { get; set; }
        public Usuario usuario { get; set; } //id
        public Usuario barbero { get; set; }//id
        public Servicio servicio { get; set; }//id
        public DateTime fechaHora { get; set; }
        public string estado { get; set; }
    }
}
