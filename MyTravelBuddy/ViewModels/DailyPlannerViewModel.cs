using System;
using System.Collections.ObjectModel;

namespace MyTravelBuddy.ViewModels;

public partial class DailyPlannerViewModel : TourDetailsCollectionViewModelBase, IQueryAttributable
{
    public ObservableCollection<DayPlanItemViewModel> DayPlans { get; } = new();

    private double tourDuration;

    //todo think about what happens when the dates of the travel are changed
    // we don't want to delete everything only when a date is changed. Maybe we can just grey them out
    // and make them inactive? maybe also work with negative ints?
    public DailyPlannerViewModel()
    {
        IsLoaded = false;

        App.ShellNavigationService.AddToShellStack(details);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        SelectedMenuItem = details;

        //load necessary objects
        if (!IsLoaded)
        {
            Tour = query["Tour"] as Tour;

            tourId = Tour.TourId;
            
            var planningItems = query["DayPlans"] as List<DayPlanItemViewModel>;

            if (planningItems.Any())
            {
                var orderedPlanningItems = planningItems.OrderBy(x => x.TourDay);

                foreach (var item in orderedPlanningItems)
                    DayPlans.Add(item);
            }
            else
            {
                tourDuration = (Tour.EndsOn - Tour.StartsOn).TotalDays + 1;

                for(int i = 1; i <= tourDuration; i++)
                {
                    var date = Tour.StartsOn.AddDays(i - 1);
                    DayPlans.Add(new DayPlanItemViewModel(DayPlan.Empty(i, tourId.Value, date)));
                }
            }
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

