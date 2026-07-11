using AccesoDatos;
using Backend.Entidades;
using Backend.Logica.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class LogAdmin
    {
        public ResInsertarAdmin insertar(ReqInsertarAdmin req)
        {
            ResInsertarAdmin res = new ResInsertarAdmin();
            res.listaErrores = ValiAdmin.insertar(req);
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
                        linq.SP_INSERTAR_ADMIN(req.usuario.nombre, req.usuario.apellido, req.usuario.correoElectronico, req.usuario.telefono, contraseñaHasheada, salt, pin, fechaHoraActual, ref idReturn, ref errorIdBD, ref errorMsgBD);
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;

                        bool correoEnviado = Utilitarios.EnviarCorreoConPin(req.usuario.correoElectronico, req.usuario.nombre, pin);
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

        public ResActualizarAdmin actualizar(ReqActualizarAdmin req)
        {
            ResActualizarAdmin res = new ResActualizarAdmin();
            res.listaErrores = ValiAdmin.actualizar(req);

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
                        linq.SP_ACTUALIZAR_ADMIN(req.usuario.idUsuario, req.usuario.nombre, req.usuario.apellido, req.usuario.correoElectronico, req.usuario.telefono, contraseñaHasheada, salt, ref idReturn, ref errorIdBD, ref errorMsgBD);
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

        public ResEliminarAdmin eliminar(ReqEliminarAdmin req)
        {
            ResEliminarAdmin res = new ResEliminarAdmin();
            res.listaErrores = ValiAdmin.eliminar(req);

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
                        linq.SP_ELIMINAR_ADMIN(req.idAdmin, ref idReturn, ref errorIdBD, ref errorMsgBD);
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

        public ResListarAdmins listar(ReqListarAdmins req)
        {
            ResListarAdmins res = new ResListarAdmins();
            res.listaErrores = new List<Error>();

            try
            {
                List<SP_OBTENER_LISTAADMINResult> listaTC = new List<SP_OBTENER_LISTAADMINResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    listaTC = linq.SP_OBTENER_LISTAADMIN().ToList();
                }
                res.admins = new List<Usuario>();
                res.resultado = true;
                foreach (SP_OBTENER_LISTAADMINResult unTipoComplejo in listaTC)
                {
                    res.admins.Add(this.factoryAdmin(unTipoComplejo));
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

        private Usuario factoryAdmin(SP_OBTENER_LISTAADMINResult tc)
        {
            Usuario admin = new Usuario();
            admin.idUsuario = (int)tc.Id_Usuario;
            admin.nombre = tc.Nombre;
            admin.apellido = tc.Apellido;
            admin.correoElectronico = tc.Correo_Electronico;
            admin.telefono = tc.Telefono;
            admin.idRol = (EnumRoles)tc.Id_Rol;

            return admin;
        }
    }
}
