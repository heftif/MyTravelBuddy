using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;

namespace MyTravelBuddy.ViewModels;

public partial class TourDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private int? tourId;

    public Tour Tour;

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

    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {
        Tour.VehicleToAndFromId = SelectedVehicleToAndFrom.VehicleId;
        Tour.VehicleAtLocationId = SelectedVehicleAtLocation.VehicleId;
        Tour.TourTypeId = SelectedTourType.TourTypeId;

        //but now we always update when leaving the screen, which might not be ideal
        //also, when we have more fields, we should consider a validate function before saving
        await App.DatabaseService.SaveItemAsync(Tour);

        await Shell.Current.GoToAsync("..", true);
    }

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
        }
    }

    async Task LoadAsync()
    {
        //load necessary objects
        var vehicles = await App.DatabaseService.ListAll<Vehicle>();

        foreach (var vehicle in vehicles)
            VehiclesToAndFrom.Add(vehicle);

        var vehicles2 = await App.DatabaseService.ListAll<Vehicle>();

        foreach (var vehicle in vehicles2)
            VehiclesAtLocation.Add(vehicle);

        var tourTypes = await App.DatabaseService.ListAll<TourType>();

        foreach (var tourType in tourTypes)
            TourTypes.Add(tourType);


        if (tourId.HasValue && tourId > 0)
        {
            //get object
            Tour = await App.DatabaseService.GetObject<Tour>(tourId.Value);

            //load the parameters for the view
            SelectedVehicleToAndFrom = VehiclesToAndFrom.Where(x => x.VehicleId == Tour.VehicleToAndFromId).FirstOrDefault();
            SelectedVehicleAtLocation= VehiclesAtLocation.Where(x => x.VehicleId == Tour.VehicleAtLocationId).FirstOrDefault();
            SelectedTourType = TourTypes.Where(x => x.TourTypeId == Tour.TourTypeId).FirstOrDefault();
        }
        else
        {
            //start in edit mode
            IsInEditMode = true;
        }

        IsLoaded = true;

    }

    partial void OnSelectedTourTypeChanged(TourType value)
    {
        if (IsLoaded)
        {
            if (value.IsHike())
                SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.WalkUsage).FirstOrDefault();
            else if (value.IsBike())
                SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.CycleUsage).FirstOrDefault();
            else if (value.IsRoadTrip())
                SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.CarUsage).FirstOrDefault();
            else if (value.IsCityTrip())
                SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.WalkUsage).FirstOrDefault();
            else if (value.IsCruise())
                SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.Usage == TourUsage.BoatUsage).FirstOrDefault();
        }
    }

    
}

