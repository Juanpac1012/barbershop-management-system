using Newtonsoft.Json;
using MauiKit.Entidades;
using Backend.Entidades;
using System.Text;

namespace MauiKit.Views.PanelUsuario;

public partial class frmFacturasUsuario : BasePage
{
    public frmFacturasUsuario()
    {
        InitializeComponent();
        CargarFacturasUsuario();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarFacturasUsuario();
    }

    private async void CargarFacturasUsuario()
    {
        try
        {
            var idUsuario = Sesion.usuario.idUsuario;

            var facturasProductos = new List<Factura>();
            var facturasCitas = new List<Factura>();

            // Cargar facturas de productos
            var reqProductos = new ReqListarFacturasPUsuario { idUsuario = idUsuario };
            var jsonProductos = new StringContent(JsonConvert.SerializeObject(reqProductos), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                var respProd = await httpClient.PostAsync(App.API_URL + "factura/listarFacturasPUsuario", jsonProductos);
                if (respProd.IsSuccessStatusCode)
                {
                    var contenido = await respProd.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResListarFacturasPUsuario>(contenido);
                    if (res.resultado && res.facturas != null)
                    {
                        foreach (var factura in res.facturas)
                        {
                            factura.tipo = "Producto"; // Marcar como producto
                        }
                        facturasProductos.AddRange(res.facturas);
                    }
                }
            }

            // Cargar facturas de citas
            var reqCitas = new ReqListarFacturasCUsuario { idUsuario = idUsuario };
            var jsonCitas = new StringContent(JsonConvert.SerializeObject(reqCitas), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                var respCitas = await httpClient.PostAsync(App.API_URL + "factura/listarFacturasCUsuario", jsonCitas);
                if (respCitas.IsSuccessStatusCode)
                {
                    var contenido = await respCitas.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResListarFacturasCUsuario>(contenido);
                    if (res.resultado && res.facturas != null)
                    {
                        foreach (var factura in res.facturas)
                        {
                            factura.tipo = "Cita"; // Marcar como cita
                        }
                        facturasCitas.AddRange(res.facturas);
                    }
                }
            }

            if (facturasCitas.Any() || facturasProductos.Any())
            {
                // Asignar las listas a cada CollectionView
                facturasCitasCollectionView.ItemsSource = facturasCitas
                    .OrderByDescending(f => f.fecha)
                    .ToList();

                facturasProductosCollectionView.ItemsSource = facturasProductos
                    .OrderByDescending(f => f.fecha)
                    .ToList();
            }
            else
            {
                await DisplayAlert("Información", "No se encontraron facturas", "Aceptar");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "No se pudo conectar con el servidor", "Aceptar");
        }
    }
}
