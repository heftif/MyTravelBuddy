using CommunityToolkit.Maui.Views;

namespace MyTravelBuddy.Views;

public partial class TourDatePickerPopUpView : Popup
{
    private bool hasChangedEndDate { get; set; }

	public TourDatePickerPopUpView()
	{
		InitializeComponent();

		datePickerStart.MinimumDate = DateTime.Now;
		datePickerStart.Date = DateTime.Now;
		datePickerStart.MaximumDate = DateTime.Now.AddYears(10);

        datePickerEnd.MinimumDate = DateTime.Now;
        datePickerEnd.Date = DateTime.Now;
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

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        this.Close(new[] { datePickerStart.Date, datePickerEnd.Date });
    }
}
