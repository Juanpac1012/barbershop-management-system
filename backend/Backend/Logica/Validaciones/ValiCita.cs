using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class ValiCita
    {

        public static List<Error> insertar(ReqInsertarCita req)
        {
            List<Error> errores = new List<Error>();

            // La fecha que viene del frontend ya está en hora local (Costa Rica, UTC-6)
            DateTime fechaHoraCita = req.cita.fechaHora;
            // Obtener la hora actual en la zona horaria de Costa Rica (UTC-6)
            DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
            // Definir horario de atención en la misma fecha de la cita
            DateTime apertura = fechaHoraCita.Date.AddHours(8);  // 8:00 AM
            DateTime cierre = fechaHoraCita.Date.AddHours(20);   // 8:00 PM

            if (req == null) // Esto nunca va a ocurrir.
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.cita.usuario.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id usuario faltante"));
                }
                if (req.cita.barbero.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id barbero faltante"));
                }
                if (req.cita.servicio.idServicio < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id servicio faltante"));
                }
                if (fechaHoraCita <= fechaHoraActual)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.fechaInvalida, "La fecha y hora de la cita no pueden ser anteriores a la hora actual"));
                }
                if (fechaHoraCita.DayOfWeek == DayOfWeek.Sunday)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.fechaInvalida, "Las citas solo pueden ser de lunes a sábado"));
                }
                if (fechaHoraCita < apertura || fechaHoraCita > cierre)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.fechaInvalida, "Las citas deben ser entre las 8:00 AM y las 8:00 PM"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> actualizar(ReqActualizarCita req)
        {
            List<Error> errores = new List<Error>();

            DateTime fechaHoraCita = req.cita.fechaHora;
            // Obtener la hora actual en la zona horaria de Costa Rica (UTC-6)
            DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
            DateTime apertura = fechaHoraCita.Date.AddHours(8);  
            DateTime cierre = fechaHoraCita.Date.AddHours(20);  

            if (req == null) //Esto nunca va ocurrir.
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.cita.idCita < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id cita faltante"));
                }
                if (req.cita.usuario.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id usuario faltante"));
                }
                if (req.cita.barbero.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id barbero faltante"));
                }
                if (req.cita.servicio.idServicio < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id servicio faltante"));
                }
                if (fechaHoraCita <= fechaHoraActual)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.fechaInvalida, "La fecha y hora de la cita no pueden ser anteriores a la hora actual"));
                }
                if (fechaHoraCita.DayOfWeek == DayOfWeek.Sunday)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.fechaInvalida, "Las citas solo pueden ser de lunes a sábado"));
                }
                if (fechaHoraCita < apertura || fechaHoraCita > cierre)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.fechaInvalida, "Las citas deben ser entre las 8:00 AM y las 8:00 PM"));
                }

            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> eliminar(ReqEliminarCita req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idCita < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id cita faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> citaTerminada(ReqCitaTerminada req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idCita < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id cita faltante"));
                }
                if (req.idBarbero < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id barbero faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> citaNoAsistio(ReqCitaNoAsistio req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idCita < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id cita faltante"));
                }
                if (req.idBarbero < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id barbero faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }
    }
}
