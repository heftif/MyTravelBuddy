using CommunityToolkit.Maui.Views;
using Plugin.Maui.CalendarStore;

namespace MyTravelBuddy.Views;

public partial class ChooseCalendarView : Popup
{

    public ChooseCalendarView(string title, IEnumerable<Calendar> calendars)
    {
        InitializeComponent();
        selectItem.ItemsSource = calendars.ToList();
        titleLabel.Text = title;
        selectItem.SelectionMode = SelectionMode.Single;
    }


    void Button_Clicked(System.Object sender, System.EventArgs e)
    {

        //return a value on close of the pop up
        this.CloseAsync(selectItem.SelectedItem);

    }
}
