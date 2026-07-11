using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class Producto
    {
       public int idProducto {  get; set; }
       public string nombre { get; set; }   
       public string descripcion { get; set; }
       public decimal precio { get; set; } 
       public int stock { get; set; }
       public byte[] Img { get; set; } //revisar

       public int cantidad { get; set; } //revisar

    }
}
