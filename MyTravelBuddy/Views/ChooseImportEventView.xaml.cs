using CommunityToolkit.Maui.Views;
using Plugin.Maui.CalendarStore;

namespace MyTravelBuddy.Views;

public partial class ChooseImportEventView : Popup
{

    public ChooseImportEventView(string title, IEnumerable<CalendarEvent> events)
    {
        InitializeComponent();
        selectItem.ItemsSource = events.ToList();
        titleLabel.Text = title;
        selectItem.SelectionMode = SelectionMode.Multiple;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        if (selectItem.SelectionMode == SelectionMode.Multiple)
        {
            var selected = new List<CalendarEvent>();

            foreach(var item in selectItem.SelectedItems)
            {
                if(item is CalendarEvent)
                    selected.Add((CalendarEvent)item);
            }

            this.CloseAsync(selected);
        }

    }
}
