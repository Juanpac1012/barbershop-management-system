using Newtonsoft.Json;
using System.Text;
using Backend.Entidades;
using MauiKit.Entidades;
namespace MauiKit.Views.PanelGeneral;

public partial class frmProductos : BasePage
{
	public frmProductos()
	{
		InitializeComponent();
        cargarProductos();
    }
    private async void cargarProductos()
    {
        try
        {
            ReqListarProducto req = new ReqListarProducto();
            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "producto/listar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResListarProducto res = JsonConvert.DeserializeObject<ResListarProducto>(responseContent);
                if (res.resultado)
                {
                    productosCollectionView.ItemsSource = res.producto;
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

    private async void AgregarAlCarritoClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Producto producto)
        {
            CarritoServicio.AgregarProducto(producto);
            await DisplayAlert("Exito", "Producto agregado", "Aceptar");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        cargarProductos();
    }
}