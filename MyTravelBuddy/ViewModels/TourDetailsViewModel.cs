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

    public ObservableCollection<Vehicle> Vehicles { get; } = new();
    public ObservableCollection<TourType> TourTypes { get; } = new();

    public TourDetailsViewModel(SqlDatabase database) : base(database)
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
            //get object
            Tour = await Database.GetObject<Tour>(x => x.GetId() == tourId.Value);
        }
        else
        {
            //start in edit mode
            IsInEditMode = true;
        }

        var vehicles = await Database.ListAll<Vehicle>();

        foreach (var vehicle in vehicles)
            Vehicles.Add(vehicle);

        var tourTypes = await Database.ListAll<TourType>();

        foreach (var tourType in tourTypes)
            TourTypes.Add(tourType);


    }
}

