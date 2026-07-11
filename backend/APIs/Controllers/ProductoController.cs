using Backend.Entidades;
using Backend.Logica;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class ProductoController : ApiController
    {
            
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/producto/insertar")]
        public ResInsertarProducto insertarProducto(ReqInsertarProducto req)
        {
            return new LogProducto().insertar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/producto/actualizar")]
        public ResActualizarProducto actualizarProducto(ReqActualizarProducto req)
        {
            return new LogProducto().actualizar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/producto/eliminar")]
        public ResEliminarProducto eliminarProducto(ReqEliminarProducto req)
        {
            return new LogProducto().eliminar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/producto/listar")]
        public ResListarProducto listarProducto(ReqListarProducto req)
        {
            return new LogProducto().listar(req);
        }
    }
}