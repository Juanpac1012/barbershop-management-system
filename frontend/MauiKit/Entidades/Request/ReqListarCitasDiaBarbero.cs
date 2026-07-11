using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ReqListarCitasDiaBarbero //: ReqBase?
    {
        public DateTime fechaActual { get; set; }
        public Int64 idBarbero { get; set; }
    }
}
