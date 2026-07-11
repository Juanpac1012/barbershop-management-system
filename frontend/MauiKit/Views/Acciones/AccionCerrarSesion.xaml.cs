using RGPopup.Maui.Pages;
using RGPopup.Maui.Services;

namespace MauiKit.Views.Acciones;

public partial class AccionCerrarSesion : PopupPage
{
    public event EventHandler<bool> DecisionMade;
    public AccionCerrarSesion()
	{
		InitializeComponent();
	}
    private async void AcceptButton_Clicked(object sender, EventArgs e)
    {
        DecisionMade?.Invoke(this, true);
        await PopupNavigation.Instance.PopAsync();
    }

    private async void RejectButton_Clicked(object sender, EventArgs e)
    {
        DecisionMade?.Invoke(this, false);
        await PopupNavigation.Instance.PopAsync();
    }

}