using AccesoDatos;
using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class LogProducto
    {
        public ResInsertarProducto insertar(ReqInsertarProducto req)
        {
            ResInsertarProducto res = new ResInsertarProducto();
            res.listaErrores = ValiProducto.insertar(req);

            try
            {
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        linq.SP_INSERTAR_PRODUCTO(req.producto.nombre, req.producto.descripcion, req.producto.precio, req.producto.stock, ref idReturn, ref errorIdBD, ref errorMsgBD);

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

        public ResActualizarProducto actualizar(ReqActualizarProducto req)
        {
            ResActualizarProducto res = new ResActualizarProducto();
            res.listaErrores = ValiProducto.actualizar(req);

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
                        //req.sesion.producto
                        linq.SP_ACTUALIZAR_PRODUCTO(req.producto.idProducto, req.producto.nombre, req.producto.descripcion, req.producto.precio, req.producto.stock, ref idReturn, ref errorIdBD, ref errorMsgBD);
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

        public ResEliminarProducto eliminar(ReqEliminarProducto req)
        {
            ResEliminarProducto res = new ResEliminarProducto();
            res.listaErrores = ValiProducto.eliminar(req);

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
                        //req.sesion.producto
                        linq.SP_ELIMINAR_PRODUCTO(req.idProducto, ref idReturn, ref errorIdBD, ref errorMsgBD);
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
            catch (Exception ex) {
                res.resultado = false;
                res.listaErrores.Add(Helpers.CrearError(EnumErrores.excepcionLogica, ex.Message));//mala practica
            }
            return res;
        }

        public ResListarProducto listar(ReqListarProducto req)
        {
            ResListarProducto res = new ResListarProducto();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                
                List<SP_OBTENER_LISTAPRODUCTOSResult> listaTC = new List<SP_OBTENER_LISTAPRODUCTOSResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    listaTC = linq.SP_OBTENER_LISTAPRODUCTOS().ToList();
                }
                res.producto = new List<Producto>();
                res.resultado = true;
                foreach (SP_OBTENER_LISTAPRODUCTOSResult unTipoComplejo in listaTC)
                {
                    res.producto.Add(this.factoryProducto(unTipoComplejo));
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

        private Producto factoryProducto(SP_OBTENER_LISTAPRODUCTOSResult tc)
        {
            Producto producto = new Producto();
            producto.idProducto = tc.Id_Producto;
            producto.nombre = tc.Nombre;
            producto.descripcion = tc.Descripcion;
            producto.stock = tc.Stock;
            producto.precio = tc.Precio;
            //img
            return producto;
        }
    }
}
