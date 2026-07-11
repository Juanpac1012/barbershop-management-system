using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Factura
    {
        public Int64 idFactura { get; set; }
        public Usuario usuario { get; set; }
        public DateTime fecha { get; set; }
        public string tipo { get; set; } //?
        public List<Producto> productos { get; set; }
        public Cita cita { get; set; }
        //revisar
        public int cantidadProductos { get; set; }
        public decimal subtotal { get; set; }
        public decimal total { get; set; }

        //metodo de pago

    }
}
