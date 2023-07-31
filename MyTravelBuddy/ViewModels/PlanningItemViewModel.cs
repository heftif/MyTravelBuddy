using System;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MyTravelBuddy.ViewModels;

public partial class PlanningItemViewModel : DomainObjectViewModel
{
    private PlanningItem planningItem;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotDone))]
    bool isDone;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverDue))]
    DateTime dueDate;

    [ObservableProperty]
    int daysBeforeEvent;

    [ObservableProperty]
    bool notificationEnabled;

    [ObservableProperty]
    bool isAndroidOrIOs;

    public bool IsNotDone => !IsDone;

    public bool IsNotificationChanged;

    public int PlanningItemId;

    public bool IsOverDue
    {
        get
        {
            if (DueDate < DateTime.Now && !IsDone)
            {
                return true;
            }

            return false;
        }
    }

    public PlanningItemViewModel(PlanningItem planningItem)
	{
        this.planningItem = planningItem;
        Name = planningItem.Name;
        Description = planningItem.Description;
        IsDone = planningItem.IsDone;
        DaysBeforeEvent = planningItem.DaysBeforeEvent;
        NotificationEnabled = planningItem.NotificationEnabled;
        PlanningItemId = planningItem.PlanningItemId;

#if ANDROID || IOS
        IsAndroidOrIOs = true;
#else
        IsAndroidOrIOs = false;
#endif

    }

    partial void OnIsDoneChanged(bool value)
    {
        MapProperties();

        //send update for resorting list
        WeakReferenceMessenger.Default.Send(new ReloadPlanningItemsMessage(this));
        
        //call save on baseviewmodel to ensure validation etc.
        SaveDomainObject(planningItem).SafeFireAndForget();

        //remove notification of the item -> even when IsDone = false and it was previous enabled, such
        //that is has to be set again, so everything works correctly.
        NotificationEnabled = false;
    }

    partial void OnNotificationEnabledChanged(bool value)
    {
        //make sure we handle toggeling on and off in the same action
        if (IsNotificationChanged)
        {
            IsNotificationChanged = false;
        }
        else if (planningItem.NotificationEnabled != NotificationEnabled)
        {
            IsNotificationChanged = true;
        }

        MapProperties();

        //call save on baseviewmodel to ensure validation etc.
        SaveDomainObject(planningItem).SafeFireAndForget();
    }

    void MapProperties()
    {

        planningItem.Name = Name;
        planningItem.Description = Description;
        planningItem.IsDone = IsDone;
        planningItem.DaysBeforeEvent = DaysBeforeEvent;
        planningItem.NotificationEnabled = NotificationEnabled;
    }

    public override bool Validate()
    {
        return true;
    }

}

