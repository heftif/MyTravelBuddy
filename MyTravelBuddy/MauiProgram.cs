using Microsoft.Extensions.Logging;

namespace MyTravelBuddy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        //services
        builder.Services.AddSingleton<SqlDatabase>();

        //main pages (singleton means we create the page just once)
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        //tour
        builder.Services.AddTransient<TourDetailsViewModel>();
        builder.Services.AddTransient<TourDetailsView>();

        return builder.Build();
	}
}

