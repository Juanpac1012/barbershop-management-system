using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class ValiServicio
    {
        public static List<Error> insertar(ReqInsertarServicio req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.servicio.nombre))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.nombreFaltante, "Nombre faltante"));
                }
                if (String.IsNullOrEmpty(req.servicio.descripcion))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionFaltante, "Descripcion faltante"));
                }
                else if (req.servicio.descripcion.Length < 20 || req.servicio.descripcion.Length > 200)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionInvalida, "La descripcion del servicio debe tener entre 10 y 200 caracteres."));
                }
                if (req.servicio.duracion_minutos < 30 || req.servicio.duracion_minutos > 300)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.duracion_minutosInvalido, "La duracion de tiempo debe de ser entre 30 minutos a 300 minutos (5 horas)."));
                }
                if (req.servicio.precio < 1 || req.servicio.precio > 100000)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.precioServicioInvalido, "El precio debe estar entre 1 y 100,000"));
                }

            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> actualizar(ReqActualizarServicio req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.servicio.idServicio < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
                if (String.IsNullOrEmpty(req.servicio.nombre))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.nombreFaltante, "Nombre faltante"));
                }
                if (String.IsNullOrEmpty(req.servicio.descripcion))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionFaltante, "Descripcion faltante"));
                }
                else if (req.servicio.descripcion.Length < 20 || req.servicio.descripcion.Length > 200)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionInvalida, "La descripcion del servicio debe tener entre 10 y 200 caracteres."));
                }
                if (req.servicio.duracion_minutos < 30 || req.servicio.duracion_minutos > 300)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.duracion_minutosInvalido, "La duracion de tiempo debe de ser entre 30 minutos a 300 minutos (5 horas)."));
                }
                if (req.servicio.precio < 1 || req.servicio.precio > 100000)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.precioServicioInvalido, "El precio debe estar entre 1 y 100,000"));
                }

            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> eliminar(ReqEliminarServicio req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idServicio < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }
    }
}

