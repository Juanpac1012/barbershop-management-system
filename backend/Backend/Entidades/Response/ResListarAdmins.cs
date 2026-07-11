using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public class ResListarAdmins : ResBase
    {
        public List<Usuario> admins { get; set; }
    }
}
