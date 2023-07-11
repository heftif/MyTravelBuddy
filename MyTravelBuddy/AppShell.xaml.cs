namespace MyTravelBuddy;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(TourDetailsView), typeof(TourDetailsView));
        Routing.RegisterRoute(nameof(TourDetailsViewModel), typeof(TourDetailsViewModel));
    }
}

