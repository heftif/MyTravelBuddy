namespace MyTravelBuddy.Views;

public partial class TourDetailsView : ContentPage
{
	public TourDetailsView(TourDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

        datePickerStart.MaximumDate = DateTime.Now.AddYears(10);
        datePickerEnd.MaximumDate = DateTime.Now.AddYears(10);
    }

    void datePickerStart_DateSelected(System.Object sender, Microsoft.Maui.Controls.DateChangedEventArgs e)
    {
        if (datePickerEnd.Date <= datePickerStart.Date)
            datePickerEnd.Date = datePickerStart.Date.AddDays(1);
    }

    void datePickerEnd_DateSelected(System.Object sender, Microsoft.Maui.Controls.DateChangedEventArgs e)
    {
        if (datePickerEnd.Date <= datePickerStart.Date)
            datePickerEnd.Date = datePickerStart.Date.AddDays(1);
    }
}
