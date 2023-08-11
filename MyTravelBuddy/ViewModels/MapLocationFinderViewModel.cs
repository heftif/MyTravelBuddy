using System;
using System.ComponentModel.DataAnnotations;

namespace MyTravelBuddy.ViewModels;

public partial class MapLocationFinderViewModel : DomainObjectViewModel
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

    public MapLocationFinderViewModel()
    {
        Validate();
    }

    [RelayCommand]
    public async Task GetFromCalendarAsync()
    {

    }

    [RelayCommand]
    public async Task ValidateProperty()
    {
        if (Validate())
        {
            Location location = null;

            try
            {
                await GetLocationAsync();
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
                //open map with the given location coordinates
            }
        }
    }


    async Task<Location> GetLocationAsync()
    {
        //japanese addresses don't work just like this, but they are also a bit weird so there's that.
        //string address = "380-0824, Nagano, Minami Ishidoucho 1426 380-0824";

        string address = "Tokyo, Japan";
        IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

        return locations?.FirstOrDefault();

        //if (location != null)
            //await App.AlertService.ShowAlertAsync("Location", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
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

}

