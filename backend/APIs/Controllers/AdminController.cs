using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class AdminController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/admin/insertar")]
        public ResInsertarAdmin insertarAdmin(ReqInsertarAdmin req)
        {
            return new LogAdmin().insertar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/admin/actualizar")]
        public ResActualizarAdmin actualizarAdmin(ReqActualizarAdmin req)
        {
            return new LogAdmin().actualizar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/admin/eliminar")]
        public ResEliminarAdmin eliminarAdmin(ReqEliminarAdmin req)
        {
            return new LogAdmin().eliminar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/admin/listar")]
        public ResListarAdmins listarAdmin(ReqListarAdmins req)
        {
            return new LogAdmin().listar(req);
        }

    }
}