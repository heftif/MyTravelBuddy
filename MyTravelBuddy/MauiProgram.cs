using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace MyTravelBuddy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if ANDROID || IOS
		builder.UseLocalNotification();
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

        //services
        builder.Services.AddSingleton<ISqlDatabase, SqlDatabase>();
		builder.Services.AddSingleton <IAlertService, AlertService>();
        builder.Services.AddSingleton<IShellNavigationService, ShellNavigationService>();
        builder.Services.AddSingleton<ImageUploadService>();
		//builder.Services.AddSingleton<INotificationService>();

        //main pages (singleton means we create the page just once)
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        //tour
        builder.Services.AddTransient<TourDetailsViewModel>();
        builder.Services.AddTransient<TourDetailsView>();

		//planning
		builder.Services.AddTransient<PlanningViewModel>();
		builder.Services.AddTransient<PlanningView>();

        //daily list
        builder.Services.AddTransient<DailyPlannerViewModel>();
        builder.Services.AddTransient<DailyPlannerView>();

        //settings page
        builder.Services.AddSingleton<SettingsPage>();
		builder.Services.AddSingleton<SettingsPageViewModel>();


        return builder.Build();
	}
}

