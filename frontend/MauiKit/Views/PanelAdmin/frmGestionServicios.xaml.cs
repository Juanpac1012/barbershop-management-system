using Backend.Entidades;
using MauiKit.Entidades;
using Newtonsoft.Json;
using System.Text;
namespace MauiKit.Views.PanelAdmin;

public partial class frmGestionServicios : BasePage
{
    private int? servicioSeleccionadoId;
    private int? productoSeleccionadoId;
    public frmGestionServicios()
	{
		InitializeComponent();
        cargarServicio();
        LimpiarCampos();
        cargarProductos();

        cmbServicio.SelectedIndexChanged += (s, e) =>
        {
            var servicioSeleccionado = cmbServicio.SelectedItem as Servicio;
            if (servicioSeleccionado != null && servicioSeleccionado.idServicio != 0)
            {
                servicioSeleccionadoId = servicioSeleccionado.idServicio;
                txtNombreServicio.Text = servicioSeleccionado.nombre;
                txtDescripcionServicio.Text = servicioSeleccionado.descripcion;
                txtPrecioServicio.Text = servicioSeleccionado.precio.ToString();
                txtDuracion.Text = servicioSeleccionado.duracion_minutos.ToString();
            }
            else
            {
                servicioSeleccionadoId = null;
                LimpiarCampos();
            }
        };

        cmbProducto.SelectedIndexChanged += (s, e) =>
        {
            var productoSeleccionado = cmbProducto.SelectedItem as Producto;
            if (productoSeleccionado != null && productoSeleccionado.idProducto != 0)
            {
                productoSeleccionadoId = productoSeleccionado.idProducto;
                txtNombreProducto.Text = productoSeleccionado.nombre;
                txtDescripcionProducto.Text = productoSeleccionado.descripcion;
                txtPrecioProducto.Text = productoSeleccionado.precio.ToString();
                txtStock.Text = productoSeleccionado.stock.ToString();
            }
            else
            {
                productoSeleccionadoId = null;
                LimpiarCampos();
            }
        };
    }

