namespace MyTravelBuddy.Views;

public partial class PlanningView : ContentPage
{
	public PlanningView(PlanningViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
