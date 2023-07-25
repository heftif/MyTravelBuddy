using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Messaging;


namespace MyTravelBuddy.ViewModels;

public partial class TourDetailsCollectionBase : DomainObjectViewModel
{
    public Tour Tour;

    protected int? tourId;

    [ObservableProperty]
    string selectedMenuItem;

    public ObservableCollection<string> MenuPoints { get; } = new();


    public TourDetailsCollectionBase()
	{
        string[] menuItems = { "Overview", "Details", "Planning", "More" };


        foreach (var item in menuItems)
        {
            MenuPoints.Add(item);
        }
    }

    partial void OnSelectedMenuItemChanged(string value)
    {
        if (IsLoaded)
        {
            switch (value)
            {
                case "Overview":
                    NavigateToOverview().SafeFireAndForget();
                    break;
                case "Details":
                    NavigateToDetails().SafeFireAndForget();
                    break;
                case "Planning":
                    NavigateToPlanning().SafeFireAndForget();
                    break;
                case "More":
                    NavigateToMore().SafeFireAndForget();
                    break;
            }
        }
    }

    async Task NavigateToOverview()
    {
        if (IsBusy)
            return;

        if (Tour.TourId == 0)
            return;

        var vehiclesToAndFrom = await App.DatabaseService.ListAll<Vehicle>();
        var vehiclesAt = await App.DatabaseService.ListAll<Vehicle>();
        var tourTypes = await App.DatabaseService.ListAll<TourType>();

        //it is necessary to hand over the vehicles and tourtypes to ensure correct loading of details
        //since async loading otherwise causes delays and stutters. 
        await Shell.Current.GoToAsync(nameof(TourDetailsView), false, new Dictionary<string, object>
        {
            {"Tour", Tour},
            {"VehiclesToAndFrom", vehiclesToAndFrom},
            {"VehiclesAt", vehiclesAt},
            {"TourTypes", tourTypes}
        });
    }

    async Task NavigateToDetails()
    {

    }

    async Task NavigateToPlanning()
    {
        if (IsBusy)
            return;

        if (Tour.TourId == 0)
            return;


        //it is necessary to hand over the vehicles and tourtypes to ensure correct loading of details
        //since async loading otherwise causes delays and stutters. 
        await Shell.Current.GoToAsync(nameof(PlanningView), false, new Dictionary<string, object>
        {
            {"Tour", Tour},
        });

    }

    async Task NavigateToMore()
    {

    }


    public override bool Validate()
    {
        throw new NotImplementedException();
    }
}

