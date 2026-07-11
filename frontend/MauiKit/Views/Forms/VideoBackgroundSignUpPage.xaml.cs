namespace MauiKit.Views.Forms;

public partial class VideoBackgroundSignUpPage : ContentPage
{
	public VideoBackgroundSignUpPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        MediaPlayer.Stop();
        base.OnDisappearing();
    }

    private void SignupButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.DisplayAlert("Button Clicked!", "Please add your function.", "OK");
    }
    private async void GoBack_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}