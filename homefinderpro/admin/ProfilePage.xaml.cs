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
    
    private async void OnUploadButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var pictureData = await _viewModel.GetPictureDataAsync();

            if (pictureData != null)
            {
                if (BindingContext is ProfilePictureViewModel viewModel)
                {
                    await viewModel.UploadProfilePicture(pictureData);
                }
                else
                {
                    Console.WriteLine("BindingContext is not set correctly.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during profile picture upload: {ex.Message}");
        }
    }
    
   





    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (BindingContext is ProfilePictureViewModel _viewModel)
        {
            // Check if a user is selected
            if (_viewModel.SelectedUserProfile != null)
            {
                // Call the DeleteProfilePicture method
                await _viewModel.DeleteProfilePicture(_viewModel.SelectedUserProfile.Username, _viewModel.SelectedUserProfile.Role);
            }
            else
            {
                Console.WriteLine("No user profile selected for deletion.");
            }
        }
    }






    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

}