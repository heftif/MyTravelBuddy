namespace MyTravelBuddy;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(TourDetailsView), typeof(TourDetailsView));
        Routing.RegisterRoute(nameof(TourOverviewCollectionViewModel), typeof(TourOverviewCollectionViewModel));

        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(SettingsPageViewModel), typeof(SettingsPageViewModel));

        Routing.RegisterRoute(nameof(PlanningView), typeof(PlanningView));
        Routing.RegisterRoute(nameof(PlanningCollectionViewModel), typeof(PlanningCollectionViewModel));

        Routing.RegisterRoute(nameof(DailyPlannerView), typeof(DailyPlannerView));
        Routing.RegisterRoute(nameof(DailyPlannerViewModel), typeof(DailyPlannerViewModel));

        Routing.RegisterRoute(nameof(MapLocationFinderView), typeof(MapLocationFinderView));
        Routing.RegisterRoute(nameof(MapLocationFinderViewModel), typeof(MapLocationFinderViewModel));
    }
}

