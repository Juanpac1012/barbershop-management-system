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
        public DateTime fecha { get; set; }
        //public string tipo { get; set; }  // ¿Puede ser algo como 'Producto' o 'Cita'?
        public int cantidadProductos { get; set; }
        public Usuario usuario { get; set; }
        public List<Producto> productos { get; set; }  // Lista de productos
        public Cita cita { get; set; }  // Cita asociada a la factura
        public decimal total { get; set; }
        public decimal subtotal { get; set; }

        public string estado { get; set; }

    }
}
