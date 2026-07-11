using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;

namespace MauiKit.Views.PanelGeneral;

public partial class frmCambioContraCorreo : BasePage
{
	public frmCambioContraCorreo()
	{
		InitializeComponent();
	}

    private async void btnCambiarContraseña_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarCorreoVerificacion(txtCorreo.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Datos incompletos", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqCodigoCambioContra req = new ReqCodigoCambioContra();
            req.correoElectronico = txtCorreo.Text;
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/codigoCambioContra", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResCodigoCambioContra res = new ResCodigoCambioContra();
                res = JsonConvert.DeserializeObject<ResCodigoCambioContra>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario = new Usuario();
                    Sesion.usuario.correoElectronico = req.correoElectronico;
                    await Navigation.PushAsync(new frmCambioContra());
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

    private List<string> ValidarCorreoVerificacion(string correo)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(correo))
        {
            errores.Add("Por favor, ingresa tu dirección de correo electrónico.");
        }
        else if (!helpers.EsCorreoValido(correo))
        {
            errores.Add("La dirección ingresada no es válida. Por favor, verifica que tenga el formato: usuario@dominio.com");
        }
        return errores;
    }
}