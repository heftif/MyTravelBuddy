using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace MyTravelBuddy.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Tour> Tours { get; } = new();


    //different notification services for desktop and mobile version
#if ANDROID || IOS
    Plugin.LocalNotification.INotificationService notificationService;
#else
    Services.INotificationService notificationService;
#endif

#if ANDROID || IOS
    public MainViewModel(Plugin.LocalNotification.INotificationService notificationService)
    {
        this.notificationService = notificationService;
        this.notificationService.NotificationActionTapped += NotificationService_NotificationActionTapped;

        Load();
    }
#else
    public MainViewModel(Services.INotificationService notificationService)
    {
        this.notificationService = notificationService;
        Load();
    }

#endif

    void Load()
    {
        //refreshing the view when we come back from details view
        WeakReferenceMessenger.Default.Register<ReloadItemMessage>(this, ReloadItem);

        //without package, fire and forget is a call for async void, which fires a task,
        //but does not wait for its result to return and proceeds with the code. Notorious for error handling
        //using the package should help with this and make it safer.
        //https://johnthiriet.com/removing-async-void/
        LoadAsync().SafeFireAndForget(onException: ex => App.AlertService.ShowAlertAsync("Error Loading Main", ex.ToString()));
    }

    async Task LoadAsync()
	{
        // first call to database also initialises the database
		var tours = await App.DatabaseService.ListAll<Tour>();

		var toursPlanned = tours.Count > 0;

		if(toursPlanned)
		{
			LoadTours(tours);
        }
        
	}


	void LoadTours(IList<Tour> tours)
	{
        foreach (var tour in tours)
            Tours.Add(tour);
	}

    //call must be sync, else it does not work properly for android
    public void ReloadItem(object sender, ReloadItemMessage msg)
    {
        Tours.Remove(msg.Value);

        Tours.Insert(0, msg.Value);
    }


    [RelayCommand]
	public async Task CreateNewTourAsync()
	{
		if (IsBusy)
            return;


		await ShowPopUps();
    }

    [RelayCommand]
    public async Task OpenTourAsync(Tour tour)
    {
        if (IsBusy)
            return;

        if (tour.TourId == 0)
            return;

        var vehiclesToAndFrom = await App.DatabaseService.ListAll<Vehicle>();
        var vehiclesAt = await App.DatabaseService.ListAll<Vehicle>();
        var tourTypes = await App.DatabaseService.ListAll<TourType>();

        //it is necessary to hand over the vehicles and tourtypes to ensure correct loading of details
        //since async loading otherwise causes delays and stutters. 
        await Shell.Current.GoToAsync(nameof(TourDetailsView), true, new Dictionary<string, object>
        {
            {"Tour", tour},
            {"VehiclesToAndFrom", vehiclesToAndFrom},
            {"VehiclesAt", vehiclesAt},
            {"TourTypes", tourTypes}
        });

    }

    //show various pop ups to get the wanted information
    async Task ShowPopUps()
	{

        //name of the trip
        var name = await App.AlertService.ShowPrompt("Name", "What is your trip called?");

        //name of the trip
        var destination = await App.AlertService.ShowPrompt("Destination", "Where are you going?");

        //at the moment, we can only have one pop up, as we can't close async. This will be fixed with new version

        //type of trip
        var tourTypes = await App.DatabaseService.ListAll<TourType>();
        //var tourType = (TourType)await Shell.Current.ShowPopupAsync(new NewTourPopUpView("What kind of trip do you want to take?", tourTypes));
        var tourType = tourTypes.Where(x => x.Usage == TourUsage.CycleUsage).FirstOrDefault();//setting default value for now

        //duration of trip
        var dates = (DateTime[])await Shell.Current.ShowPopupAsync(new TourDatePickerPopUpView());
        if(dates == null)
        {
            dates = new[] { DateTime.Now, DateTime.Now };
        }

        //vehicle to and from
        var vehicles = await App.DatabaseService.ListAll<Vehicle>();
        //var vehicleToAndFrom = (Vehicle)await Shell.Current.ShowPopupAsync(new NewTourPopUpView("What it you mode of transport to and from the destination?", vehicles));
        var vehicleToAndFrom = vehicles.Where(x => x.Usage == TourUsage.PlaneUsage).FirstOrDefault();

        //vehicle at
        //var vehiclesAt = (Vehicle)await Shell.Current.ShowPopupAsync(new NewTourPopUpView("What it you mode of transport at the destination?", vehicles));
        var vehicleAt = vehicles.Where(x => x.Usage == TourUsage.BikeUsage).FirstOrDefault();

        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(destination))
        {
            App.AlertService.ShowAlert("Error", "Either no name or no destination entered, Tour could not be planned.");
        }
        else
        {
            var tour = new Tour
            {
                Name = !string.IsNullOrEmpty(name) ? name : "",
                GeneralLocation = !string.IsNullOrEmpty(destination) ? destination : "",
                StartsOn = dates[0],
                EndsOn = dates[1],
                TourTypeId = tourType.TourTypeId,
                VehicleToAndFromId = vehicleToAndFrom.VehicleId,
                VehicleAtLocationId = vehicleAt.VehicleId,
            };

            var tourId = await App.DatabaseService.SaveItemAsync(tour);

            //add the planning items for the newly created tour
            await App.DatabaseService.AddPlanningItems(tourId);

            tour.TourId = tourId;

            Tours.Add(tour);
        }
 
    }

    #region Notifications

    //fires as soon as the notification is tapped by the user
    private void NotificationService_NotificationActionTapped(NotificationEventArgs e)
    {
        NagivateToPlanningView(e.Request.NotificationId).SafeFireAndForget();
    }

    async Task NagivateToPlanningView(int notificationId)
    {
        IsBusy = true;

        //get tour from NotificationId
        var tourIdString = notificationId.ToString();
        int? tourId = Int32.Parse(tourIdString.Substring(1, tourIdString.Length - 1));

        if (tourId.HasValue)
        {
            //go to the planningviewmodel
            var tour = await App.DatabaseService.GetObject<Tour>(tourId.Value);

            var allPlanningItems = await App.DatabaseService.ListAll<PlanningItem>();
            var planningItems = allPlanningItems.Where(x => x.TourId == tour.TourId).ToList();

            await Shell.Current.GoToAsync(nameof(PlanningView), true, new Dictionary<string, object>
            {
                {"Tour", tour},
                {"PlanningItems", planningItems }
            });
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(MainPage), true, new Dictionary<string, object>
            {

            });
        }

        IsBusy = false;
    }

    #endregion
}

