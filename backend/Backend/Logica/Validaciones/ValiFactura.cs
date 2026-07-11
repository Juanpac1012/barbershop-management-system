using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class ValiFactura
    {
        public static List<Error> insertarProductos(ReqInsertarFacturaProductos req)
        {
            List<Error> errores = new List<Error>();

            if (req == null) // Esto nunca va a ocurrir.
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.factura.usuario.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id usuario faltante"));
                }
            }
            Helpers.ObtenerErrores(errores);
            return errores;
        }

        public static List<Error> insertarCitas(ReqInsertarFacturaCita req)
        {
            List<Error> errores = new List<Error>();

            if (req == null) // Esto nunca va a ocurrir.
            {
                errores.Add(Helpers.CrearError(EnumErrores.resquestNulo, "Req null"));
            }
            else
            {
                if (req.idUsuario < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id usuario faltante"));
                }
                if (req.idCita < 1)
                {
                    errores.Add(Helpers.CrearError(EnumErrores.idFaltante, "Id usuario faltante"));
                }
            }

            Helpers.ObtenerErrores(errores);
            return errores;
        }


    }
}
