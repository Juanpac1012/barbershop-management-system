namespace MauiKit.Views.Properties;

public partial class PropertyBookingPage : BasePage
{
	public PropertyBookingPage()
	{
		InitializeComponent();
		BindingContext = new PropertyBookingViewModel();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}