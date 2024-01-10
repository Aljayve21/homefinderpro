using homefinderpro.AdminViewModels;
using homefinderpro.LandlordModels;
using homefinderpro.LandlordViewModels;

namespace homefinderpro.admin;

public partial class LandlordPostDetailView : ContentPage
{
    private AdminApprovalViewModel _adminApprovalViewModel;

    public LandlordPostDetailView(LandlordPost viewModel, AdminApprovalViewModel adminApprovalViewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
        _adminApprovalViewModel = adminApprovalViewModel;
    }

    private async void OnApproveClicked(object sender, EventArgs e)
    {
        
        if (BindingContext is LandlordPostViewModel viewModel && viewModel.Id != null)
        {
            var landlordPostId = viewModel.Id;
            await _adminApprovalViewModel.ApproveLandlordPost(landlordPostId);

            
            Navigation.PopAsync();
        }
    }

    private async void OnRejectClicked(object sender, EventArgs e)
    {
        
        if (BindingContext is LandlordPostViewModel viewModel && viewModel.Id != null)
        {
            var landlordPostId = viewModel.Id;
            await _adminApprovalViewModel.RejectLandlordPost(landlordPostId);

            
            Navigation.PopAsync();
        }
    }


}