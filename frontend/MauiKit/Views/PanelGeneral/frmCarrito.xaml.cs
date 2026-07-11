using MauiKit.Entidades;
using Backend.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelGeneral;

public partial class frmCarrito : BasePage, INotifyPropertyChanged
{
    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Carrito> CarritoItems => CarritoServicio.Items;

    public decimal CarritoTotal => CarritoServicio.Total;

    public Command<Carrito> EliminarCommand { get; }
    public Command RefrescarCommand { get; }

    public frmCarrito()
    {
        InitializeComponent();

        RefrescarCommand = new Command(() =>
        {
            ActualizarVista();
            IsRefreshing = false;
        });

        BindingContext = this;

        // Se actualiza vista cuando cambia el carrito
        CarritoServicio.Items.CollectionChanged += (s, e) => ActualizarVista();
    }

    private void ActualizarVista()
    {
        OnPropertyChanged(nameof(CarritoItems));
        OnPropertyChanged(nameof(CarritoTotal)); // Refresca el subtotal visualmente
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ActualizarVista();
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void btnEliminarCarrito_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Carrito item)
        {
            bool respuesta = await DisplayAlert("Confirmar", $"¿Eliminar {item.Nombre} del carrito?", "Sí", "No");

            if (respuesta)
            {
                CarritoServicio.EliminarProducto(item);
                ActualizarVista(); // Refresca vista y subtotal después de eliminar
            }
        }
    }

    private async void btnPagar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var productos = CarritoServicio.Items.Select(p => new
            {
                idProducto = p.Id,
                cantidad = p.Cantidad
            }).ToList();

            ReqInsertarFacturaProductos req = new ReqInsertarFacturaProductos
            {
                factura = new Factura
                {
                    cantidadProductos = productos.Count,
                    usuario = new Usuario { idUsuario = Sesion.usuario.idUsuario },
                    productos = productos.Select(p => new Producto
                    {
                        idProducto = p.idProducto,
                        cantidad = p.cantidad
                    }).ToList()
                }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage respuestaHttp = await httpClient.PostAsync(App.API_URL + "factura/insertarProductos", jsonContent);

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResInsertarFacturaProductos res = JsonConvert.DeserializeObject<ResInsertarFacturaProductos>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Factura creada exitosamente", "Aceptar");
                        CarritoServicio.Items.Clear();
                        ActualizarVista(); 
                    }
                    else
                    {
                        string mensajeErrores = string.Join("\n", res.listaErrores.Select(e => e.Message));
                        await DisplayAlert("Error al crear la factura", mensajeErrores, "Aceptar");
                    }
                }
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }
}
