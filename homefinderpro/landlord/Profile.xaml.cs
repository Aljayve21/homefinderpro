namespace homefinderpro.landlord;

public partial class Profile : ContentPage
{
	public Profile()
	{
		InitializeComponent();
	}

    private async void landlordOnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}