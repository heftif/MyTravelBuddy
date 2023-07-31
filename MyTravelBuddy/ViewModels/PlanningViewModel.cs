using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;

namespace MyTravelBuddy.ViewModels;

public partial class PlanningViewModel : TourDetailsCollectionBase, IQueryAttributable
{
    public ObservableCollection<PlanningItemViewModel> PlanningItems { get; } = new();

    public PlanningViewModel()
    {
        IsLoaded = false;

        App.ShellNavigationService.AddToShellStack(planning);

        WeakReferenceMessenger.Default.Register<ReloadPlanningItemsMessage>(this, OnReloadPlanningItemsReceived);
    }

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

    //todo -> sort done tasks to the bottom
    //todo -> add option to add custom tasks

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

    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {
        //always go back to main page! 
        await GoBackToMainPage();
    }

}

