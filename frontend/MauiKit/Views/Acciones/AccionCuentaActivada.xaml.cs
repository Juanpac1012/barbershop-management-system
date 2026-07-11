using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;

namespace MauiKit.Views.Acciones;

public partial class AccionCuentaActivada : PopupPage
{
	public AccionCuentaActivada()
	{
		InitializeComponent();
	}
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigation.Instance.PopAsync();
    }
}