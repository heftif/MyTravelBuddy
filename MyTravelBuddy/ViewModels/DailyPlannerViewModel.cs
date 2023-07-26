using System;

namespace MyTravelBuddy.ViewModels;

public partial class DailyPlannerViewModel : TourDetailsCollectionBase, IQueryAttributable
{
    public DailyPlannerViewModel()
    {
        IsLoaded = false;

        App.ShellNavigationService.AddToShellStack(details);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        SelectedMenuItem = details;

        if (!IsLoaded)
        {
            Tour = query["Tour"] as Tour;

            tourId = Tour.TourId;
        }

        IsLoaded = true;
    }

    protected override async Task NavigateToDetails()
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

