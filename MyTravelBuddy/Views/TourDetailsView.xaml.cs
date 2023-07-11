namespace MyTravelBuddy.Views;

public partial class TourDetailsView : ContentPage
{
	public TourDetailsView(TourDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
