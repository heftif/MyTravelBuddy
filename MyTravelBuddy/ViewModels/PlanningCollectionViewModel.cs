using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualBasic;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MyTravelBuddy.ViewModels;

public partial class PlanningCollectionViewModel : TourDetailsCollectionViewModelBase, IQueryAttributable
{
    public ObservableCollection<PlanningItemViewModel> PlanningItems { get; } = new();

    [ObservableProperty]
    bool allNotificationsEnabled;


    //different notification services for desktop and mobile version
#if ANDROID || IOS
    Plugin.LocalNotification.INotificationService notificationService;
#else
    Services.INotificationService notificationService;
#endif

#if ANDROID || IOS
    public PlanningCollectionViewModel(Plugin.LocalNotification.INotificationService notificationService)
    {
        IsLoaded = false;
        this.notificationService = notificationService;

        App.ShellNavigationService.AddToShellStack(planning);

        WeakReferenceMessenger.Default.Register<ReloadPlanningItemsMessage>(this, OnReloadPlanningItemsReceived);
    }
#else
    public PlanningCollectionViewModel(Services.NotificationService notificationService)
    {
        this.notificationService = notificationService;
        IsLoaded = false;
        App.ShellNavigationService.AddToShellStack(planning);
        WeakReferenceMessenger.Default.Register<ReloadPlanningItemsMessage>(this, OnReloadPlanningItemsReceived);
    }
#endif

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        SelectedMenuItem = planning;

        if (!IsLoaded)
        {
            Tour = query["Tour"] as Tour;

            tourId = Tour.TourId;

            //load necessary objects
            var planningItems = query["PlanningItems"] as List<PlanningItemViewModel>;


            foreach (var item in planningItems)
            {
                item.DueDate = Tour.StartsOn.AddDays(-item.DaysBeforeEvent);
            }

            var orderedPlanningItems = planningItems.OrderBy(x => x.IsDone).OrderByDescending(x => x.DaysBeforeEvent);

            foreach (var item in orderedPlanningItems)
                PlanningItems.Add(item);
        }

        IsLoaded = true;
    }

    protected override async Task NavigateToPlanning()
    {
        return;
    }

    public override bool Validate()
    {
        return true;
    }

    //call must be sync, else it does not work properly for android
    public void OnReloadPlanningItemsReceived(object sender, ReloadPlanningItemsMessage msg)
    {
        PlanningItems.Remove(msg.Value);

        if(msg.Value.IsDone)
        {
            var amount = PlanningItems.Count();

            PlanningItems.Insert(amount, msg.Value);
        }
        else
        {
            PlanningItems.Insert(0, msg.Value);
        }
        
    }

    partial void OnAllNotificationsEnabledChanged(bool value)
    {
        foreach(var item in PlanningItems)
        {
            if(!item.IsDone) //schedule notifications only for not done items
                item.NotificationEnabled = value;
        }
    }

    [RelayCommand]
    public async Task AddCustomPlanningItemAsync()
    {
        //todo implement custom item adding screen 
    }

    //triggered when navigating from that page
    [RelayCommand]
    public async Task DisappearingAsync()
    {

#if ANDROID || IOS
        if (PlanningItems.Any(x => x.IsNotificationChanged))
        {
            foreach (var item in PlanningItems.Where(x => x.IsNotificationChanged))
            {
                await SetupNotifications(item);
            }

    #if DEBUG
            //for checks
            var pendingNotificationList = await notificationService.GetPendingNotificationList();
            await App.AlertService.ShowAlertAsync($"Amount of Notifications Schedules", $"{pendingNotificationList.Count}");
    #endif

        }
#endif
    }


    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {

        //always go back to main page! 
        await GoBackToMainPage();
    }


    #region Notifications

    async Task SetupNotifications(PlanningItemViewModel item)
    {
        if (item.NotificationEnabled)
            await SetupPlanningNotification(item);
        else
            await RemovePlanningNotification(item); //remove the notification if exists!

    }

    async Task SetupPlanningNotification(PlanningItemViewModel item)
    {
            var reminderDate = item.DueDate;
            var notificationId = 10000 + item.PlanningItemId;

            await ScheduleNotification($"Departure Reminder for {item.Name}",
                "Check Departure List",
                item.Description,
                reminderDate,
                notificationId
                );
    }

    async Task RemovePlanningNotification(PlanningItemViewModel item)
    {
        var notificationId = 10000 + item.PlanningItemId;
        await RemoveNotification(notificationId, item.Title);
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

#if ANDROID || IOS
            var ff = await notificationService.Show(request);
#endif

        }
        catch (Exception exception)
        {
            await App.AlertService.ShowAlertAsync("Error in Permissions", exception.ToString());
        }

    }

    async Task RemoveNotification(int notificationId, string title)
    {
        
        var notificationCancelled = notificationService.Cancel(notificationId);

        if (notificationCancelled)
        {
            //await App.AlertService.ShowAlertAsync($"Disabled push notification", $"Removed Notification for {title}");
        }
        else
        {
            await App.AlertService.ShowAlertAsync($"No notification found", $"Couldn't find notification {notificationId}");
        }

    }

    #endregion


}

