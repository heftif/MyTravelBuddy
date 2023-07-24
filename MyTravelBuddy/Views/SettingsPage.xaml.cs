using System.Timers;

namespace MyTravelBuddy.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        testNotificationButton.IsEnabled = false;
        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Interval = 5000; //ms
        aTimer.Enabled = true;
        aTimer.Elapsed += ATimer_Elapsed;
    }

    private void ATimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        Device.BeginInvokeOnMainThread(() => { testNotificationButton.IsEnabled = true; });
    }
}
