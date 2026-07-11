using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica.Validaciones
{
    public class ValiAdmin
    {
        public static List<Error> insertar(ReqInsertarAdmin req)
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

        public static List<Error> actualizar(ReqActualizarAdmin req)
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

        public static List<Error> eliminar(ReqEliminarAdmin req)
        {
            List<Error> errores = new List<Error>();
            Helpers helpers = new Helpers();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idAdmin < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

    }
}
