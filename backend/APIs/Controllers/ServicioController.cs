using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class ServicioController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/servicio/insertar")]
        public ResInsertarServicio insertarServicio(ReqInsertarServicio req)
        {
            return new LogServicio().insertar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/servicio/actualizar")]
        public ResActualizarServicio actualizarservicio(ReqActualizarServicio req)
        {
            return new LogServicio().actualizar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/servicio/eliminar")]
        public ResEliminarServicio eliminarservicio(ReqEliminarServicio req)
        {
            return new LogServicio().eliminar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/servicio/listar")]
        public ResListarSercicio listarServicio(ReqListarServicio req)
        {
            return new LogServicio().listar(req);
        }

    }
}