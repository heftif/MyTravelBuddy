namespace MyTravelBuddy.Views;

public partial class WayPointDisplayView : ContentPage
{
	public WayPointDisplayView(WayPointDisplayViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
