using homefinderpro.Models;
using homefinderpro.LandlordViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Maps;
using homefinderpro.LandlordModels;
namespace homefinderpro.landlord;

public partial class Post : ContentPage
{
    private readonly LandlordPostViewModel _viewModel;

    
    public Post()
    {
        InitializeComponent();
        var landlordPost = new LandlordPost();

        _viewModel = new LandlordPostViewModel(landlordPost);
        BindingContext = _viewModel;

        CategoryPicker.ItemsSource = _viewModel.Categories;

    }

    private void OnAddPhotoClicked(object sender, EventArgs e)
    {

        _viewModel.OnAddPhotoClicked();
    }

    private void OnUploadValidIdClicked(object sender, EventArgs e)
    {

        _viewModel.UploadValidId();
    }

    private void OnUploadGovernmentDocumentClicked(object sender, EventArgs e)
    {

        _viewModel.UploadGovernmentDocument();
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {

        _viewModel.SubmitPost();

    }










}