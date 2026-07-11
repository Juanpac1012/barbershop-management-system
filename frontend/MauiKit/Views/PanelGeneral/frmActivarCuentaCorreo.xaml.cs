using MauiKit.Entidades;
using Backend.Entidades;
using Newtonsoft.Json;
using System.Text;

namespace MauiKit.Views.PanelGeneral;

public partial class frmActivarCuentaCorreo : BasePage
{
	public frmActivarCuentaCorreo()
	{
		InitializeComponent();
	}

    private async void ActivarCuenta_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarCorreoVerificacion(txtCorreo.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Datos incompletos", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqSolicitarCodigo req = new ReqSolicitarCodigo();
            req.correoElectronico = txtCorreo.Text;
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/solicitarCodigo", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResSolicitarCodigo res = new ResSolicitarCodigo();
                res = JsonConvert.DeserializeObject<ResSolicitarCodigo>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario = new Usuario();
                    Sesion.usuario.correoElectronico = req.correoElectronico;
                    await Navigation.PushAsync(new frmVerificacionCodigo());
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
            errores.Add("Por favor ingrese su dirección de correo electrónico.");
        }
        else if (!helpers.EsCorreoValido(correo))
        {
            errores.Add("La dirección de correo ingresada no es válida. Debe tener formato: usuario@dominio.com");
        }

        return errores;
    }
}