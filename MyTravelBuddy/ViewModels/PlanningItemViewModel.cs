using System;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Messaging;

namespace MyTravelBuddy.ViewModels;

public partial class PlanningItemViewModel : DomainObjectViewModel
{
    private PlanningItem planningItem;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    bool isDone;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverDue))]
    DateTime dueDate;

    [ObservableProperty]
    int daysBeforeEvent;

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

	}

    partial void OnIsDoneChanged(bool value)
    {
        MapProperties();

        //send update for resorting list
        WeakReferenceMessenger.Default.Send(new ReloadPlanningItemsMessage(this));
        
        //call save on baseviewmodel to ensure validation etc.
        SaveDomainObject(planningItem).SafeFireAndForget();
    }

    void MapProperties()
    {
        planningItem.Name = Name;
        planningItem.Description = Description;
        planningItem.IsDone = IsDone;
        planningItem.DaysBeforeEvent = DaysBeforeEvent;
    }

    public override bool Validate()
    {
        return true;
    }
}

