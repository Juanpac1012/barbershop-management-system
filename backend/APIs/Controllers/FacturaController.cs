using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class FacturaController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/factura/insertarProductos")]
        public ResInsertarFacturaProductos insertarFacturaProductos(ReqInsertarFacturaProductos req)
        {
            return new LogFactura().insertarConProductos(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/factura/listarFacturasPUsuario")]
        public ResListarFacturasPUsuario listarFacturasPUsuario(ReqListarFacturasPUsuario req)
        {
            return new LogFactura().listarFacturasPUsuario(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/factura/insertarCita")]
        public ResInsertarFacturaCita insertarFacturaCita(ReqInsertarFacturaCita req)
        {
            return new LogFactura().insertarConCita(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/factura/listarFacturasCUsuario")]
        public ResListarFacturasCUsuario listarFacturasCUsuario(ReqListarFacturasCUsuario req)
        {
            return new LogFactura().listarFacturasCUsuario(req);
        }
    }
}