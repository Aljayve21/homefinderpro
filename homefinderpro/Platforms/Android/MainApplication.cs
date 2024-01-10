using Android.App;
using Android.Runtime;

namespace homefinderpro;

[Application]
[MetaData("com.google.android.maps.v2.API_KEY",
	Value = "AIzaSyBcUDWZDnJBOX_Q5IOqDJi60RuqJy1-ZkY")]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
