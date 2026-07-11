using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ResListarTodasCitas : ResBase
    {
        public List<Cita> cita { get; set; }
    }
}
