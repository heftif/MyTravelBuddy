using CommunityToolkit.Maui.Views;

namespace MyTravelBuddy.Views;

public partial class NewTourPopUpView : Popup
{
	public NewTourPopUpView(string title, IList<Vehicle> vehicles)
	{
		InitializeComponent();
        selectItem.ItemsSource = vehicles;
        titleLabel.Text = title;
    }

    public NewTourPopUpView(string title, IList<TourType> tourTypes)
    {
        InitializeComponent();
        selectItem.ItemsSource = tourTypes;
        titleLabel.Text = title;
    }



    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        //return a value on close of the pop up

        this.CloseAsync(selectItem.CurrentItem);
    }
}
