using MauiKit.Entidades;
using Backend.Entidades;
using Newtonsoft.Json;
using MauiKit.Views.Acciones;
using MauiKit.Views.PanelUsuario;
using MauiKit.Views.PanelBarbero;
using MauiKit.Views.PanelAdmin;
using RGPopup.Maui.Services;
using System.Text;

namespace MauiKit.Views.PanelGeneral;

public partial class frmLogin : ContentPage
{
    private bool isPasswordVisible = true;
    public frmLogin()
	{
		InitializeComponent();
	}
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarDatos(txtCorreo.Text, txtContraseña.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Datos incompletos", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqLogin req = new ReqLogin();
            req.correoElectronico = txtCorreo.Text;
            req.contraseña = txtContraseña.Text;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();

            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/login", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();

                ResLogin res = new ResLogin();
                res.usuario = new Usuario();
                res = JsonConvert.DeserializeObject<ResLogin>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario = new Usuario();

                    Sesion.usuario.nombre = res.usuario.nombre;
                    Sesion.usuario.apellido = res.usuario.apellido;
                    Sesion.usuario.telefono = res.usuario.telefono;
                    Sesion.usuario.correoElectronico = res.usuario.correoElectronico;
                    Sesion.usuario.idUsuario = res.usuario.idUsuario;
                    Sesion.usuario.idRol = res.usuario.idRol;
                    Sesion.estado = EnumEstadoSesion.abierto;

                    if (Sesion.usuario.idRol == EnumRoles.Cliente)
                    {
                        await Navigation.PushAsync(new MainUsuario());
                        var popup = new AccionBienvenida();
                        await PopupNavigation.Instance.PushAsync(popup);
                    }
                    else if (Sesion.usuario.idRol == EnumRoles.Admin)
                    {
                       await Navigation.PushAsync(new MainAdmin());
                       var popup = new AccionBienvenida();
                       await PopupNavigation.Instance.PushAsync(popup);
                    }
                    else
                    {
                       await Navigation.PushAsync(new MainBarbero());
                       var popup = new AccionBienvenida();
                       await PopupNavigation.Instance.PushAsync(popup);
                    }
                }
                else
                {
                    string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                    await DisplayAlert("Error de inicio de sesión", mensajeErrores, "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }

    }

    private async void btnCambioContraseña_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new frmCambioContraCorreo());
    }

    private async void btnRegistro_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new frmRegistro());
    }

    private void btnAlternarContraseña_Clicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        txtContraseña.IsPassword = !isPasswordVisible;

        if (btnAlternarContraseña.ImageSource is FontImageSource fontImageSource)
        {
            fontImageSource.Glyph = isPasswordVisible ?
                MauiKitIcons.Eye : 
                MauiKitIcons.EyeOff;     
        }
    }

    private List<string> ValidarDatos(string correo, string contraseña)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(correo))
        {
            errores.Add("Por favor, ingresa tu dirección de correo electrónico.");
        }
        else if (!helpers.EsCorreoValido(correo))
        {
            errores.Add("La dirección de correo electrónico no tiene un formato válido. Ejemplo: usuario@dominio.com");
        }
        if (string.IsNullOrWhiteSpace(contraseña))
        {
            errores.Add("Por favor, ingresa tu contraseña.");
        }
        return errores;
    }
}