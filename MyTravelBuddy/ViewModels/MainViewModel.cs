using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using CommunityToolkit.Maui.Views;

namespace MyTravelBuddy.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Tour> Tours { get; } = new();

    public MainViewModel()
	{
		LoadAsync().SafeFireAndForget();
    }

	async Task LoadAsync()
	{
		var tours = await App.DatabaseService.ListAll<Tour>();

		var toursPlanned = tours.Count > 0;

		if(toursPlanned)
		{
			await LoadTours(tours);
        }
	}

	async Task LoadTours(IList<Tour> tours)
	{
        foreach (var tour in tours)
            Tours.Add(tour);
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


        await Shell.Current.GoToAsync(nameof(TourDetailsView), true, new Dictionary<string, object>
        {
            {"TourId", tour.TourId}
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

        //need to reload the screen to show the images, but maybe the interface will do that for us
        //with a observable object
        tour.TourId = tourId;

        Tours.Add(tour);
    }
}

