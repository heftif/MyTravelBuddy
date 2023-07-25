using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;

namespace MyTravelBuddy.ViewModels;

public partial class TourDetailsViewModel : TourDetailsCollectionBase, IQueryAttributable
{

    [ObservableProperty]
    bool isInEditMode;

    [ObservableProperty]
    Vehicle selectedVehicleToAndFrom;

    [ObservableProperty]
    Vehicle selectedVehicleAtLocation;

    [ObservableProperty]
    TourType selectedTourType;

    [ObservableProperty]
    byte[] tourImage;


    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is Required")]
    [MaxLength(15, ErrorMessage ="Name is too long!")]
    [ObservableProperty]
    string name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNameNotValid))]
    bool isNameValid;

    public bool IsNameNotValid => !IsNameValid;


    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Destination is Required")]
    [MaxLength(80, ErrorMessage = "Destination is too long!")]
    [ObservableProperty]
    string generalLocation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDestinationNotValid))]
    bool isDestinationValid;

    public bool IsDestinationNotValid => !IsDestinationValid;

    [ObservableProperty]
    DateTime startsOn;

    [ObservableProperty]
    DateTime endsOn;

    public ObservableCollection<Vehicle> VehiclesToAndFrom { get; } = new();
    public ObservableCollection<Vehicle> VehiclesAtLocation { get; } = new();
    public ObservableCollection<TourType> TourTypes { get; } = new();
    
    ImageUploadService imageUploadService;


    public TourDetailsViewModel(ImageUploadService service)
    {
        IsLoaded = false;
        IsInEditMode = false;
        imageUploadService = service;


        SelectedMenuItem = "Overview";
        //due to issues with carouselview, the work around of using a message when view is loaded and making it async was necessary!
        WeakReferenceMessenger.Default.Register<SetSelectedItemMessage>(this, (r,m) => SetSelectedItems(r,m).SafeFireAndForget());
    }

    //probably causes the loading issues in android
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!IsLoaded)
        {
            Tour = query["Tour"] as Tour;

            tourId = Tour.TourId;

            //load necessary objects
            var vehiclesAt = query["VehiclesAt"] as List<Vehicle>;

            foreach (var vehicle in vehiclesAt)
                VehiclesAtLocation.Add(vehicle);

            var vehiclesToAndFrom = query["VehiclesToAndFrom"] as List<Vehicle>;

            foreach (var vehicle in vehiclesToAndFrom)
                VehiclesToAndFrom.Add(vehicle);


            var tourTypes = query["TourTypes"] as List<TourType>;

            foreach (var tourType in tourTypes)
                TourTypes.Add(tourType);

            //handle all async loading calls in the vm before and hand it over, so loading can
            //be sync to avoid stutters when loading an ensure proper binding
            Load();

        }
    }

    void Load()
    {

        if (tourId.HasValue && tourId > 0)
        {
            LoadProperties().SafeFireAndForget();
        }
        else
        {
            //start in edit mode
            IsInEditMode = true;
        }

        IsLoaded = true;

    }

    async Task LoadProperties()
    {
        //get object
        GeneralLocation = Tour.GeneralLocation;
        Name = Tour.Name;
        StartsOn = Tour.StartsOn;
        EndsOn = Tour.StartsOn;

        Title = Tour.Name;
        TourImage = Tour.Image;

        // so all errors etc are set correctly
        Validate();
    }

    async Task SetSelectedItems(object sender, SetSelectedItemMessage msg)
    {
        //necessary for workaround for issues with carouselview!
        //currently, scrolling to the given item does not yet work for android!
        //https://github.com/dotnet/maui/issues/7575
        await Task.Delay(500).ConfigureAwait(true);

        if (msg.Value == "TourType")
        {
            SelectedTourType = TourTypes.Where(x => x.TourTypeId == Tour.TourTypeId).FirstOrDefault();
        }
        else if (msg.Value == "VehicleToAndFrom")
        {
            SelectedVehicleToAndFrom = VehiclesToAndFrom.Where(x => x.VehicleId == Tour.VehicleToAndFromId).FirstOrDefault();
        }
        else if (msg.Value == "VehicleAt")
        {
            SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.VehicleId == Tour.VehicleAtLocationId).FirstOrDefault();
        }
    }


    [RelayCommand]
    public void ValidateProperty()
    {
        Validate();
    }

    public override bool Validate()
    {
        ValidateAllProperties();

        if (HasErrors)
            Error = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
        else
            Error = String.Empty;

        IsNameValid = !(GetErrors().ToDictionary(k => k.MemberNames.First(), v => v.ErrorMessage) ?? new Dictionary<string, string?>()).TryGetValue(nameof(Name), out var errorName);
        IsDestinationValid = !(GetErrors().ToDictionary(k => k.MemberNames.First(), v => v.ErrorMessage) ?? new Dictionary<string, string?>()).TryGetValue(nameof(GeneralLocation), out var errorDestination);

        return !HasErrors;
    }


    partial void OnSelectedTourTypeChanged(TourType value)
    {
        if (value == null)
            return;

        /*if (IsLoaded)
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
        }*/
    }


    [RelayCommand]
    public async Task UploadImageAsync()
    {
        if (imageUploadService != null)
        {
            var img = await imageUploadService.OpenMediaPickerAsync();
            var imagefile = await imageUploadService.Upload(img);

            //save to tour as byte array
            Tour.Image = imageUploadService.StringToByteBase64(imagefile.byteBase64);

            //display image in xaml
            TourImage = imageUploadService.StringToByteBase64(imagefile.byteBase64);
        }
    }

    [RelayCommand]
    public async Task GoToSettingsAsync()
    {
        //maybe save tour before leaving the site?
        //not really necessary, as long as I make sure you can leave the site only with also triggering the back
        //button action

        //should be made with a specific call to the database, instead of getting all push settings
        var allPushSettings = await App.DatabaseService.ListAll<PushSetting>();

        var currentPushSetting = allPushSettings.Where(x => x.TourId == tourId.Value).FirstOrDefault();


        await Shell.Current.GoToAsync(nameof(SettingsPage), true, new Dictionary<string, object>
        {
            {"Tour", Tour},
            {"PushSetting", currentPushSetting }
        });
    }


    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {
        MapProperties();

        //call save on baseviewmodel to ensure validation etc.
        var success = await SaveDomainObject(Tour);

        if (success)
        {
            WeakReferenceMessenger.Default.Send(new ReloadItemMessage(Tour));

            await Shell.Current.GoToAsync("..", true);
        }
    }

    void MapProperties()
    {
        Tour.VehicleToAndFromId = SelectedVehicleToAndFrom.VehicleId;
        Tour.VehicleAtLocationId = SelectedVehicleAtLocation.VehicleId;
        Tour.TourTypeId = SelectedTourType.TourTypeId;

        Tour.Name = Name;
        Tour.GeneralLocation = GeneralLocation;
        Tour.StartsOn = StartsOn;
        Tour.EndsOn = EndsOn;
        Tour.Image = TourImage;
    }


}

