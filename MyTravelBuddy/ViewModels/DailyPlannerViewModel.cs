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

    //todo set icon of trip on transportations, but this is an async function we can only perform after
    //loading so this needs some thinking on how we could do that. => check if we could not add the
    //objects as well as the ids in the sqlite database
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
                    DayPlans.Add(new DayPlanItemViewModel(DayPlan.Empty(i, tourId.Value, date), true));
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

    [RelayCommand]
    public async Task EditDetailsAsync()
    {
        //simplest of all of them -> enter a location and a name -> maybe we could gather this info direclty from
        //transportation and stay!
    }

    [RelayCommand]
    public async Task ShowTransportationAsync()
    {
        //add google maps view of a route (is that possible)
    }

    [RelayCommand]
    public async Task ShowOvernightAsync()
    {
        //add info about overnight stay -> could we take this from google or import it to google?
        //or at least get some google maps where the location can be searched and saved?


        await Shell.Current.GoToAsync(nameof(MapLocationFinderView), true, new Dictionary<string, object>
        {

        });

    }

    [RelayCommand]
    public async Task ShowDocumentsAsync()
    {
        //add documents for travel
    }

    [RelayCommand]
    public async Task DisappearingAsync()
    {
        if(DayPlans.Any(x => x.IsChanged))
        {
            foreach (var item in DayPlans.Where(x => x.IsChanged))
            {
                //todo add mapping for this to work correctly
                await SaveDomainObject(item.DayPlan);
            }
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

