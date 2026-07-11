using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class UsuarioController : ApiController
    {

        //INVESTIGAR: Recibir y retornar HTTP

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuarioCliente/insertar")]
        public ResInsertarUsuario ingresarUsuario(ReqInsertarUsuario req)
        {
            return new LogUsuario().insertar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuarioCliente/actualizar")]
        public ResActualizarUsuario actualizarUsuario(ReqActualizarUsuario req)
        {
            return new LogUsuario().actualizar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuarioCliente/eliminar")]
        public ResEliminarUsuario eliminarUsuario(ReqEliminarUsuario req)
        {
            return new LogUsuario().eliminar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuario/activarCuenta")]
        public ResActivarCuenta activarCuenta(ReqActivarCuenta req)
        {
            return new LogUsuario().activarCuenta(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuario/login")]
        public ResLogin login(ReqLogin req)
        {
            return new LogUsuario().login(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuario/cambiarContra")]
        public ResCambiarContraseña cambiarContraseña(ReqCambiarContraseña req)
        {
            return new LogUsuario().cambiarContraseña(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuario/solicitarCodigo")]
        public ResSolicitarCodigo solicitarCodigo(ReqSolicitarCodigo req)
        {
            return new LogUsuario().solicitarCodigo(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuario/obtener")]
        public ResObtenerUsuario obtener(ReqObtenerUsuario req)
        {
            return new LogUsuario().obtenerUsuario(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/usuario/codigoCambioContra")]
        public ResCodigoCambioContra codigoCambioContra(ReqCodigoCambioContra req)
        {
            return new LogUsuario().CodigoCambioContra(req);
        }

    }
}