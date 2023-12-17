using homefinderpro.AdminViewModels;

namespace homefinderpro;

public partial class SignupPage : ContentPage
{
	private SignupViewModel viewModel;
	public SignupPage()
	{
		InitializeComponent();
		viewModel = new SignupViewModel();
		BindingContext = viewModel;

		InitializeControls();
	}

	private void InitializeControls()
	{
		RolePicker.ItemsSource = new[] { "admin", "landlord", "customer"};
	}

	private async void SignupButton_Click(object sender, EventArgs e)
	{
		string selectedRole = RolePicker.SelectedItem as string;
		await viewModel.Signup("password", selectedRole);
	}
}