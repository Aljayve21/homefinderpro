using homefinderpro.ChatModels;

namespace homefinderpro.landlord;

public partial class CustomerService : ContentPage
{
	private LiveChat _liveChat;
	public CustomerService()
	{
		InitializeComponent();
		_liveChat = new LiveChat(new Models.DBConnection());
	}

	private void SendMessageToCustomer(string customerId, string message)
	{

	}
}