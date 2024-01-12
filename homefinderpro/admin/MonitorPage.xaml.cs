using homefinderpro.AdminModels;
using homefinderpro.AdminViewModels;
using homefinderpro.LandlordModels;
using homefinderpro.LandlordViewModels;
using homefinderpro.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.ObjectModel;

namespace homefinderpro.admin;

public partial class MonitorPage : ContentPage
{
    private readonly AdminApprovalViewModel _adminApprovalViewModel;
    private readonly DBConnection _dbConnection;

    public ObservableCollection<LandlordPost> LandlordPostsForApproval { get; set; }
    public MonitorPage()
    {
        InitializeComponent();

        _adminApprovalViewModel = new AdminApprovalViewModel();
        _dbConnection = new DBConnection();
        LandlordPostsForApproval = new ObservableCollection<LandlordPost>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        if (_adminApprovalViewModel != null)
        {
            //LoadAdminApprovalData();
            await LoadLandlordPostsForApproval();
        }
    }


    private async Task LoadLandlordPostsForApproval()
    {
        try
        {
            var landlordPosts = await _adminApprovalViewModel.GetLandlordPostsForApproval();

            LandlordPostsForApproval = new ObservableCollection<LandlordPost>(); // Initialize here

            if (landlordPosts != null)
            {
                foreach (var post in landlordPosts)
                {
                    LandlordPostsForApproval.Add(post);
                }
            }

            OnPropertyChanged(nameof(LandlordPostsForApproval));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading landlord posts: {ex.Message}");

            LandlordPostsForApproval = new ObservableCollection<LandlordPost>();
            OnPropertyChanged(nameof(LandlordPostsForApproval));
        }
    }




    public void HandleItemSelected(LandlordPost selectedLandlordPost)
    {
        AdminApprovalViewModel adminApprovalViewModel = new AdminApprovalViewModel();

        var landlordPostDetailView = new LandlordPostDetailView(selectedLandlordPost, adminApprovalViewModel);
        landlordPostDetailView.BindingContext = new LandlordPostViewModel(selectedLandlordPost);

        Navigation.PushAsync(landlordPostDetailView);

        ApprovalListView.SelectedItem = null;
    }

    private void LoadAdminApprovalData()
    {
        LoadLandlordPostsForApproval();
        var adminCollection = _dbConnection.GetDatabase().GetCollection<AdminApproval>("adminapproval");
        var adminApprovalList = adminCollection.Find(Builders<AdminApproval>.Filter.Empty).ToList();

        var adminApprovalViewModelList = new List<AdminApprovalViewModel>();

        foreach (var adminApproval in adminApprovalList)
        {
            var landlordPost = GetLandlordPost(adminApproval.LandlordPostId);

            var adminApprovalViewModel = new AdminApprovalViewModel
            {
                Id = adminApproval.Id,
                LandlordId = adminApproval.LandlordId,
                LandlordPostId = adminApproval.LandlordPostId,
                ApprovalStatus = adminApproval.ApprovalStatus,
                // Check for null values before assigning
            };

            var landlordPostViewModel = new LandlordPostViewModel(landlordPost);

            adminApprovalViewModelList.Add(adminApprovalViewModel);
        }

        _adminApprovalViewModel.AdminApprovalList = new ObservableCollection<AdminApprovalViewModel>(adminApprovalViewModelList);
        ApprovalListView.ItemsSource = _adminApprovalViewModel.AdminApprovalList;
    }

    private LandlordPost GetLandlordPost(ObjectId landlordPostId)
    {
        var landlordPostCollection = _dbConnection.GetDatabase().GetCollection<LandlordPost>("landlordposts");
        return landlordPostCollection.Find(p => p.Id == landlordPostId).FirstOrDefault();
    }


    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is LandlordPost selectedLandlordPost)
        {
            HandleItemSelected(selectedLandlordPost);
        }
    }

}