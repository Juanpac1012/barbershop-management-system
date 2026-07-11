using AccesoDatos;
using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class LogServicio
    {
        public ResInsertarServicio insertar(ReqInsertarServicio req)
        {
            ResInsertarServicio res = new ResInsertarServicio();
            res.listaErrores = ValiServicio.insertar(req);

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_INSERTAR_SERVICIO(req.servicio.nombre, req.servicio.descripcion, req.servicio.duracion_minutos, req.servicio.precio, ref idReturn, ref errorIdBD, ref errorMsgBD);
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
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));//mala practica
                res.resultado = false;
            }
            return res;
        }

        public ResActualizarServicio actualizar(ReqActualizarServicio req)
        {
            ResActualizarServicio res = new ResActualizarServicio();
            res.listaErrores = ValiServicio.actualizar(req);

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
                        //req.sesion.servicio.
                        linq.SP_ACTUALIZAR_SERVICIO(req.servicio.idServicio, req.servicio.nombre, req.servicio.descripcion, req.servicio.duracion_minutos, req.servicio.precio, ref idReturn, ref errorIdBD, ref errorMsgBD);
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

        public ResEliminarServicio eliminar(ReqEliminarServicio req)
        {
            ResEliminarServicio res = new ResEliminarServicio();
            res.listaErrores = ValiServicio.eliminar(req);

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
                        //req.sesion.servicio..
                        linq.SP_ELIMINAR_SERVICIO(req.idServicio, ref idReturn, ref errorIdBD, ref errorMsgBD);
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
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));
            }
            return res;
        }

        public ResListarSercicio listar(ReqListarServicio req)
        {
            ResListarSercicio res = new ResListarSercicio();
            res.listaErrores = new List<Error>();

            try
            {
                Error error = new Error();
                List<SP_OBTENER_LISTASERVICIOSResult> listaTC = new List<SP_OBTENER_LISTASERVICIOSResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    listaTC = linq.SP_OBTENER_LISTASERVICIOS().ToList();
                }
                res.servicio = new List<Servicio>();
                res.resultado = true;
                foreach (SP_OBTENER_LISTASERVICIOSResult unTipoComplejo in listaTC)
                {
                    res.servicio.Add(this.factoryServicio(unTipoComplejo));
                }
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

        private Servicio factoryServicio(SP_OBTENER_LISTASERVICIOSResult tc)
        {
            Servicio servicio = new Servicio();
            servicio.idServicio = tc.Id_Servicio;
            servicio.nombre = tc.Nombre;
            servicio.descripcion = tc.Descripcion;
            servicio.duracion_minutos = tc.Duracion_Minutos;
            servicio.precio = (int)tc.Precio;
            //img

            return servicio;
        }
    }
}
