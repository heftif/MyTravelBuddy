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

    public bool CanSaveLocation => Place != null && LocationHasChanged(); 

    WayPoint wayPoint;

    DayPlan dayPlan;

    string wayPointType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveLocation))]
    Place place;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSaveLocation))]
    List<Place> places;

    public Address Address;

    public MapLocationFinderViewModel()
    {
        IsReady = false;

        Places = new List<Place>();

        Validate();
    }


    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!IsLoaded)
        {
            dayPlan = query["DayPlan"] as DayPlan;

            wayPointType = query["Type"] as string;

            wayPoint = query["WayPoint"] as WayPoint;

            var action = query["Action"] as string;
            if (action == "edit")
            {
                Load();
            }
            else if(action == "create" && wayPoint != null)
            {
                //place map on the starting waypoint
                CreatePlaceAndMoveMap(new Location(wayPoint.Latitude, wayPoint.Longitude));
            }

            
        }
    }

    private void Load()
    {
        Street = wayPoint.Street;
        City = wayPoint.City;
        Country = wayPoint.Country;

        CreatePlaceAndMoveMap(new Location(wayPoint.Latitude, wayPoint.Longitude));
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

    private void CreatePlaceAndMoveMap(Location location)
    {
        Places.Clear();

        //show location on map
        Place = new Place
        {
            Location = location,
            Address = GetPlaceAddress(), 
            Description = "Test",
        };

        Places.Add(Place);
        BindablePlaces = new ObservableCollection<Place>(Places);

        if (Address != null)
        {
            MapAddressProperties();
        }

        IsReady = true;
    }

    //todo: get location from click on map
    [RelayCommand]
    public async Task MapClickedAsync(MapClickedEventArgs args)
    {
        //replace the found location with the clickedLocation
        if(args?.Location != null)
        {
            IsReady = false;

            if(BindablePlaces != null && BindablePlaces.Count > 0)
                BindablePlaces.Clear();

            await SetAddressAsync(args.Location);
            CreatePlaceAndMoveMap(args.Location); 

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
                await SetAddressAsync(location);
                CreatePlaceAndMoveMap(location);
            }
        }
    }

    async Task SetAddressAsync(Location location)
    {
        if (location == null)
            return;

        var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);

        if (placemarks.Count() > 0)
        {
            var placemark = placemarks.FirstOrDefault();

            Address = new Address(placemark.FeatureName, placemark.Locality, placemark.CountryName, placemark.CountryCode, placemark.AdminArea, placemark.PostalCode);
        }
        else
        {
            Address = new Address(Street, City, Country, "", "", "");
        }
    }

    string GetPlaceAddress()
    {
        if(Address != null)
        {
            return Address.GetAddressString();
        }
        else
        {
            return string.Join(",", new string[] { Street, City, Country });
        }
    }

    void MapAddressProperties()
    {
        Street = Address.Street;
        City = Address.City;
        Country = Address.Country;
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

    private bool LocationHasChanged()
    {
        if(Place != null && wayPoint != null)
        {
            return Place.Location.Longitude != wayPoint.Longitude && Place.Location.Latitude != wayPoint.Latitude;
        }

        return true;
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

            //if this was an endpoint and there is no startpoint for the next day, then save this point as new startpoint for next day
            await SaveNextDayStartPoint();

            if (success)
            {
                WeakReferenceMessenger.Default.Send(new ReloadWayPointsMessage(wayPoint));
            }
        }
    }

    private void MapProperties()
    {

        if (Place != null)
        {
            if (wayPoint == null)
                wayPoint = new WayPoint();

            wayPoint = Mapper.Map(wayPoint, Place, Address, wayPointType, dayPlan.DayPlanId);

        }
    }

    //todo: think about if it would make sense to overwrite it, if we already have one there...
    private async Task SaveNextDayStartPoint()
    {
        if(wayPointType == "end")
        {
            var nextDayDate = dayPlan.Date + TimeSpan.FromDays(1);

            //get dayplan from next day, if there are days left in the trip
            var allDayPlans = await App.DatabaseService.ListAll<DayPlan>();
            var nextDayPlan = allDayPlans.Where(x => x.TourId == dayPlan.TourId && x.Date == nextDayDate).FirstOrDefault();

            if(nextDayPlan != null)
            {
                //check if there is a start waypoint for the next day
                var allWayPoints = await App.DatabaseService.ListAll<WayPoint>();
                var nextDayStartWayPoint = allWayPoints.Where(x => x.DayPlanId == nextDayPlan.DayPlanId && x.IsStartPoint).FirstOrDefault();

                //don't overwrite it 
                if (nextDayStartWayPoint != null)
                {
                    return;
                }
                else
                {
                    var nextDayWaypoint = new WayPoint();

                    nextDayWaypoint = Mapper.Map(nextDayWaypoint, Place, Address, "start", nextDayPlan.DayPlanId);

                    await SaveDomainObject(nextDayWaypoint);
                }
            }  
            

        }
    }

}

