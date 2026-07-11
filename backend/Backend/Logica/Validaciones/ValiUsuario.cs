using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class ValiUsuario 
    {
        public static List<Error> insertarUsuario(ReqInsertarUsuario req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.usuario.nombre))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.nombreFaltante, "Nombre faltante"));
                }
                if (String.IsNullOrEmpty(req.usuario.apellido))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.apellidoFaltante, "Apellido faltante"));
                }
                if (String.IsNullOrEmpty(req.usuario.telefono))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.telefonoFaltante, "Telefono faltante"));
                }
                else if (req.usuario.telefono.Length != 8)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.telefonoInvalido, "Telefono debe tener exactamente 8 digitos"));
                }
                if (String.IsNullOrEmpty(req.usuario.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo faltante"));
                }
                else if (!helpers.EsCorreoValido(req.usuario.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
                if (String.IsNullOrEmpty(req.usuario.contraseña))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.contraseñaFaltante, "Password vacio"));
                }
                else if (!helpers.EsPasswordSeguro(req.usuario.contraseña))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.contraseñaDebil, "Password debil"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> ActualizarUsuario(ReqActualizarUsuario req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.usuario.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
                if (String.IsNullOrEmpty(req.usuario.nombre))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.nombreFaltante, "Nombre faltante"));
                }
                if (String.IsNullOrEmpty(req.usuario.apellido))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.apellidoFaltante, "Apellido faltante"));
                }
                if (String.IsNullOrEmpty(req.usuario.telefono))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.telefonoFaltante, "Telefono faltante"));
                }
                else if (req.usuario.telefono.Length != 8)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.telefonoInvalido, "Telefono debe tener exactamente 8 digitos"));
                }
                if (String.IsNullOrEmpty(req.usuario.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo faltante"));
                }
                else if (!helpers.EsCorreoValido(req.usuario.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> eliminar(ReqEliminarUsuario req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> ActivarUsuario(ReqActivarCuenta req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null) 
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo no valido vacio"));
                }
                else if (!helpers.EsCorreoValido(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
                if (string.IsNullOrEmpty(req.numeroVerificacion))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.numeroVerificacionFaltante, "Numero de verificación faltante.")); ;
                }
                else if (req.numeroVerificacion.Length != 5)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.numeroVerificacionInvalido, "El numero de verificación debe tener exactamente 5 caracteres."));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> login(ReqLogin req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null) 
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {              
                if (String.IsNullOrEmpty(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo no valido vacio"));
                }
                else if (!helpers.EsCorreoValido(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
                if (String.IsNullOrEmpty(req.contraseña))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.contraseñaFaltante, "Password vacio"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> cambiarContra(ReqCambiarContraseña req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo no valido vacio"));
                }
                else if (!helpers.EsCorreoValido(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
                if (String.IsNullOrEmpty(req.contraseña))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.contraseñaFaltante, "Password vacio"));
                }
                else if (!helpers.EsPasswordSeguro(req.contraseña))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.contraseñaDebil, "Password debil"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> solicitarCodigo(ReqSolicitarCodigo req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo no valido vacio"));
                }
                else if (!helpers.EsCorreoValido(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> CodigoCambioContra(ReqCodigoCambioContra req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoFaltante, "Correo no valido vacio"));
                }
                else if (!helpers.EsCorreoValido(req.correoElectronico))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.correoInvalido, "Correo incorrecto"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }
    }
}
