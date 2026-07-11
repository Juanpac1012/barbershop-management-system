using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using MauiKit.Views.Acciones;
using System.Text;
namespace MauiKit.Views.PanelGeneral;

public partial class frmVerificacionCodigo : BasePage
{
	public frmVerificacionCodigo()
	{
		InitializeComponent();
	}
    private async void btnVerificacion_Clicked(object sender, EventArgs e)
    {
        string numeroVerificacion = $"{Numero1.Text}{Numero2.Text}{Numero3.Text}{Numero4.Text}{Numero5.Text}";

        var errores = ValidarNumero(numeroVerificacion);
        if (errores.Count > 0)
        {
            await DisplayAlert("Datos incompletos", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqActivarCuenta req = new ReqActivarCuenta();
            req.numeroVerificacion = numeroVerificacion;
            req.correoElectronico = Sesion.usuario.correoElectronico;

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "usuario/activarCuenta", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResActivarCuenta res = new ResActivarCuenta();
                res = JsonConvert.DeserializeObject<ResActivarCuenta>(responseContent);

                if (res.resultado)
                {
                    Sesion.usuario = null;
                    await Navigation.PushAsync(new frmLogin());
                    var popup = new AccionCuentaActivada();
                    await PopupNavigation.Instance.PushAsync(popup);
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

    private async void btnRenviarCodigo_Clicked(object sender, EventArgs e)
    {
        try
        {
            ReqSolicitarCodigo req = new ReqSolicitarCodigo();
            req.correoElectronico = Sesion.usuario.correoElectronico;
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
                    await DisplayAlert("Exito", "Correo enviado", "Aceptar");
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

    private List<string> ValidarNumero(string numeroVerificacion)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(numeroVerificacion))
        {
            errores.Add("Por favor ingrese el código de verificación.");
        }
        else if (numeroVerificacion.Length != 5 || !numeroVerificacion.All(char.IsDigit))
        {
            errores.Add("El código debe contener exactamente 5 dígitos numéricos (ejemplo: 12345).");
        }

        return errores;
    }
}