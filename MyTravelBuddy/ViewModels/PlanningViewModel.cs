using System;
using System.Collections.ObjectModel;

namespace MyTravelBuddy.ViewModels;

public partial class PlanningViewModel : TourDetailsCollectionBase, IQueryAttributable
{
    public ObservableCollection<PlanningItem> PlanningItems { get; } = new();

    public PlanningViewModel()
    {
        IsLoaded = false;

        App.ShellNavigationService.AddToShellStack(planning);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        SelectedMenuItem = planning;

        if (!IsLoaded)
        {
            Tour = query["Tour"] as Tour;

            tourId = Tour.TourId;

            //load necessary objects
            var planningItems = query["PlanningItems"] as List<PlanningItem>;

            //todo set the duedates for all these planning items!

            foreach (var item in planningItems)
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

    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {
        //always go back to main page! 
        await GoBackToMainPage();
    }
}

