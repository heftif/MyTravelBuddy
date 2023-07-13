namespace MyTravelBuddy;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertService;
    public static ISqlDatabase DatabaseService;

    public App(IServiceProvider provider)
	{
		InitializeComponent();

        //these services can then directly be accessed via the app
        Services = provider;
        AlertService = Services.GetService<IAlertService>();
        DatabaseService = Services.GetService<ISqlDatabase>();

        MainPage = new AppShell();
	}
}

