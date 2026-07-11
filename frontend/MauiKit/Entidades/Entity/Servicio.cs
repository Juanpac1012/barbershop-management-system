using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Servicio
    {
        public int idServicio{ get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int duracion_minutos { get; set; }
        public decimal precio { get; set; }
        public byte[] Img { get; set; } //revisar

    }
}
