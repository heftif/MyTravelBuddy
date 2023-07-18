﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;

namespace MyTravelBuddy.ViewModels;

public partial class TourDetailsViewModel : DomainObjectViewModel, IQueryAttributable
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

    [ObservableProperty]
    Byte[] tourImage;


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

    }

    //probably causes the loading issues in android
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
            await LoadProperties();
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



    async Task LoadProperties()
    {
        //get object
        Tour = await App.DatabaseService.GetObject<Tour>(tourId.Value);

        GeneralLocation = Tour.GeneralLocation;
        Name = Tour.Name;
        StartsOn = Tour.StartsOn;
        EndsOn = Tour.StartsOn;

        Title = Tour.Name;
        TourImage = Tour.Image;

        //load the parameters for the view
        SelectedVehicleToAndFrom = VehiclesToAndFrom.Where(x => x.VehicleId == Tour.VehicleToAndFromId).FirstOrDefault();
        SelectedVehicleAtLocation = VehiclesAtLocation.Where(x => x.VehicleId == Tour.VehicleAtLocationId).FirstOrDefault();
        SelectedTourType = TourTypes.Where(x => x.TourTypeId == Tour.TourTypeId).FirstOrDefault();

        // so all errors etc are set correctly
        Validate();
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


    //triggered when pressing back button
    [RelayCommand]
    public async Task GoBackAsync()
    {
        MapProperties();

        //call save on baseviewmodel to ensure validation etc.
        await SaveDomainObject(Tour);

        WeakReferenceMessenger.Default.Send(new ReloadItemsMessage(true));

        await Shell.Current.GoToAsync("..", true);
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

