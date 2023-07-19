using System;
namespace MyTravelBuddy.ViewModels;

public partial class SettingsPageViewModel : BaseViewModel
{
	[ObservableProperty]
	bool pushNotificationsEnabled;

    [ObservableProperty]
    bool remindersEnabled;

    public SettingsPageViewModel()
	{
	}

    partial void OnPushNotificationsEnabledChanged(bool value)
    {
        
    }
}

