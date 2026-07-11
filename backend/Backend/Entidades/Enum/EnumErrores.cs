using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Entidades
{
    public enum EnumErrores
    {
        excepcionBaseDatos = -2,
        excepcionLogica = -1,
        resquestNulo = 1,
        nombreFaltante = 2,
        apellidoFaltante = 3,
        telefonoFaltante = 4,
        correoFaltante = 5,
        correoInvalido = 6,
        contraseñaFaltante = 7,
        contraseñaDebil = 8,
        descripcionFaltante = 9,
        precioInvalido = 10,
        stockInvalido = 11,
        idFaltante = 12,
        sesionCerrada = 13,
        correoNoEnviado = 14,
        telefonoInvalido = 15,
        descripcionInvalida = 16,
        numeroVerificacionFaltante = 17,
        numeroVerificacionInvalido = 18,
        credencialesIncorrectas = 19,
        duracion_minutosInvalido = 20,
        precioServicioInvalido = 21,
        fechaInvalida = 22,
    }
}