    #region Métodos para Productos
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
                    var listaProductos = new List<Producto> { new Producto { idProducto = 0, nombre = "     " } };
                    listaProductos.AddRange(res.producto);
                    cmbProducto.ItemsSource = listaProductos;
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron productos.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }

    private async void btnEliminarProducto_Clicked(object sender, EventArgs e)
    {
        if (productoSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un producto para eliminar.", "Aceptar");
            return;
        }

        bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar este producto?", "Sí, eliminar", "Cancelar");
        if (confirmar)
        {
            try
            {
                ReqEliminarProducto req = new ReqEliminarProducto();
                req.idProducto = productoSeleccionadoId.Value;

                HttpResponseMessage respuestaHttp = new HttpResponseMessage();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "producto/eliminar", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResEliminarProducto res = JsonConvert.DeserializeObject<ResEliminarProducto>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Producto eliminado correctamente.", "Aceptar");
                        cargarProductos();
                        LimpiarCampos();
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

    private async void btnActualizarProducto_Clicked(object sender, EventArgs e)
    {
        if (productoSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un producto para actualizar.", "Aceptar");
            return;
        }

        var errores = ValidarProducto(txtNombreProducto.Text, txtDescripcionProducto.Text, txtPrecioProducto.Text, txtStock.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqActualizarProducto req = new ReqActualizarProducto();
            req.producto = new Producto();
            req.producto.idProducto = productoSeleccionadoId.Value;
            req.producto.nombre = txtNombreProducto.Text;
            req.producto.descripcion = txtDescripcionProducto.Text;
            req.producto.precio = Convert.ToDecimal(txtPrecioProducto.Text);
            req.producto.stock = Convert.ToInt32(txtStock.Text);

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "producto/actualizar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResActualizarProducto res = JsonConvert.DeserializeObject<ResActualizarProducto>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Producto actualizado correctamente.", "Aceptar");
                    cargarProductos();
                    LimpiarCampos();
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

    private async void btnAgregarProducto_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarProducto(txtNombreProducto.Text, txtDescripcionProducto.Text, txtPrecioProducto.Text, txtStock.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }

        try
        {
            ReqInsertarProducto req = new ReqInsertarProducto();
            req.producto = new Producto();
            req.producto.nombre = txtNombreProducto.Text;
            req.producto.descripcion = txtDescripcionProducto.Text;
            req.producto.precio = Convert.ToDecimal(txtPrecioProducto.Text);
            req.producto.stock = Convert.ToInt32(txtStock.Text);

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "producto/insertar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarProducto res = JsonConvert.DeserializeObject<ResInsertarProducto>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Producto agregado correctamente.", "Aceptar");
                    cargarProductos();
                    LimpiarCampos();
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

    private List<string> ValidarProducto(string nombre, string descripcion, string precioTexto, string stockTexto)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add("Nombre del producto requerido. Por favor ingrese un nombre para el producto.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            errores.Add("Descripción requerida. Por favor agregue una descripción del producto.");
        }
        else if (descripcion.Length < 10 || descripcion.Length > 200)
        {
            errores.Add("La descripción debe contener entre 10 y 200 caracteres (actual: " + descripcion.Length + " caracteres).");
        }
        if (!decimal.TryParse(precioTexto, out decimal precio))
        {
            errores.Add("Formato de precio inválido. Ingrese un valor numérico (ej: 25.99).");
        }
        else if (precio < 1 || precio > 100000)
        {
            errores.Add("El precio debe estar entre ₡1.00 y ₡100,000.00 (valor ingresado: " + precio.ToString("C") + ").");
        }
        if (!int.TryParse(stockTexto, out int stock))
        {
            errores.Add("Formato de cantidad inválido. Ingrese un número entero (ej: 50).");
        }
        else if (stock < 0 || stock > 100000)
        {
            errores.Add("La cantidad en stock debe estar entre 0 y 100,000 unidades (valor ingresado: " + stock + ").");
        }

        return errores;
    }

    #endregion

    #region Métodos para servicios
    private async void btnEliminarServicio_Clicked(object sender, EventArgs e)
    {
        if (servicioSeleccionadoId == 0)
        {
            await DisplayAlert("Error", "Debe seleccionar un servicio para eliminar.", "Aceptar");
            return;
        }

        bool confirmar = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar este servicio?", "Sí, eliminar", "Cancelar");
        if (confirmar)
        {

            try
            {
                ReqEliminarServicio req = new ReqEliminarServicio();
                req.idServicio = servicioSeleccionadoId.Value; //revisar

                HttpResponseMessage respuestaHttp = new HttpResponseMessage();
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync(App.API_URL + "servicio/eliminar", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    ResEliminarServicio res = new ResEliminarServicio();
                    res = JsonConvert.DeserializeObject<ResEliminarServicio>(responseContent);

                    if (res.resultado)
                    {
                        await DisplayAlert("Éxito", "Servicio eliminado correctamente.", "Aceptar");
                        cargarServicio();
                        LimpiarCampos();
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

    private async void btnActualizarServicio_Clicked(object sender, EventArgs e)
    {
        if (servicioSeleccionadoId == 0) //revisar
        {
            await DisplayAlert("Error", "Debe seleccionar un servicio para actualizar.", "Aceptar");
            return;
        }

        var errores = ValidarServicio(txtNombreServicio.Text, txtDescripcionServicio.Text, txtPrecioServicio.Text, txtDuracion.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqActualizarServicio req = new ReqActualizarServicio();
            req.servicio = new Servicio();
            req.servicio.idServicio = servicioSeleccionadoId.Value;//revisar
            req.servicio.nombre = txtNombreServicio.Text;
            req.servicio.descripcion = txtDescripcionServicio.Text;
            req.servicio.precio = Convert.ToDecimal(txtPrecioServicio.Text);
            req.servicio.duracion_minutos = Convert.ToInt32(txtDuracion.Text);

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "servicio/actualizar", jsonContent);
            }

            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResActualizarServicio res = new ResActualizarServicio();
                res = JsonConvert.DeserializeObject<ResActualizarServicio>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Servicio actualizado correctamente.", "Aceptar");
                    cargarServicio();
                    LimpiarCampos();
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

    private async void btnAgregarServicio_Clicked(object sender, EventArgs e)
    {
        var errores = ValidarServicio(txtNombreServicio.Text, txtDescripcionServicio.Text, txtPrecioServicio.Text, txtDuracion.Text);
        if (errores.Count > 0)
        {
            await DisplayAlert("Error", string.Join("\n", errores), "Aceptar");
            return;
        }
        try
        {
            ReqInsertarServicio req = new ReqInsertarServicio();
            req.servicio = new Servicio();
            req.servicio.nombre = txtNombreServicio.Text;
            req.servicio.descripcion = txtDescripcionServicio.Text;
            req.servicio.precio = Convert.ToDecimal(txtPrecioServicio.Text);
            req.servicio.duracion_minutos = Convert.ToInt32(txtDuracion.Text);

            HttpResponseMessage respuestaHttp = new HttpResponseMessage();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                respuestaHttp = await httpClient.PostAsync(App.API_URL + "servicio/insertar", jsonContent);
            }
            if (respuestaHttp.IsSuccessStatusCode)
            {
                var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                ResInsertarServicio res = new ResInsertarServicio();
                res = JsonConvert.DeserializeObject<ResInsertarServicio>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("Éxito", "Servicio registrado correctamente.", "Aceptar");
                    cargarServicio();
                    LimpiarCampos();
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

    private List<string> ValidarServicio(string nombre, string descripcion, string precioTexto, string duracionMinutosTexto)
    {
        List<string> errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add("Nombre del servicio requerido. Por favor ingrese un nombre para el servicio.");
        }

        if (string.IsNullOrWhiteSpace(descripcion))
        {
            errores.Add("Descripción requerida. Por favor agregue una descripción del servicio.");
        }
        else if (descripcion.Length < 10 || descripcion.Length > 200)
        {
            errores.Add("La descripción debe contener entre 10 y 200 caracteres (actual: " + descripcion.Length + " caracteres).");
        }

        if (!decimal.TryParse(precioTexto, out decimal precio))
        {
            errores.Add("Formato de precio inválido. Ingrese un valor numérico (ej: 15000).");
        }
        else if (precio < 1 || precio > 100000)
        {
            errores.Add("El precio debe estar entre ₡1.00 y ₡100,000.00 (valor ingresado: " + precio.ToString("C") + ").");
        }

        if (!int.TryParse(duracionMinutosTexto, out int duracionMinutos))
        {
            errores.Add("Formato de duración inválido. Ingrese minutos en números enteros (ej: 45).");
        }
        else if (duracionMinutos < 30 || duracionMinutos > 300)
        {
            errores.Add("La duración debe estar entre 30 y 300 minutos (5 horas) (valor ingresado: " + duracionMinutos + " minutos).");
        }
        return errores;
    }

    private async void cargarServicio()
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
                ResListarSercicio res = JsonConvert.DeserializeObject<ResListarSercicio>(responseContent);

                if (res.resultado)
                {
                    var listaServicio = new List<Servicio> { new Servicio { idServicio = 0, nombre = "     " } }; // Opción vacía, revisar
                    listaServicio.AddRange(res.servicio);

                    cmbServicio.ItemsSource = listaServicio; // Asigna la lista de productos al Picker
                    cmbServicio.ItemDisplayBinding = new Binding("nombre"); // Muestra solo el nombre del producto
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron productos.", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la lista de productos.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error de conexión", "No hay respuesta del servidor", "Aceptar");
        }
    }
    #endregion
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LimpiarCampos();
        cargarServicio();
        cargarProductos();
    }

    private void LimpiarCampos()
    {
        // Limpiar campos de servicios
        servicioSeleccionadoId = null;
        cmbServicio.SelectedIndex = -1;
        txtNombreServicio.Text = "";
        txtDescripcionServicio.Text = "";
        txtPrecioServicio.Text = "";
        txtDuracion.Text = "";

        // Limpiar campos de productos
        productoSeleccionadoId = null;
        cmbProducto.SelectedIndex = -1;
        txtNombreProducto.Text = "";
        txtDescripcionProducto.Text = "";
        txtPrecioProducto.Text = "";
        txtStock.Text = "";
    }
}