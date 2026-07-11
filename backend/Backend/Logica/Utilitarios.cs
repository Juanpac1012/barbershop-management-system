using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Backend.Entidades;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Backend.Logica
{
    public class Utilitarios
    {
        public static bool EnviarCorreoConPin(string destinatario, string nombreUsuario, string pin)
        {
            string remitente = "barberiaproyectopm@gmail.com";  // Tu correo
            string clave = "wrevyexhhoznzyok";  // Tu contraseña de aplicación de Gmail

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(remitente);
                    mail.To.Add(destinatario);
                    mail.Subject = "🔑 Tu código PIN de verificación";

                    // Cuerpo del correo con estilos embebidos en línea
                    string htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; text-align: center; padding: 20px; background-color: #f4f4f4; margin: 0;'>
    <div style='max-width: 400px; width: 100%; padding: 20px; background: #ffffff; border-radius: 8px; 
                box-shadow: 0 4px 8px rgba(0,0,0,0.1); text-align: center; margin: auto;'>
        <div style='font-size: 24px; font-weight: bold; color: #444; margin-bottom: 20px;'>💈 Barbería Proyecto 💈</div>
        <h2 style='color: #333;'>Hola, {nombreUsuario}! 👋</h2>
        <p style='color: #555;'>Usa el siguiente código para completar tu proceso de verificación:</p>
        <div style='font-size: 28px; font-weight: bold; color: #ffffff; background: #007bff; padding: 15px;
                    border-radius: 5px; display: inline-block; margin-top: 15px; letter-spacing: 3px;'>
            {pin}
        </div>
        <p style='color: #555;'>Este código expirará en unos minutos. No lo compartas con nadie.</p>
        <div style='font-size: 14px; color: #e74c3c; margin-top: 15px;'>
            <p>Si tu código ha expirado, por favor solicita uno nuevo desde la aplicación.</p>
        </div>
        <div style='font-size: 12px; color: #666; margin-top: 20px;'>
            <p>Si no solicitaste este código, puedes ignorar este mensaje.</p>
            <p>© {DateTime.Now.Year} Barbería Proyecto PM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                    mail.Body = htmlBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) // Puerto 587 con TLS
                    {
                        smtp.Credentials = new NetworkCredential(remitente, clave);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar el correo: " + ex.Message);
                return false;
            }

        }

        public static bool EnviarCorreoCambioContraseña(string destinatario, string pin)
        {
            string remitente = "barberiaproyectopm@gmail.com";  // Tu correo
            string clave = "wrevyexhhoznzyok";  // Tu contraseña de aplicación de Gmail

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(remitente);
                    mail.To.Add(destinatario);
                    mail.Subject = "🔒 Código para Cambio de Contraseña";

                    // Cuerpo del correo con estilos embebidos en línea
                    string htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; text-align: center; padding: 20px; background-color: #f4f4f4; margin: 0;'>
    <div style='max-width: 400px; width: 100%; padding: 20px; background: #ffffff; border-radius: 8px; 
                box-shadow: 0 4px 8px rgba(0,0,0,0.1); text-align: center; margin: auto;'>
        <div style='font-size: 24px; font-weight: bold; color: #444; margin-bottom: 20px;'>💈 Barbería Proyecto 💈</div>
        <h2 style='color: #333;'>¡Hola! 👋</h2>
        <p style='color: #555;'>Usa el siguiente código para cambiar tu contraseña:</p>
        <div style='font-size: 28px; font-weight: bold; color: #ffffff; background: #007bff; padding: 15px;
                    border-radius: 5px; display: inline-block; margin-top: 15px; letter-spacing: 3px;'>
            {pin}
        </div>
        <p style='color: #555;'>Este código expirará en unos minutos. No lo compartas con nadie.</p>
        <div style='font-size: 14px; color: #e74c3c; margin-top: 15px;'>
            <p>Si tu código ha expirado, por favor solicita uno nuevo desde la aplicación.</p>
        </div>
        <div style='font-size: 12px; color: #666; margin-top: 20px;'>
            <p>Si no solicitaste este código, puedes ignorar este mensaje.</p>
            <p>© {DateTime.Now.Year} Barbería Proyecto PM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                    mail.Body = htmlBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) // Puerto 587 con TLS
                    {
                        smtp.Credentials = new NetworkCredential(remitente, clave);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar el correo: " + ex.Message);
                return false;
            }
        }

        public static bool EnviarCorreoConfirmacionCita(string destinatario, string nombreCliente, DateTime fechaCita, string servicio)
        {
            string remitente = "barberiaproyectopm@gmail.com";
            string clave = "wrevyexhhoznzyok";

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(remitente);
                    mail.To.Add(destinatario);
                    mail.Subject = "💈 Confirmación de Cita en Barbería Proyecto 💈";

                    string htmlBody = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            padding: 20px;
            background-color: #f4f4f4;
            margin: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }
        .container {
            max-width: 400px;
            width: 100%;
            padding: 20px;
            background: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            text-align: center;
        }
        h2 {
            color: #333;
        }
        .barberia-title {
            font-size: 24px;
            font-weight: bold;
            color: #444;
            margin-bottom: 20px;
        }
        .footer {
            font-size: 12px;
            color: #666;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='barberia-title'>💈 Barbería Proyecto 💈</div>
        <h2>📅 ¡Tu cita ha sido confirmada!</h2>
        <p>Hola " + nombreCliente + @",</p>
        <p>Tu cita ha sido reservada con éxito.</p>
        <p><strong>📅 Fecha y Hora:</strong> " + fechaCita.ToString("dd/MM/yyyy hh:mm tt") + @"</p>
        <p><strong>💇 Servicio:</strong> " + servicio + @"</p>
        <div class='footer'>
            <p>Gracias por elegir Barbería Proyecto PM.</p>
            <p>© " + DateTime.Now.Year + @" Barbería Proyecto PM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                    mail.Body = htmlBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(remitente, clave);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar el correo: " + ex.Message);
                return false;
            }
        }

        public static bool EnviarCorreoConNuevoPin(string destinatario, string nuevoPin)
        {
            string remitente = "barberiaproyectopm@gmail.com";  // Tu correo
            string clave = "wrevyexhhoznzyok";  // Tu contraseña de aplicación de Gmail

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(remitente);
                    mail.To.Add(destinatario);
                    mail.Subject = "🔑 Nuevo código PIN de verificación"; // Asunto actualizado

                    // Cuerpo del correo con estilos embebidos en línea
                    string htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; text-align: center; padding: 20px; background-color: #f4f4f4; margin: 0;'>
    <div style='max-width: 400px; width: 100%; padding: 20px; background: #ffffff; border-radius: 8px; 
                box-shadow: 0 4px 8px rgba(0,0,0,0.1); text-align: center; margin: auto;'>
        <div style='font-size: 24px; font-weight: bold; color: #444; margin-bottom: 20px;'>💈 Barbería Proyecto 💈</div>
        <h2 style='color: #333;'>¡Hola!</h2>
        <p style='color: #555;'>Aquí tienes tu nuevo código de verificación:</p>
        <div style='font-size: 28px; font-weight: bold; color: #ffffff; background: #007bff; padding: 15px;
                    border-radius: 5px; display: inline-block; margin-top: 15px; letter-spacing: 3px;'>
            {nuevoPin}
        </div>
        <p style='color: #555;'>Este código expirará en unos minutos. No lo compartas con nadie.</p>
        <div style='font-size: 14px; color: #e74c3c; margin-top: 15px;'>
            <p>Si tu código ha expirado, por favor solicita uno nuevo desde la aplicación.</p>
        </div>
        <div style='font-size: 12px; color: #666; margin-top: 20px;'>
            <p>Si no solicitaste este código, puedes ignorar este mensaje.</p>
            <p>© {DateTime.Now.Year} Barbería Proyecto PM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                    mail.Body = htmlBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) // Puerto 587 con TLS
                    {
                        smtp.Credentials = new NetworkCredential(remitente, clave);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return true;  // El correo se ha enviado exitosamente.
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar el correo: " + ex.Message);
                return false;  // Hubo un error al enviar el correo.
            }
        }

        public static bool EnviarFacturaProductosConPdf(Factura factura)
        {
            string remitente = "barberiaproyectopm@gmail.com";
            string clave = "wrevyexhhoznzyok";

            try
            {
                // Cuerpo del correo en formato HTML bonito
                string cuerpoCorreo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            padding: 20px;
            margin: 0;
        }}
        .container {{
            max-width: 500px;
            margin: auto;
            background: #fff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            text-align: center;
        }}
        h2 {{
            color: #333;
        }}
        p {{
            color: #555;
        }}
        .footer {{
            font-size: 12px;
            color: #888;
            margin-top: 30px;
        }}
        .total {{
            font-size: 18px;
            font-weight: bold;
            color: #222;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='barberia-title'>💈 Barbería Proyecto PM 💈</div>
        <h2>🧾 ¡Gracias por tu compra, {factura.usuario.nombre}!</h2>
        <p>Adjuntamos el PDF con los detalles de tu factura.</p>
        <p><strong>📅 Fecha:</strong> {factura.fecha:dd/MM/yyyy hh:mm tt}</p>
        <p class='total'>💵 Total: ₡{factura.total:N2}</p>
        <div class='footer'>
            <p>Gracias por elegir Barbería Proyecto PM.</p>
            <p>© {DateTime.Now.Year} Barbería Proyecto PM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                // Crear PDF
                byte[] archivoPdf = CrearPdfFacturaProductos(factura);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(remitente);
                    mail.To.Add(factura.usuario.correoElectronico);
                    mail.Subject = "📄 Tu Factura de Barbería Proyecto";
                    mail.Body = cuerpoCorreo;
                    mail.IsBodyHtml = true;

                    // Adjuntar PDF
                    mail.Attachments.Add(new Attachment(new MemoryStream(archivoPdf), $"Factura_{factura.idFactura}.pdf"));

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(remitente, clave);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar el correo con factura: " + ex.Message);
                return false;
            }
        }

        public static byte[] CrearPdfFacturaProductos(Factura factura)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40f, 40f, 40f, 40f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                // Encabezado
                doc.Add(new Paragraph("💈 Barbería Proyecto PM", tituloFont));
                doc.Add(new Paragraph($"Factura N°: {factura.idFactura}", normalFont));
                doc.Add(new Paragraph($"Fecha: {factura.fecha:dd/MM/yyyy hh:mm tt}", normalFont));
                doc.Add(new Paragraph($"Cliente: {factura.usuario.nombre}", normalFont));
                doc.Add(new Paragraph($"Correo: {factura.usuario.correoElectronico}", normalFont));
                doc.Add(new Paragraph(" ")); // Espacio

                // Tabla de productos
                PdfPTable tabla = new PdfPTable(4);
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 40, 20, 20, 20 });

                // Encabezados de tabla
                tabla.AddCell(new PdfPCell(new Phrase("Producto", tituloFont)));
                tabla.AddCell(new PdfPCell(new Phrase("Cantidad", tituloFont)));
                tabla.AddCell(new PdfPCell(new Phrase("Precio Unitario", tituloFont)));
                tabla.AddCell(new PdfPCell(new Phrase("Subtotal", tituloFont)));

                // Productos
                foreach (var p in factura.productos)
                {
                    tabla.AddCell(new PdfPCell(new Phrase(p.nombre, normalFont)));
                    tabla.AddCell(new PdfPCell(new Phrase(p.cantidad.ToString(), normalFont)));
                    tabla.AddCell(new PdfPCell(new Phrase($"₡{p.precio:N2}", normalFont)));
                    tabla.AddCell(new PdfPCell(new Phrase($"₡{(p.precio * p.cantidad):N2}", normalFont)));
                }

                doc.Add(tabla);
                doc.Add(new Paragraph(" ")); // Espacio

                // Totales
                doc.Add(new Paragraph($"Subtotal: ₡{factura.subtotal:N2}", normalFont));
                doc.Add(new Paragraph($"Total: ₡{factura.total:N2}", normalFont));
                doc.Add(new Paragraph($"Estado: {factura.estado}", normalFont));

                doc.Close();
                return ms.ToArray();
            }
        }

        public static bool EnviarFacturaCitaConPdf(Factura factura)
        {
            string remitente = "barberiaproyectopm@gmail.com";
            string clave = "wrevyexhhoznzyok";

            try
            {
                string cuerpoCorreo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            padding: 20px;
            margin: 0;
        }}
        .container {{
            max-width: 500px;
            margin: auto;
            background: #fff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            text-align: center;
        }}
        h2 {{ color: #333; }}
        p {{ color: #555; }}
        .footer {{
            font-size: 12px;
            color: #888;
            margin-top: 30px;
        }}
        .total {{
            font-size: 18px;
            font-weight: bold;
            color: #222;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='barberia-title'>💈 Barbería Proyecto PM 💈</div>
        <h2>🧾 ¡Gracias por tu cita, {factura.usuario.nombre}!</h2>
        <p>Adjuntamos el PDF con los detalles de tu factura.</p>
        <p><strong>📅 Fecha de la cita:</strong> {factura.cita.fechaHora:dd/MM/yyyy hh:mm tt}</p>
        <p><strong>💇 Servicio:</strong> {factura.cita.servicio.nombre}</p>
        <p class='total'>💵 Total: ₡{factura.total:N2}</p>
        <div class='footer'>
            <p>Gracias por elegir Barbería Proyecto PM.</p>
            <p>© {DateTime.Now.Year} Barbería Proyecto PM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                byte[] archivoPdf = CrearPdfFacturaCita(factura);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(remitente);
                    mail.To.Add(factura.usuario.correoElectronico);
                    mail.Subject = "📄 Tu Factura de Barbería Proyecto";
                    mail.Body = cuerpoCorreo;
                    mail.IsBodyHtml = true;
                    mail.Attachments.Add(new Attachment(new MemoryStream(archivoPdf), $"Factura_{factura.idFactura}.pdf"));

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(remitente, clave);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar el correo con factura de cita: " + ex.Message);
                return false;
            }
        }

        public static byte[] CrearPdfFacturaCita(Factura factura)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40f, 40f, 40f, 40f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph("💈 Barbería Proyecto PM", tituloFont));
                doc.Add(new Paragraph($"Factura N°: {factura.idFactura}", normalFont));
                doc.Add(new Paragraph($"Fecha: {factura.fecha:dd/MM/yyyy hh:mm tt}", normalFont));
                doc.Add(new Paragraph($"Cliente: {factura.usuario.nombre}", normalFont));
                doc.Add(new Paragraph($"Correo: {factura.usuario.correoElectronico}", normalFont));
                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph("📋 Detalles del Servicio", tituloFont));
                doc.Add(new Paragraph($"Servicio: {factura.cita.servicio.nombre}", normalFont));
                doc.Add(new Paragraph($"Precio: ₡{factura.cita.servicio.precio:N2}", normalFont));
                doc.Add(new Paragraph($"Fecha de la cita: {factura.cita.fechaHora:dd/MM/yyyy hh:mm tt}", normalFont));
                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph($"Subtotal: ₡{factura.subtotal:N2}", normalFont));
                doc.Add(new Paragraph($"Total: ₡{factura.total:N2}", normalFont));
                doc.Add(new Paragraph($"Estado: {factura.estado}", normalFont));

                doc.Close();
                return ms.ToArray();
            }
        }

    }
}