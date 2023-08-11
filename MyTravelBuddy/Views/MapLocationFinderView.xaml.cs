namespace MyTravelBuddy.Views;

public partial class MapLocationFinderView : ContentPage
{
	public MapLocationFinderView(MapLocationFinderViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected async override void OnAppearing()
    {
        await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

    }
}
