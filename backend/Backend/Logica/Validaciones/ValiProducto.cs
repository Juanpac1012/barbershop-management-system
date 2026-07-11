using Backend.Entidades;
using Backend.Logica;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class ValiProducto
    {
        public static List<Error> insertar(ReqInsertarProducto req)
        {
            List<Error> errores = new List<Error>();
          
            if (req == null) 
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (String.IsNullOrEmpty(req.producto.nombre))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.nombreFaltante, "Nombre faltante"));
                }
                if (String.IsNullOrEmpty(req.producto.descripcion))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionFaltante, "Descripcion faltante"));
                }  
                else if (req.producto.descripcion.Length < 10 || req.producto.descripcion.Length > 200)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionInvalida, "La descripcion del producto debe tener entre 10 y 200 caracteres."));
                }
                if (req.producto.precio < 1 || req.producto.precio > 100000) 
                {
                    errores.Add(Helpers.CrearError(EnumErrores.precioInvalido, "El precio debe estar entre 1 y 100,000."));
                }
                if (req.producto.stock < 0 || req.producto.stock > 100000) 
                {
                    errores.Add(Helpers.CrearError(EnumErrores.stockInvalido, "Stock faltante"));             
                }

            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> actualizar(ReqActualizarProducto req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.producto.idProducto < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
                if (String.IsNullOrEmpty(req.producto.nombre))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.nombreFaltante, "Nombre faltante"));
                }
                if (String.IsNullOrEmpty(req.producto.descripcion))
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionFaltante, "Descripcion faltante"));
                }
                else if (req.producto.descripcion.Length < 10 || req.producto.descripcion.Length > 200)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.descripcionInvalida, "La descripcion del producto debe tener entre 10 y 200 caracteres."));
                }
                if (req.producto.precio < 1 || req.producto.precio > 100000)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.precioInvalido, "El precio debe estar entre 1 y 100,000."));
                }
                if (req.producto.stock < 0 || req.producto.stock > 100000)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.stockInvalido, "Stock faltante"));
                }

            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> eliminar(ReqEliminarProducto req)
        {
            List<Error> errores = new List<Error>();

            if (req == null)
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idProducto < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

    }
}
