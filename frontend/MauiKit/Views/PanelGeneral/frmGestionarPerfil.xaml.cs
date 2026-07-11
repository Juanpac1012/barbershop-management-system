using MauiKit.Entidades;
using Backend.Entidades;
using MauiKit.Views.Acciones;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System.Text;

namespace MauiKit.Views.PanelGeneral;

public partial class frmGestionarPerfil : BasePage
{
	public frmGestionarPerfil()
	{
		InitializeComponent();
        cargarDatos();
    }

    private async void btnActualizarUsuario_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarActualizarUsuario(txtNombre.Text, txtApellido.Text, txtTelefono.Text, lblCorreo.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqActualizarUsuario req = new ReqActualizarUsuario();
            req.usuario = new Usuario();
            req.usuario.idUsuario = Sesion.usuario.idUsuario;
            req.usuario.nombre = txtNombre.Text;
            req.usuario.apellido = txtApellido.Text;
            req.usuario.correoElectronico = lblCorreo.Text;
            req.usuario.telefono = txtTelefono.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuarioCliente/actualizar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResActualizarUsuario res = JsonConvert.DeserializeObject<ResActualizarUsuario>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario.nombre = req.usuario.nombre;
                    Sesion.usuario.apellido = req.usuario.apellido;
                    Sesion.usuario.correoElectronico = req.usuario.correoElectronico;
                    Sesion.usuario.telefono = req.usuario.telefono;
                    cargarDatos();
                    await DisplayAlert("Éxito", "Usuario actualizado correctamente", "Aceptar");
                }
                else
                {
                    string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                    await DisplayAlert("Error", mensajeErrores, "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private async void btnEliminarUsuario_Clicked(object sender, EventArgs e)
    {
        if (Sesion.usuario.idUsuario == 0)
        {
            await DisplayAlert("Error", "La sesión no está activa", "Aceptar");
            return;
        }

        try
        {
            var popup = new AccionBorrarCuenta();
            var tcs = new TaskCompletionSource<bool>();
            popup.DecisionMade += (s, result) =>
            {
                tcs.TrySetResult(result);
            };
            await PopupNavigation.Instance.PushAsync(popup);
            bool confirmacion = await tcs.Task;

            if (confirmacion)
            {
                ReqEliminarUsuario req = new ReqEliminarUsuario();
                req.idUsuario = Sesion.usuario.idUsuario;
                HttpResponseMessage respuestaHttp = new HttpResponseMessage();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuarioCliente/eliminar", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResEliminarUsuario res = JsonConvert.DeserializeObject<ResEliminarUsuario>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Usuario eliminado correctamente.", "Aceptar");
                        Sesion.usuario = null;
                        Sesion.estado = EnumEstadoSesion.cerrada;
                        Navigation.PushAsync(new frmLogin());
                    }
                    else
                    {
                        string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                        await DisplayAlert("Error", mensajeErrores, "Aceptar");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private void cargarDatos()
    {
        txtNombre.Text = Sesion.usuario.nombre;
        txtApellido.Text = Sesion.usuario.apellido;
        txtTelefono.Text = Sesion.usuario.telefono;
        lblCorreo.Text = Sesion.usuario.correoElectronico;
    }

    private List<string> ValidarActualizarUsuario(string nombre, string apellido, string telefono, string correo)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add("Por favor ingrese su nombre completo.");
        }

        if (string.IsNullOrWhiteSpace(apellido))
        {
            errores.Add("Por favor ingrese su apellido completo.");
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            errores.Add("El número de teléfono es requerido.");
        }
        else if (telefono.Length != 8 || !telefono.All(char.IsDigit))
        {
            errores.Add("El teléfono debe contener exactamente 8 dígitos numéricos (sin espacios ni guiones).");
        }

        if (string.IsNullOrWhiteSpace(correo))
        {
            errores.Add("Debe proporcionar una dirección de correo electrónico.");
        }
        else if (!helpers.EsCorreoValido(correo))
        {
            errores.Add("El formato del correo electrónico no es válido. Ejemplo válido: usuario@dominio.com");
        }

        return errores;
    }

}