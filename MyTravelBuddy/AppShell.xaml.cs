namespace MyTravelBuddy;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(TourDetailsView), typeof(TourDetailsView));
        Routing.RegisterRoute(nameof(TourDetailsViewModel), typeof(TourDetailsViewModel));

        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(SettingsPageViewModel), typeof(SettingsPageViewModel));

        Routing.RegisterRoute(nameof(PlanningView), typeof(PlanningView));
        Routing.RegisterRoute(nameof(PlanningViewModel), typeof(PlanningViewModel));

        Routing.RegisterRoute(nameof(DailyPlannerView), typeof(DailyPlannerView));
        Routing.RegisterRoute(nameof(DailyPlannerViewModel), typeof(DailyPlannerViewModel));
    }
}

