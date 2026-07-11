using Backend.Entidades;
using Newtonsoft.Json;
using System.Text;

namespace MauiKit.Views.PanelGeneral;

public partial class frmServicios : BasePage
{
	public frmServicios()
	{
		InitializeComponent();
        cargarServicios();
    }
    private async void cargarServicios()
    {
        try
        {
            ReqListarServicio req = new ReqListarServicio();
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "servicio/listar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarServicio res = JsonConvert.DeserializeObject<ResListarServicio>(responseContent);
                if (res.resultado)
                {
                    servicioCollectionView.ItemsSource = res.servicio;
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron materias", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        cargarServicios();
    }

    private void btnAgendar_Clicked(object sender, EventArgs e)
    {
        if (Parent is TabbedPage tabbedPage)
        {
            var serviciosPage = tabbedPage.Children.FirstOrDefault(p => p.Title == "Agendar Cita");

            if (serviciosPage != null)
            {
                tabbedPage.CurrentPage = serviciosPage;
            }
        }
    }
}