using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;

namespace MyTravelBuddy.ViewModels;

public partial class TourDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private int? tourId;

    [ObservableProperty]
    Tour tour;

    [ObservableProperty]
    bool isInEditMode;

    [ObservableProperty]
    Vehicle selectedVehicleToAndFrom;

    [ObservableProperty]
    Vehicle selectedVehicleAtLocation;

    [ObservableProperty]
    TourType selectedTourType;

    public ObservableCollection<Vehicle> VehiclesToAndFrom { get; } = new();
    public ObservableCollection<Vehicle> VehiclesAtLocation { get; } = new();

    public ObservableCollection<TourType> TourTypes { get; } = new();

    public TourDetailsViewModel()
	{
        IsLoaded = false;
        IsInEditMode = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        tourId = query["TourId"] as int?;

        if (!IsLoaded)
        {
            LoadAsync().SafeFireAndForget();
            IsLoaded = true;
        }
    }

    async Task LoadAsync()
    {
        if(tourId.HasValue && tourId > 0)
        {
            //figure out the issue here!
            //get object
            Tour = await App.DatabaseService.GetObject<Tour>(tourId.Value);
        }
        else
        {
            //start in edit mode
            IsInEditMode = true;
        }

        var vehicles = await App.DatabaseService.ListAll<Vehicle>();

        foreach (var vehicle in vehicles)
            VehiclesToAndFrom.Add(vehicle);

        var vehicles2 = await App.DatabaseService.ListAll<Vehicle>();

        foreach (var vehicle in vehicles2)
            VehiclesAtLocation.Add(vehicle);

        var tourTypes = await App.DatabaseService.ListAll<TourType>();

        foreach (var tourType in tourTypes)
            TourTypes.Add(tourType);

    }

    partial void OnSelectedTourTypeChanged(TourType value)
    {
        if (value.IsHike())
            SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.WalkUsage).FirstOrDefault();
        else if(value.IsBike())
            SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.CycleUsage).FirstOrDefault();
        else if(value.IsRoadTrip())
            SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.CarUsage).FirstOrDefault();
        else if (value.IsCityTrip())
            SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.WalkUsage).FirstOrDefault();
        else if (value.IsCruise())
            SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.BoatUsage).FirstOrDefault();
    }
}

