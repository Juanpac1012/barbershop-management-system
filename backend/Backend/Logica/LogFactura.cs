using AccesoDatos;
using Backend.Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Logica
{
    public class LogFactura
    {
        public ResInsertarFacturaProductos insertarConProductos(ReqInsertarFacturaProductos req)
        {
            ResInsertarFacturaProductos res = new ResInsertarFacturaProductos();
            res.listaErrores = new List<Error>();
            Error error = new Error();
            Factura facturaGenerada = null;

            try
            {
                res.listaErrores = ValiFactura.insertarProductos(req); //revisar y cambiar validaciones
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";
                    // Convertir los productos a formato JSON
                    var productosParaSP = req.factura.productos.Select(p => new
                    {
                        Id_Producto = p.idProducto, 
                        Cantidad = p.cantidad
                    }).ToList();

                    var productosJson = JsonConvert.SerializeObject(productosParaSP);

                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);

                    List<SP_INSERTAR_FACTURA_PRODUCTOResult> tc = new List<SP_INSERTAR_FACTURA_PRODUCTOResult>();

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        var resultado = linq.SP_INSERTAR_FACTURA_PRODUCTO( req.factura.usuario.idUsuario, fechaHoraActual,"Pendiente", productosJson, ref idReturn,ref errorIdBD,ref errorMsgBD).ToList();
                        if (resultado.Any())
                        {
                            facturaGenerada = factoryFactura(resultado);
                        }                  
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        if (facturaGenerada != null)
                        {
                            bool correoEnviado = Utilitarios.EnviarFacturaProductosConPdf(facturaGenerada);
                            if (!correoEnviado)
                            {
                                error.ErrorCode = EnumErrores.correoNoEnviado;
                                error.Message = "Factura creada pero no se pudo enviar el correo";
                                res.listaErrores.Add(error);
                            }
                        }
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practicajnnnnnn
            }
            return res;
        }

        public ResInsertarFacturaCita insertarConCita(ReqInsertarFacturaCita req)
        {
            ResInsertarFacturaCita res = new ResInsertarFacturaCita();
            res.listaErrores = new List<Error>();
            Error error = new Error();
            Factura facturaGenerada = null;

            try
            {
                res.listaErrores = ValiFactura.insertarCitas(req); //revisar y cambiar validaciones
                if (!res.listaErrores.Any())
                {
                    int? idReturn = 0;
                    int? errorIdBD = 0;
                    string errorMsgBD = "";

                    DateTime fechaHoraActual = DateTime.UtcNow.AddHours(-6);
                    SP_INSERTAR_FACTURA_CITAResult tc = new SP_INSERTAR_FACTURA_CITAResult();

                    using (ConexionDataContext linq = new ConexionDataContext())
                    {
                        var resultado = linq.SP_INSERTAR_FACTURA_CITA(req.idUsuario, req.idCita, fechaHoraActual, "Pendiente", ref idReturn, ref errorIdBD, ref errorMsgBD).ToList();
                        if (resultado.Any())
                        {
                            facturaGenerada = factoryFacturaCita(resultado.First());
                        }
                    }

                    if (idReturn >= 1)
                    {
                        res.resultado = true;
                        bool correoEnviado = Utilitarios.EnviarFacturaCitaConPdf(facturaGenerada);
                        if (!correoEnviado)
                        {
                            error.ErrorCode = EnumErrores.correoNoEnviado;
                            error.Message = "Factura creada pero no se pudo enviar el correo";
                            res.listaErrores.Add(error);
                        }
                    }
                    else
                    {
                        error.ErrorCode = EnumErrores.excepcionBaseDatos;
                        error.Message = errorMsgBD;
                        res.listaErrores.Add(error);
                    }
                }
            }
            catch (Exception ex)
            {
                res.resultado = false;
                error.ErrorCode = EnumErrores.excepcionLogica;
                error.Message = ex.Message;//mala practicajnnnnnn
            }
            return res;
        }

        public ResListarFacturasPUsuario listarFacturasPUsuario(ReqListarFacturasPUsuario req)
        {
            ResListarFacturasPUsuario res = new ResListarFacturasPUsuario();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                List<SP_LISTAR_FACTURAS_USUARIO_PRODUCTOSResult> listaTC = new List<SP_LISTAR_FACTURAS_USUARIO_PRODUCTOSResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    listaTC = linq.SP_LISTAR_FACTURAS_USUARIO_PRODUCTOS(req.idUsuario).ToList();
                }
                res.facturas = new List<Factura>();
                res.resultado = true;
                foreach (var grupo in listaTC.GroupBy(x => x.Id_Factura)) 
                {
                    res.facturas.Add(fa(grupo));
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

        public ResListarFacturasCUsuario listarFacturasCUsuario(ReqListarFacturasCUsuario req)
        {
            ResListarFacturasCUsuario res = new ResListarFacturasCUsuario();
            res.listaErrores = new List<Error>();
            Error error = new Error();

            try
            {
                List<SP_LISTAR_FACTURAS_USUARIO_CITASResult> listaTC = new List<SP_LISTAR_FACTURAS_USUARIO_CITASResult>();
                using (ConexionDataContext linq = new ConexionDataContext())
                {
                    listaTC = linq.SP_LISTAR_FACTURAS_USUARIO_CITAS(req.idUsuario).ToList();
                }
                res.facturas = new List<Factura>();
                res.resultado = true;
                foreach (SP_LISTAR_FACTURAS_USUARIO_CITASResult unTipoComplejo in listaTC)
                {
                    res.facturas.Add(this.factoryListarFacturaCita(unTipoComplejo));
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

        private Factura factoryFactura(List<SP_INSERTAR_FACTURA_PRODUCTOResult> resultadoSP)
        {
            if (resultadoSP == null || !resultadoSP.Any())
                return null;

            dynamic tc = resultadoSP.First();

            Factura factura = new Factura();
            factura.idFactura = tc.Id_Factura;
            factura.fecha = tc.Fecha_Factura;
            factura.subtotal = tc.Subtotal;
            factura.total = tc.Total;
            factura.estado = tc.Estado_Factura;

            // Usuario (cliente)
            factura.usuario = new Usuario();
            factura.usuario.nombre = tc.Nombre_Cliente;
            factura.usuario.correoElectronico = tc.Correo_Electronico;

            // Lista de productos
            factura.productos = new List<Producto>();

            foreach (var item in resultadoSP)
            {
                Producto producto = new Producto();
                producto.idProducto = item.Id_Producto;
                producto.nombre = item.Nombre_Producto;
                producto.cantidad = item.Cantidad;
                producto.precio = item.Precio_Unitario;
                producto.stock = item.Nuevo_Stock; 

                factura.productos.Add(producto);
            }

            factura.cantidadProductos = factura.productos.Count;

            return factura;
        }

        private Factura factoryFacturaCita(SP_INSERTAR_FACTURA_CITAResult tc)
        {
            if (tc == null)
                return null;

            Factura factura = new Factura();
            factura.idFactura = tc.Id_Factura;
            factura.fecha = (DateTime)tc.Fecha; 
            factura.subtotal = (decimal)tc.Subtotal;
            factura.total = (decimal)tc.Total;
            factura.estado = tc.Estado;

            factura.usuario = new Usuario();
            factura.usuario.idUsuario = tc.Id_Usuario;
            factura.usuario.nombre = tc.NombreUsuario;
            factura.usuario.correoElectronico = tc.CorreoUsuario;

            factura.cita = new Cita();
            factura.cita.idCita = tc.Id_Cita;
            factura.cita.fechaHora = tc.FechaCita;

            factura.cita.servicio = new Servicio();
            factura.cita.servicio.idServicio = tc.Id_Servicio;
            factura.cita.servicio.nombre = tc.NombreServicio;
            factura.cita.servicio.precio = tc.PrecioServicio;

            return factura;
        }

        private Factura factoryListarFacturaCita(SP_LISTAR_FACTURAS_USUARIO_CITASResult tc)
        {
            if (tc == null)
                return null;

            Factura factura = new Factura();
            factura.idFactura = tc.Id_Factura;
            factura.fecha = tc.Fecha_Cita;
            factura.subtotal = (decimal)tc.Subtotal;
            factura.total = (decimal)tc.Total;
            factura.estado = tc.Estado;

            factura.usuario = new Usuario();
            factura.usuario.idUsuario = tc.Id_Usuario;
            factura.usuario.nombre = tc.Usuario_Nombre;
            factura.usuario.apellido = tc.Usuario_Apellido;
            factura.usuario.correoElectronico = tc.Usuario_Correo;
            factura.usuario.telefono = tc.Usuario_Telefono;

            factura.cita = new Cita();
            factura.cita.idCita = tc.Id_Cita;
            factura.cita.fechaHora = tc.Fecha_Cita;

            factura.cita.servicio = new Servicio();
            factura.cita.servicio.nombre = tc.Servicio;
            factura.cita.servicio.descripcion = tc.Descripcion;
            factura.cita.servicio.duracion_minutos = tc.Duracion_Minutos;
            factura.cita.servicio.precio = tc.Precio_Servicio;

            return factura;
        }

        private Factura fa(IEnumerable<dynamic> grupo)
        {
            var factura = new Factura();
            var tc = grupo.First(); // Datos generales de la factura

            factura.idFactura = tc.Id_Factura;
            factura.fecha = tc.Fecha_Factura;
            factura.estado = tc.Estado_Factura;
            factura.subtotal = tc.Subtotal;
            factura.total = tc.Total;

            factura.usuario = new Usuario();
            factura.usuario.idUsuario = tc.Id_Usuario;
            factura.usuario.nombre = tc.Usuario_Nombre;
            factura.usuario.apellido = tc.Usuario_Apellido;
            factura.usuario.correoElectronico = tc.Usuario_Correo;
            factura.usuario.telefono = tc.Usuario_Telefono;

            factura.productos = new List<Producto>();

            foreach (var item in grupo)
            {
                Producto producto = new Producto();
                producto.idProducto = item.Id_Producto;
                producto.nombre = item.Nombre_Producto;
                producto.cantidad = item.Cantidad;
                producto.precio = item.Precio_Unitario;

                factura.productos.Add(producto);
            }

            factura.cantidadProductos = factura.productos.Count;

            return factura;
        }

    }
}

