using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Maps;
using MyTravelBuddy.Models.Messages;

namespace MyTravelBuddy.ViewModels;

public partial class MapLocationFinderViewModel : DomainObjectViewModel, IQueryAttributable
{
    [ObservableProperty]
    string street;

    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "City is Required")]
    [ObservableProperty]
    string city;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCityNotValid))]
    bool isCityValid;

    public bool IsCityNotValid => !IsCityValid;

    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Country is Required")]
    [ObservableProperty]
    string country;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCountryNotValid))]
    bool isCountryValid;

    public bool IsCountryNotValid => !IsCountryValid;

    [ObservableProperty]
    bool isReady;

    [ObservableProperty]
    ObservableCollection<Place> bindablePlaces;

    public bool CanSaveLocation => Places.Count > 0;

    WayPoint wayPoint;

    DayPlan dayPlan;

    string wayPointType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveLocation))]
    List<Place> places;

    public MapLocationFinderViewModel()
    {
        IsReady = false;

        Places = new List<Place>();

        Validate();
    }

    //todo: see if start or end location
    //todo: see if we edit or create a new waypoint
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!IsLoaded)
        {
            dayPlan = query["DayPlan"] as DayPlan;

            wayPointType = query["Type"] as string;

            Load();
        }
    }

    private void Load()
    {
        //todo load the properties from the previous way point if we're editing
           // LoadProperties();
        

    }


    [RelayCommand]
    public async Task ConfirmLocationAsync()
    {
        if (BindablePlaces != null && BindablePlaces.Count > 0)
        {
            await DisappearingAsync();
        }

        await Shell.Current.GoToAsync("..", true);

    }

    [RelayCommand]
    public async Task GetFromCalendarAsync()
    {

    }


    [RelayCommand]
    public async Task MapClickedAsync(MapClickedEventArgs args)
    {
        //replace the found location with the clickedLocation
        if(args?.Location != null)
        {
            IsReady = false;

            if(BindablePlaces != null && BindablePlaces.Count > 0)
                BindablePlaces.Clear();

            //show location on map
            var place = new Place
            {
                Location = args.Location,
                Address = $"{City}, {Country}", //todo: fix this address
                Description = "Test",
            };

            Places = new List<Place>() { place };
            BindablePlaces = new ObservableCollection<Place>(Places);
            IsReady = true;

        }    
    }



    [RelayCommand]
    public async Task ValidateProperty()
    {
        if (Validate())
        {
            Location location = null;

            try
            {
                location = await GetLocationAsync();
            }
            catch(Exception ex)
            {
                await App.AlertService.ShowAlertAsync("Error finding Location", $"Could not resolve place: {ex}");
            }

            if(location == null)
            {
                await App.AlertService.ShowAlertAsync("Not Found", "Could not find location");
                return;
            }
            else
            {
                //show location on map
                var place = new Place
                {
                    Location = location,
                    Address = $"{City}, {Country}",
                    Description = "Test",
                };

                Places = new List<Place>() { place };
                BindablePlaces = new ObservableCollection<Place>(Places);
                IsReady = true;

            }
        }
    }

    async Task<Location> GetLocationAsync()
    {
        //japanese addresses don't work just like this, but they are also a bit weird so there's that.
        //string address = "380-0824, Nagano, Minami Ishidoucho 1426 380-0824";

        string address = $"{City}, {Country}";
        IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

        return locations?.FirstOrDefault();
    }

    public override bool Validate()
    {
        ValidateAllProperties();

        if (HasErrors)
            Error = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
        else
            Error = String.Empty;

        IsCityValid = !(GetErrors().ToDictionary(k => k.MemberNames.First(), v => v.ErrorMessage) ?? new Dictionary<string, string?>()).TryGetValue(nameof(City), out var errorName);
        IsCountryValid = !(GetErrors().ToDictionary(k => k.MemberNames.First(), v => v.ErrorMessage) ?? new Dictionary<string, string?>()).TryGetValue(nameof(Country), out var errorDestination);

        return !HasErrors;
    }


    //todo only save when there were changes
    //triggered when navigating from this page
    [RelayCommand]
    public async Task DisappearingAsync()
    {
        if (CanSaveLocation && Places.Count > 0)
        {
            MapProperties();

            //call save on baseviewmodel to ensure validation etc.
            var success = await SaveDomainObject(wayPoint);

            if (success)
            {
                WeakReferenceMessenger.Default.Send(new ReloadWayPointsMessage(wayPoint));
            }
        }
    }

    private void MapProperties()
    {
        var place = Places.First();

        if (place != null)
        {
            if (wayPoint == null)
                wayPoint = new WayPoint();

            wayPoint = Mapper.Map(wayPoint, place, wayPointType, dayPlan.DayPlanId);

        }

    }

}

