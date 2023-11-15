using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using Plugin.Maui.CalendarStore;

namespace MyTravelBuddy.ViewModels;

public partial class DailyPlannerViewModel : TourDetailsCollectionViewModelBase, IQueryAttributable
{
    public ObservableCollection<DayPlanItemViewModel> DayPlans { get; } = new();

    private double tourDuration;

    ICalendarStore calendarService;

    //todo think about what happens when the dates of the travel are changed
    // we don't want to delete everything only when a date is changed. Maybe we can just grey them out
    // and make them inactive? maybe also work with negative ints?
    public DailyPlannerViewModel(ICalendarStore calendarService)
    {
        IsLoaded = false;

        this.calendarService = calendarService;

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
        //add google maps view of a route (is that possible?)

        //get and save a calendar for reference
        var calendarSettings = await App.DatabaseService.ListAll<CalendarSetting>();

        if (!calendarSettings.Any())
        {
            var calendars = await calendarService.GetCalendars();

            //show a list with all the calendars and ask user to chose from which to import
            var cal = (Calendar)await Shell.Current.ShowPopupAsync(new ChooseCalendarView("From which calendar do you want to import events?", calendars));

            //save calendarsetting
            if (cal != null)
            {
                var c = new CalendarSetting
                {
                    Id = cal.Id,
                    Name = cal.Name,
                    
                };

                await SaveDomainObject(c);
            }
        }


        var calendar = (await App.DatabaseService.ListAll<CalendarSetting>()).ToList().FirstOrDefault();

        //show list with events that the user wants to import, corresponding to the travel dates.
        var events = await calendarService.GetEvents(calendar.Id, Tour.StartsOn, Tour.EndsOn);


        var importEvents = (List<CalendarEvent>)await Shell.Current.ShowPopupAsync(new ChooseImportEventView("Which events do you want to import?", events));


        //todo: for each event, we could try to create a dayplan based on the value that we were given. => logic might not be too easy

    }

    [RelayCommand]
    public async Task ShowOvernightAsync(DayPlanItemViewModel dayPlanViewModel)
    {

        var dayPlan = dayPlanViewModel.DayPlan;
        var allWayPoints = await App.DatabaseService.ListAll<WayPoint>();
        var wayPoints = allWayPoints.Where(x => x.DayPlanId == dayPlan.DayPlanId).ToList();

        await Shell.Current.GoToAsync(nameof(WayPointDisplayView), true, new Dictionary<string, object>
        {
            { "DayPlan", dayPlan },
            { "WayPoints", wayPoints}
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

