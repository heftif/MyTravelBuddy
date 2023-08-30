using System;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MyTravelBuddy.ViewModels;

public partial class SettingsPageViewModel : DomainObjectViewModel, IQueryAttributable
{
	[ObservableProperty]
	bool pushNotificationsEnabled;

    [ObservableProperty]
    bool closeRemindersEnabled;

    [ObservableProperty]
    bool farRemindersEnabled;

    Tour tour;

    PushSetting currentPushSetting;

    private readonly Plugin.LocalNotification.INotificationService notificationService;

    public SettingsPageViewModel(Plugin.LocalNotification.INotificationService notificationService)
	{
        this.notificationService = notificationService;
	}

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        tour = query["Tour"] as Tour;

        if (tour == null)
        {
            await App.AlertService.ShowAlertAsync("Error", "No corresponding Tour found");
        }

        currentPushSetting = query["PushSetting"] as PushSetting;

        if(currentPushSetting != null)
        {
            if (currentPushSetting.CloseReminders || currentPushSetting.FarReminders)
                PushNotificationsEnabled = true;

            CloseRemindersEnabled = currentPushSetting.CloseReminders;
            FarRemindersEnabled = currentPushSetting.FarReminders;
        }

    }

    [RelayCommand]
    public async Task SendTestNotificationAsync()
    {
        var reminderDate = DateTime.Now.AddSeconds(10);
        var notificationId = 30000 + tour.TourId; //test id

        await ScheduleNotification($"Departure Reminder for {tour.Name}",
                "Check Departure List",
                "Check if you got everything ready before your trip",
                reminderDate,
                notificationId
                );
    }

    public override bool Validate()
    {
        return true;
    }

    void MapProperties()
    {
        currentPushSetting = Mapper.Map(currentPushSetting, this);
        
    }

    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {
        IsBusy = true;

        //save the settings for the reminder.
        if (SettingsChanged(currentPushSetting))
        {
            //make notification settings here
            await SetupNotifications();

            bool success;

            if (currentPushSetting == null)
            {
                var pushSetting = new PushSetting
                {
                    TourId = tour.TourId,
                    CloseReminders = CloseRemindersEnabled,
                    FarReminders = FarRemindersEnabled
                };

                //call save on baseviewmodel to ensure validation etc.
                success = await SaveDomainObject(pushSetting);
            }
            else
            {
                MapProperties();
                success = await SaveDomainObject(currentPushSetting);
            }

            IsBusy = false;

            if (success)
            {
                await Shell.Current.GoToAsync("..", true);
            }
            else
            {
                await App.AlertService.ShowAlertAsync("Error saving settings", "Could not save push settings!");
                await Shell.Current.GoToAsync("..", true);
            }
        }

        await Shell.Current.GoToAsync("..", true);

    }

    async Task SetupNotifications()
    {
        if (CloseRemindersEnabled)
            await SetupCloseNotifications();
        else
            await RemoveCloseNotifications(); //remove the notification if exists!


        if (FarRemindersEnabled)
            await SetupFarNotifications();
        else
            await RemoveFarNotifications(); //remove notification if exists!

    }

    async Task SetupCloseNotifications()
    {
        if (currentPushSetting == null || currentPushSetting.CloseReminders != CloseRemindersEnabled)
        {
            var reminderDate = tour.StartsOn.AddDays(-7);
            var notificationId = 10000 + tour.TourId;

            await ScheduleNotification($"Departure Reminder for {tour.Name}",
                "Check Departure List",
                "Check if you got everything ready before your trip",
                reminderDate,
                notificationId
                );
        }
    }

    async Task RemoveCloseNotifications()
    {
        if(currentPushSetting != null && currentPushSetting.CloseReminders != CloseRemindersEnabled) //if the settings exist
        {
            var notificationId = 10000 + tour.TourId;
            await RemoveNotification(notificationId, "Close Notification");
        }
    }

    async Task SetupFarNotifications()
    {
        if (currentPushSetting == null || currentPushSetting.FarReminders != FarRemindersEnabled)
        {
            var reminderDate = tour.StartsOn.AddDays(-90);
            var notificationId = 20000 + tour.TourId;

            await ScheduleNotification($"Planning Reminder for {tour.Name}",
                "Check Planning List",
                "Check what to plan so you can enjoy your travel worry free!",
                reminderDate,
                notificationId);
        }
    }

    async Task RemoveFarNotifications()
    {
        if (currentPushSetting != null && currentPushSetting.FarReminders != FarRemindersEnabled) //if the settings exist
        {
            var notificationId = 20000 + tour.TourId;
            await RemoveNotification(notificationId, "Far Notification");
        }

    }

    async Task ScheduleNotification(string title, string subtitle, string description, DateTime reminderDate, int notificationId)
    {
        //set notificationid to find the notification again later and cancel it if it's not needed anymore
        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Subtitle = subtitle,
            Description = description,
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = reminderDate,
                //NotifyTime = DateTime.Now.AddSeconds(10),
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

    }


    async Task RemoveNotification(int notificationId, string title)
    {
        //var pendingNotificationList = await notificationService.GetPendingNotificationList();
        var notificationCancelled = notificationService.Cancel(notificationId);

        if (notificationCancelled)
        {
            await App.AlertService.ShowAlertAsync($"Disabled push notification", $"Removed Notification for {title}");
        }
        else
        {
            await App.AlertService.ShowAlertAsync($"No notification found", $"Couldn't find notification {notificationId}");
        }

    }

    bool SettingsChanged(PushSetting pushSetting)
    {
        //save settings for the first time
        if (pushSetting == null)
            return true;

        if (pushSetting.CloseReminders != CloseRemindersEnabled || pushSetting.FarReminders != FarRemindersEnabled)
            return true;

        return false;
    }

}


