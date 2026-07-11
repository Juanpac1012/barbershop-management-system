using MauiKit.Entidades;
using Backend.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelGeneral;

public partial class frmCambioContraPerfil1 : BasePage
{
	public frmCambioContraPerfil1()
	{
		InitializeComponent();
	}
    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {

        try
        {
            ReqCodigoCambioContra req = new ReqCodigoCambioContra();
            req.correoElectronico = Sesion.usuario.correoElectronico;
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
                    await Navigation.PushAsync(new frmCambioContraPerfil2());
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
}