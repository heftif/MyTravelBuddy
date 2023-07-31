namespace MyTravelBuddy.Views;

public partial class PlanningView : ContentPage
{
	public PlanningView(PlanningCollectionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

}
