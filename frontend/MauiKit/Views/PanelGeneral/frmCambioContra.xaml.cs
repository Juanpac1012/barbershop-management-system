using MauiKit.Entidades;
using Newtonsoft.Json;
using Backend.Entidades;
using MauiKit.Views.Acciones;
using RGPopup.Maui.Services;
using System.Text;

namespace MauiKit.Views.PanelGeneral;

public partial class frmCambioContra : BasePage
{
    private bool isPasswordVisible = true;
    private bool isPasswordConfirVisible = true;
    public frmCambioContra()
	{
		InitializeComponent();
	}

    private async void btnCambiarContraseña_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarCambioContra(txtCodigo.Text, txtContraseña.Text, txtConfirContraseña.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Datos incompletos", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqCambiarContraseña req = new ReqCambiarContraseña();
            req.numeroVerificacion = txtCodigo.Text;
            req.contraseña = txtContraseña.Text;
            req.correoElectronico = Sesion.usuario.correoElectronico;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/cambiarContra", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResCambiarContraseña res = new ResCambiarContraseña();
                res = JsonConvert.DeserializeObject<ResCambiarContraseña>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario = null; 
                    var popup = new AccionCambioContraseña();
                    await PopupNavigation.Instance.PushAsync(popup);
                    await Navigation.PushAsync(new frmLogin());
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

    private List<string> ValidarCambioContra(string numeroVerificacion, string contraseña, string confirmContra)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(numeroVerificacion))
        {
            errores.Add("Por favor ingrese el código de verificación enviado a su correo electrónico.");
        }
        else if (numeroVerificacion.Length != 5 || !numeroVerificacion.All(char.IsDigit))
        {
            errores.Add("El código de verificación debe ser un número de exactamente 5 dígitos (ejemplo: 12345).");
        }
        if (string.IsNullOrWhiteSpace(contraseña))
        {
            errores.Add("Debe ingresar una nueva contraseña.");
        }
        else if (!helpers.EsPasswordSeguro(contraseña))
        {
            errores.Add("La contraseña debe contener:\n- 8+ caracteres\n- 1 mayúscula\n- 1 minúscula\n- 1 número\n- 1 carácter especial");
        }
        if (confirmContra != contraseña)
        {
            errores.Add("Las contraseñas no coinciden. Verifique que sean iguales en ambos campos.");
        }
        return errores;
    }

    private void btnAlternarContraseña_Clicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        txtContraseña.IsPassword = !isPasswordVisible;

        // Cambiar el icono del botón
        if (btnAlternarContraseña.ImageSource is FontImageSource fontImageSource)
        {
            fontImageSource.Glyph = isPasswordVisible ?
                MauiKitIcons.Eye :
                MauiKitIcons.EyeOff;
        }
    }

    private void btnAlternarContraseñaConfir_Clicked(object sender, EventArgs e)
    {
        isPasswordConfirVisible = !isPasswordConfirVisible;
        txtConfirContraseña.IsPassword = !isPasswordConfirVisible;

        if (btnAlternarContraseñaConfir.ImageSource is FontImageSource fontImageSource)
        {
            fontImageSource.Glyph = isPasswordConfirVisible ?
                MauiKitIcons.Eye :
                MauiKitIcons.EyeOff;
        }
    }
}