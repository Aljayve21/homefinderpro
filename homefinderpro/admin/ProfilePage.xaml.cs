using homefinderpro.AdminViewModels;
namespace homefinderpro.admin;

public partial class ProfilePage : ContentPage
{

    private readonly ProfilePictureViewModel _viewModel;

    public ProfilePage()
    {
        InitializeComponent();
        _viewModel = new ProfilePictureViewModel();
        BindingContext = _viewModel;


    }

    /*
    private async void UploadPictureButton_Clicked(object sender, EventArgs e)
    {
        var stream = _viewModel.GetPictureStream();
        if (stream != null)
        {
            var byteArray = _viewModel.ReadFully(stream);
            await _viewModel.SaveProfilePictureToDatabase("username", "user_role", ImageSource.FromStream(() => new MemoryStream(byteArray)));
        }
    }
    */









    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

}