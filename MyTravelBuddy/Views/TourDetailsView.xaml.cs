﻿using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.Messaging;

namespace MyTravelBuddy.Views;

public partial class TourDetailsView : ContentPage
{
	public TourDetailsView(TourOverviewCollectionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

        datePickerStart.MaximumDate = DateTime.Now.AddYears(10);
        datePickerEnd.MaximumDate = DateTime.Now.AddYears(10);


    }

    void datePickerStart_DateSelected(System.Object sender, Microsoft.Maui.Controls.DateChangedEventArgs e)
    {
        if (datePickerEnd.Date <= datePickerStart.Date)
            datePickerEnd.Date = datePickerStart.Date.AddDays(1);
    }

    void datePickerEnd_DateSelected(System.Object sender, Microsoft.Maui.Controls.DateChangedEventArgs e)
    {
        if (datePickerEnd.Date <= datePickerStart.Date)
            datePickerEnd.Date = datePickerStart.Date.AddDays(1);
    }

    void TourTypeCarouselView_Loaded(System.Object sender, System.EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new SetSelectedItemMessage("TourType"));
    }

    void VehicleToAndFromCarouselView_Loaded(System.Object sender, System.EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new SetSelectedItemMessage("VehicleToAndFrom"));
    }

    void VehicleAtCarouselView_Loaded(System.Object sender, System.EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new SetSelectedItemMessage("VehicleAt"));
    }

}
