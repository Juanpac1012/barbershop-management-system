using AccesoDatos;
using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class LogBarbero
    {
        public ResInsertarBarbero insertar(ReqInsertarBarbero req)
        {
            ResInsertarBarbero res = new ResInsertarBarbero();
            res.listaErrores = ValiBarbero.insertar(req);
            Helpers helpers = new Helpers();

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    string pin = helpers.GenerarPin(5);

                    // Hashear la contraseña con salt
                    string salt;
                    string contraseñaHasheada = Helpers.HashearContraseña(req.usuario.contraseña, out salt);
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_INSERTAR_BARBERO(req.usuario.nombre, req.usuario.apellido, req.usuario.correoElectronico, req.usuario.telefono, contraseñaHasheada, salt, pin, fechaHoraActual,ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        // enviar el correo con el PIN
                        bool correoEnviado = Utilitarios.EnviarCorreoConPin(req.usuario.correoElectronico,req.usuario.nombre, pin);
                        if (!correoEnviado)
                        {
                            res.resultado = false;
                            res.listaErrores.Add(Helpers.CrearError(EnumErrores.correoNoEnviado, "Correo no enviado"));
                        }
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionBaseDatos, errorMsgBD));
                    }
                }

            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));//mala practica
            }
            return res;
        }

        public ResActualizarBarbero actualizar(ReqActualizarBarbero req)
        {
            ResActualizarBarbero res = new ResActualizarBarbero();
            res.listaErrores = ValiBarbero.actualizar(req);

            try
            {
                //validar sesion
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    string salt;
                    string contraseñaHasheada = Helpers.HashearContraseña(req.usuario.contraseña, out salt);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        //req.sesion.usuario.
                        linq.SP_ACTUALIZAR_BARBERO(req.usuario.idUsuario, req.usuario.nombre, req.usuario.apellido, req.usuario.correoElectronico, req.usuario.telefono, contraseñaHasheada, salt, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionBaseDatos, errorMsgBD));
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));//mala practica
            }
            return res;
        }

        public ResEliminarBarbero eliminar(ReqEliminarBarbero req)
        {
            ResEliminarBarbero res = new ResEliminarBarbero();
            res.listaErrores = ValiBarbero.eliminar(req);

            try
            {
                //validar sesion
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        //req.sesion.usuario.
                        linq.SP_ELIMINAR_BARBERO(req.idBarbero, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionBaseDatos, errorMsgBD));
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));//mala practica
            }
            return res;
        }  

        public ResListarBarberos listar(ReqListarBarberos req)
        {
            ResListarBarberos res = new ResListarBarberos();
            res.listaErrores = new List<Error>();

            try
            {
                //sesion TALVEZ CAMBIAR POR UN METODO
                //if (req.sesion.estado == EnumEstadoSesion.cerrada)
                //{
                //    Error error = new Error();
                //    error.ErrorCode = (int)EnumErrores.sesionCerrada;
                //    error.Message = "Sesion expirada";
                //    res.listaErrores.Add(error);
                //}
                //else
                //{
                    List<SP_OBTENER_LISTABARBEROSResult> listaTC = new List<SP_OBTENER_LISTABARBEROSResult>();
                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        listaTC = linq.SP_OBTENER_LISTABARBEROS().ToList();
                    }
                    res.barberos = new List<Usuario>();
                    res.resultado = true;
                    foreach (SP_OBTENER_LISTABARBEROSResult unTipoComplejo in listaTC)
                    {
                        res.barberos.Add(this.factoryBarbero(unTipoComplejo));
                    }
                //}
            }
            catch (Exception ex)
            {
                Error error = new Error();
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;
                res.listaErrores.Add(error);
            }
            return res;
        }

        private Usuario factoryBarbero (SP_OBTENER_LISTABARBEROSResult tc)
        {
            Usuario barbero = new Usuario();
            barbero.idUsuario = (int)tc.Id_Usuario;
            barbero.nombre = tc.Nombre;
            barbero.apellido = tc.Apellido;
            barbero.correoElectronico = tc.Correo_Electronico;
            barbero.telefono = tc.Telefono;
            barbero.idRol = (EnumRoles)tc.Id_Rol;

            return barbero;
        }
    }
}
