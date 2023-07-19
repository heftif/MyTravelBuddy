using System;
using System.Security.Principal;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MyTravelBuddy.ViewModels;

public partial class SettingsPageViewModel : BaseViewModel
{
	[ObservableProperty]
	bool pushNotificationsEnabled;

    [ObservableProperty]
    bool remindersEnabled;

    private readonly INotificationService notificationService;

    public SettingsPageViewModel(INotificationService notificationService)
	{
        this.notificationService = notificationService;
	}

    partial void OnPushNotificationsEnabledChanged(bool value)
    {
        
    }

    partial void OnRemindersEnabledChanged(bool value)
    {
        //should add busy indicator!
        //ShowNumberOfNotifications();
        ScheduleNotification();
        //ShowNumberOfNotifications();
    }

    private async Task ShowNumberOfNotifications()
    {
        //needed because calling async like this I freeze the UI
        IsBusy = true;

        var deliveredNotificationList = await notificationService.GetDeliveredNotificationList();

        if (deliveredNotificationList != null)
        {
            await App.AlertService.ShowAlertAsync("Delivered Notification Count", deliveredNotificationList.Count.ToString());
        }

        IsBusy = false;

    }

    private async Task ScheduleNotification()
    {
        IsBusy = true;

        //set notificationid to find the notification again later and cancel it if it's not needed anymore
        var request = new NotificationRequest
        {
            NotificationId = 123,
            Title = "Travel Reminder",
            Subtitle = "Pending Task is due",
            Description = "Look into you pending Tasks",
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(10),
                //NotifyRepeatInterval = TimeSpan.FromMinutes(1),
                //NotifyAutoCancelTime = DateTime.Now.AddMinutes(3),
                //RepeatType = NotificationRepeat.Daily
            },
            Android =
                {
                    IconSmallName =
                    {
                        ResourceName = "i2",
                    },
                    Color =
                    {
                        ResourceName = "colorPrimary"
                    },
                    Priority = AndroidPriority.High
                    //AutoCancel = false,
                    //Ongoing = true
                }
        };

        try
        {
            if (await notificationService.AreNotificationsEnabled() == false)
            {
                await notificationService.RequestNotificationPermission();
            }

            var ff = await notificationService.Show(request);
        }
        catch (Exception exception)
        {
            await App.AlertService.ShowAlertAsync("Error in Permissions", exception.ToString());
        }

        //there are more options for android using Android = new AndroidOptions (similar for iOs)

        //LocalNotificationCenter.Current.Show(request);

        IsBusy = false;
    }
}


