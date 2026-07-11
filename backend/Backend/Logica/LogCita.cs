using AccesoDatos;
using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class LogCita
    {
        public ResInsertarCita insertar(ReqInsertarCita req)
        {
            ResInsertarCita res = new ResInsertarCita();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                res.listaErrores = ValiCita.insertar(req);
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    SP_INSERTAR_CITAResult tc = new SP_INSERTAR_CITAResult();

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        var resultado = linq.SP_INSERTAR_CITA(req.cita.usuario.idUsuario, req.cita.barbero.idUsuario, req.cita.servicio.idServicio,req.cita.fechaHora, ref idReturn, ref errorIdBD, ref errorMsgBD).ToList();
                        if (resultado.Any())
                        {
                            tc = resultado.First();
                        }
                    }
                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        bool correoEnviado = Utilitarios.EnviarCorreoConfirmacionCita(tc.Usuario_Correo, tc.Usuario_Nombre, tc.Fecha_Hora,tc.Servicio_Nombre );
                        if (!correoEnviado)
                        {
                           error.ErrorCode = EnumErrores.correoNoEnviado;
                           error.Message = errorMsgBD;
                           res.listaErrores.Add(error);
                        }
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practica
            }
            return res;
        }

        public ResActualizarCita actualizar(ReqActualizarCita req)
        {
            ResActualizarCita res = new ResActualizarCita();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                res.listaErrores = ValiCita.actualizar(req);
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        //req.sesion.usuario....
                        linq.SP_ACTUALIZAR_CITA(req.cita.idCita,req.cita.usuario.idUsuario, req.cita.barbero.idUsuario, req.cita.servicio.idServicio, req.cita.fechaHora, "pendiente", ref idReturn, ref errorIdBD, ref errorMsgBD);

                    }
                    if (idReturn >= 1) //REVISAR
                    {
                        res.resultado = true;
                        //cuando se maneje sesiones que mande el correo 
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practica
            }
            return res;
        }

        public ResEliminarCita eliminar(ReqEliminarCita req)
        {
            ResEliminarCita res = new ResEliminarCita();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                res.listaErrores = ValiCita.eliminar(req);
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        //req.sesion.usuario....
                        linq.SP_ELIMINAR_CITA(req.idCita,req.idUsuario, fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD);

                    }
                    if (idReturn >= 1) //REVISAR
                    {
                        res.resultado = true;
                        //cuando se maneje sesiones que mande el correo 
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practica
            }
            return res;
        }

        public ResListarCitasUsuario listarCitasUsuario(ReqListarCitasUsuario req)
        {
            ResListarCitasUsuario res = new ResListarCitasUsuario();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                List<SP_OBTENER_CITAS_USUARIOResult> listaTC = new List<SP_OBTENER_CITAS_USUARIOResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    //req.sesion.usuario.id usuario
                    listaTC = linq.SP_OBTENER_CITAS_USUARIO(req.idUsuario).ToList(); 
                }
                res.cita = new List<Cita>();
                res.resultado = true;
                foreach(SP_OBTENER_CITAS_USUARIOResult unTipoComplejo in listaTC)
                {
                    res.cita.Add(this.factoryCita(unTipoComplejo));
                }
                
            }
            catch (Exception ex)
            {
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;
                res.listaErrores.Add(error);
            }
            return res;
        }

        public ResListarCitasBarbero listarCitasBarbero(ReqListarCitasBarbero req)
        {
            ResListarCitasBarbero res = new ResListarCitasBarbero();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                List<SP_OBTENER_CITAS_BARBEROResult> listaTC = new List<SP_OBTENER_CITAS_BARBEROResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    //req.sesion.usuario.id usuario
                    listaTC = linq.SP_OBTENER_CITAS_BARBERO(req.idBarbero).ToList();
                }
                res.cita = new List<Cita>();
                res.resultado = true;
                foreach (SP_OBTENER_CITAS_BARBEROResult unTipoComplejo in listaTC)
                {
                    res.cita.Add(this.factoryCita(unTipoComplejo));
                }

            }
            catch (Exception ex)
            {
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;
                res.listaErrores.Add(error);
            }
            return res;
        }

        public ResListarCitasDia listarCitasDia(ReqListarCitasDia req)
        {
            ResListarCitasDia res = new ResListarCitasDia();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                List<SP_OBTENER_CITAS_POR_DIAResult> listaTC = new List<SP_OBTENER_CITAS_POR_DIAResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
                    listaTC = linq.SP_OBTENER_CITAS_POR_DIA(fechaHoraActual).ToList();
                }
                res.cita = new List<Cita>();
                res.resultado = true;
                foreach (SP_OBTENER_CITAS_POR_DIAResult unTipoComplejo in listaTC)
                {
                    res.cita.Add(this.factoryCita(unTipoComplejo));
                }

            }
            catch (Exception ex)
            {
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;
                res.listaErrores.Add(error);
            }
            return res;
        }

        public ResListarCitasDiaBarbero listarCitasDiaBarbero (ReqListarCitasDiaBarbero req)
        {
            ResListarCitasDiaBarbero res = new ResListarCitasDiaBarbero();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                List<SP_OBTENER_CITAS_POR_DIA_BARBEROResult> listaTC = new List<SP_OBTENER_CITAS_POR_DIA_BARBEROResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
                    listaTC = linq.SP_OBTENER_CITAS_POR_DIA_BARBERO(req.idBarbero, fechaHoraActual).ToList();
                }
                res.cita = new List<Cita>();
                res.resultado = true;
                foreach (SP_OBTENER_CITAS_POR_DIA_BARBEROResult unTipoComplejo in listaTC)
                {
                    res.cita.Add(this.factoryCita(unTipoComplejo));
                }

            }
            catch (Exception ex)
            {
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;
                res.listaErrores.Add(error);
            }
            return res;
        }

        public ResListarTodasCitas listarTodasCitas (ReqListarTodasCitas req)
        {
            ResListarTodasCitas res = new ResListarTodasCitas();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                //validar sesion activa
                List<SP_OBTENER_TODAS_CITASResult> listaTC = new List<SP_OBTENER_TODAS_CITASResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    listaTC = linq.SP_OBTENER_TODAS_CITAS().ToList();
                }
                res.cita = new List<Cita>();
                res.resultado = true;
                foreach (SP_OBTENER_TODAS_CITASResult unTipoComplejo in listaTC)
                {
                    res.cita.Add(this.factoryCita(unTipoComplejo));
                }

            }
            catch (Exception ex)
            {
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;
                res.listaErrores.Add(error);
            }
            return res;
        }

        public ResCitaTerminada citaTerminada(ReqCitaTerminada req)
        {
            ResCitaTerminada res = new ResCitaTerminada();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                res.listaErrores = ValiCita.citaTerminada(req);
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = ""; 

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_CONFIRMAR_CITA_TERMINADA(req.idCita, req.idBarbero, ref idReturn, ref errorIdBD, ref errorMsgBD);

                    }
                    if (idReturn >= 1) 
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practica
            }
            return res;
        }

        public ResCitaNoAsistio citaNoAsistio(ReqCitaNoAsistio req)
        {
            ResCitaNoAsistio res = new ResCitaNoAsistio();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                res.listaErrores = ValiCita.citaNoAsistio(req);
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_MARCAR_CITA_NO_ASISTIO(req.idCita, req.idBarbero, ref idReturn, ref errorIdBD, ref errorMsgBD);

                    }
                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practica
            }
            return res;
        }

        private Cita factoryCita(dynamic tc) 
        {
            Cita cita = new Cita();
            cita.idCita = tc.Id_Cita;
            cita.fechaHora = tc.Fecha_Hora;
            cita.estado = tc.Estado;

            cita.usuario = new Usuario();
            cita.usuario.idUsuario = tc.Id_Usuario;
            cita.usuario.nombre = tc.Usuario_Nombre;
            cita.usuario.apellido = tc.Usuario_Apellido;
            cita.usuario.correoElectronico = tc.Usuario_Correo;
            cita.usuario.telefono = tc.Usuario_Telefono;

            cita.servicio = new Servicio();
            cita.servicio.idServicio = tc.Id_Servicio;
            cita.servicio.nombre = tc.Servicio_Nombre;
            cita.servicio.duracion_minutos = tc.Servicio_Duracion; 
            cita.servicio.precio = tc.Servicio_Precio;

            cita.barbero = new Usuario();
            cita.barbero.idUsuario = tc.Id_Barbero;
            cita.barbero.nombre = tc.Barbero_Nombre;

            return cita;
        }
    }
}
