using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class BarberoController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/barbero/insertar")]
        public ResInsertarBarbero insertarBarbero(ReqInsertarBarbero req)
        {
            return new LogBarbero().insertar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/barbero/actualizar")]
        public ResActualizarBarbero actualizarBarbero(ReqActualizarBarbero req)
        {
            return new LogBarbero().actualizar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/barbero/eliminar")]
        public ResEliminarBarbero eliminarBarbero(ReqEliminarBarbero req)
        {
            return new LogBarbero().eliminar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/barbero/listar")]
        public ResListarBarberos listarBarberos(ReqListarBarberos req)
        {
            return new LogBarbero().listar(req);
        }
    }
}