using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIs.Controllers
{
    public class CitaController : ApiController
    {

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/insertar")]
        public ResInsertarCita insertarCita(ReqInsertarCita req)
        {
            return new LogCita().insertar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/actualizar")]
        public ResActualizarCita actualizarCita(ReqActualizarCita req)
        {
            return new LogCita().actualizar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/eliminar")]
        public ResEliminarCita eliminarCita(ReqEliminarCita req)
        {
            return new LogCita().eliminar(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/listarCitasUsuario")]
        public ResListarCitasUsuario listarCitasUsuario(ReqListarCitasUsuario req)
        {
            return new LogCita().listarCitasUsuario(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/listarCitasBarbero")]
        public ResListarCitasBarbero listarCitasBarbero(ReqListarCitasBarbero req)
        {
            return new LogCita().listarCitasBarbero(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/listarCitasDia")]
        public ResListarCitasDia listarCitasDia(ReqListarCitasDia req)
        {
            return new LogCita().listarCitasDia(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/listarCitasDiaBarbero")]
        public ResListarCitasDiaBarbero listarCitasDiaBarbero(ReqListarCitasDiaBarbero req)
        {
            return new LogCita().listarCitasDiaBarbero(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/citaTeminada")]
        public ResCitaTerminada citaTerminada(ReqCitaTerminada req)
        {
            return new LogCita().citaTerminada(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/citaNoAsistio")]
        public ResCitaNoAsistio CitaNoAsistio(ReqCitaNoAsistio req)
        {
            return new LogCita().citaNoAsistio(req);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/cita/todasCitas")]
        public ResListarTodasCitas todasCitas(ReqListarTodasCitas req)
        {
            return new LogCita().listarTodasCitas(req);
        }
    }
}