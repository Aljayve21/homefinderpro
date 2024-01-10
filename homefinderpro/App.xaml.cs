namespace homefinderpro;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx0QXxbf1xzZFdMYFlbRXVPMyBoS35RdURhWHhecnRQR2NfVUZw");
		MainPage = new AppShell();
	}

    protected override void OnStart()
    {
        base.OnStart();

		Shell.Current.GoToAsync("//MainPage");
    }
}
