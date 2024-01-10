using Maui.GoogleMaps.Hosting;
using Syncfusion.Maui.Core.Hosting;
using Microsoft.Extensions.Logging;

namespace homefinderpro;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.ConfigureSyncfusionCore()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
#if ANDROID
		builder.UseGoogleMaps();
#elif IOS
		builder.UseGoogleMaps("AIzaSyBcUDWZDnJBOX_Q5IOqDJi60RuqJy1-ZkY");
#endif

        return builder.Build();
	}
}
