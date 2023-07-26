namespace MyTravelBuddy.Views;

public partial class DailyPlannerView : ContentPage
{
	public DailyPlannerView(DailyPlannerViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
