using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ResListarProducto : ResBase
    {
        public List<Producto> producto { get; set; }
    }
}
