using homefinderpro.AdminViewModels;
using homefinderpro.LandlordViewModels;
using System.Collections.ObjectModel;

namespace homefinderpro.landlord;

public partial class Home : ContentPage
{

    private AdminApprovalViewModel _adminApprovalViewModel;
    public ObservableCollection<LandlordPostViewModel> LandlordPost { get; set; }
    public Home()
	{
		InitializeComponent();
        _adminApprovalViewModel = new AdminApprovalViewModel();
        BindingContext = _adminApprovalViewModel;

        
    }

    

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    
}