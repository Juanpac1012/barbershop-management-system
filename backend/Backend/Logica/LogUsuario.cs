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
    public class LogUsuario
    {
        
        public ResInsertarUsuario insertar(ReqInsertarUsuario req)
        {
            ResInsertarUsuario res = new ResInsertarUsuario();
            res.listaErrores = ValiUsuario.insertarUsuario(req);
            Helpers helpers = new Helpers();

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
                    string pin = helpers.GenerarPin(5);
                    string salt;
                    string contraseñaHasheada = Helpers.HashearContraseña(req.usuario.contraseña, out salt);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_INSERTAR_USUARIO(req.usuario.nombre, req.usuario.apellido, req.usuario.correoElectronico, req.usuario.telefono, contraseñaHasheada, salt, pin,fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        
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

        public ResActualizarUsuario actualizar(ReqActualizarUsuario req)
        {
            ResActualizarUsuario res = new ResActualizarUsuario();
            res.listaErrores = ValiUsuario.ActualizarUsuario(req);

            try
            {
                //validar sesion activa
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        //req.sesion...
                        linq.SP_ACTUALIZAR_USUARIO(req.usuario.idUsuario, req.usuario.nombre, req.usuario.apellido, req.usuario.correoElectronico,req.usuario.telefono, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }

                    if(idReturn >= 1)
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

        public ResEliminarUsuario eliminar(ReqEliminarUsuario req)
        {
            ResEliminarUsuario res = new ResEliminarUsuario();
            res.listaErrores = ValiUsuario.eliminar(req);

            try
            {
                //validar sesion activa
                if (!res.listaErrores.Any())
                {
                    
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        //req.sesion.usuario...
                        linq.SP_ELIMINAR_USUARIO(req.idUsuario, ref idReturn, ref errorIdBD, ref errorMsgBD);
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

        public ResObtenerUsuario obtenerUsuario(ReqObtenerUsuario req)
        {
            ResObtenerUsuario res = new ResObtenerUsuario();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                   //validar sesion activa
                
                SP_OBTENER_USUARIOResult miTipoComplejo = new SP_OBTENER_USUARIOResult();

                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    //req.sesion.usuario.idUsuario
                    res.usuario = this.factoryUsuario((SP_OBTENER_USUARIOResult)linq.SP_OBTENER_USUARIO(req.sesion.usuario.idUsuario));
                }

                res.resultado = true;
                    
                
            }
            catch (Exception ex)
            {
                //revisar
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message; //mala practica
                res.resultado = false;
                res.listaErrores.Add(error);
            }
            return res;
        }

        public ResActivarCuenta activarCuenta(ReqActivarCuenta req)
        {
            ResActivarCuenta res = new ResActivarCuenta();
            res.listaErrores = ValiUsuario.ActivarUsuario(req);

            try
            {               
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    int? filasAct = 0;
                    string errorMsgBD = "";
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_ACTIVAR_CUENTA(req.correoElectronico, req.numeroVerificacion,fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD, ref filasAct);
                    }

                    if (idReturn == 1) //revisar
                    {
                        res.resultado = true;
                    }
                    else
                    {
                        res.resultado = false;
                        res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, errorMsgBD));
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;          
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));                   
            }

            return res;
        } 

        public ResLogin login(ReqLogin req)
        {
            ResLogin res = new ResLogin();
            res.listaErrores = ValiUsuario.login(req);
            Helpers helpers = new Helpers();
            
            try
            {
                if (!res.listaErrores.Any())
                {             
                    string hashAlmacenado = "";
                    string saltAlmacenado = "";
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    SP_LOGINResult tc = new SP_LOGINResult(); 

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        var resultado = linq.SP_LOGIN(req.correoElectronico, ref idReturn, ref hashAlmacenado, ref saltAlmacenado, ref errorIdBD, ref errorMsgBD).ToList();

                        if (resultado.Any())
                        {
                            tc = resultado.First();
                        }
                    }

                    if (errorIdBD == 0 )
                    {
                        if(!Helpers.VerificarContraseña(req.contraseña, hashAlmacenado, saltAlmacenado))
                        {
                            res.listaErrores.Add(Helpers.CrearError(EnumErrores.credencialesIncorrectas, "Credenciales incorrectas"));
                            //revisar
                            res.resultado = false; 
                            return res;
                        }
                          
                        res.resultado = true;
                        res.usuario = new Usuario();
                        res.usuario = factoryUsuario(tc);
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
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message)); //mala practica
            }
            return res;
        }

        public ResCambiarContraseña cambiarContraseña(ReqCambiarContraseña req)
        {
            ResCambiarContraseña res = new ResCambiarContraseña();
            res.listaErrores = ValiUsuario.cambiarContra(req);

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
                    string salt;
                    string contraseñaHasheada = Helpers.HashearContraseña(req.contraseña, out salt);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_CAMBIAR_CONTRASEÑA(req.correoElectronico,req.numeroVerificacion, contraseñaHasheada, salt,fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }
                    if (idReturn == 1)
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
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message)); //mala practica
            }
            return res;
        } 

        public ResSolicitarCodigo solicitarCodigo(ReqSolicitarCodigo req)
        {
            ResSolicitarCodigo res = new ResSolicitarCodigo();
            res.listaErrores = ValiUsuario.solicitarCodigo(req);
            Helpers helpers = new Helpers();

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";
                    string pin = helpers.GenerarPin(5);
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_SOLICITAR_NUEVO_CODIGO(req.correoElectronico, pin,fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }
                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        bool correoEnviado = Utilitarios.EnviarCorreoConNuevoPin(req.correoElectronico, pin);
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
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message)); //mala practica
            }
            return res;
        }

        public ResCodigoCambioContra CodigoCambioContra(ReqCodigoCambioContra req)
        {
            ResCodigoCambioContra res = new ResCodigoCambioContra();
            res.listaErrores = ValiUsuario.CodigoCambioContra(req);
            Helpers helpers = new Helpers();

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";
                    string pin = helpers.GenerarPin(5);
                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_CODIGO_CAMBIO_CONTRA(req.correoElectronico, pin,fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }
                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        bool correoEnviado = Utilitarios.EnviarCorreoCambioContraseña(req.correoElectronico, pin);
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
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message)); //mala practica
            }
            return res;
        }

        private Usuario factoryUsuario(dynamic tc)
        {
            Usuario usuario = new Usuario();
            usuario.idUsuario = (int)tc.Id_Usuario;
            usuario.nombre = tc.Nombre;
            usuario.apellido = tc.Apellido;
            usuario.correoElectronico = tc.Correo_Electronico;
            usuario.telefono = tc.Telefono;
            usuario.idRol = (EnumRoles)tc.Id_Rol;
            return usuario;
        }
    }
}
