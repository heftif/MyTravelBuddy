using System;
using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Messaging;


namespace MyTravelBuddy.ViewModels;

public partial class TourDetailsCollectionBase : DomainObjectViewModel
{
    protected const string overview = "Overview";
    protected const string details = "Details";
    protected const string planning = "Planning";
    protected const string more = "More";

    public Tour Tour;

    protected int? tourId;

    [ObservableProperty]
    string selectedMenuItem;

    public ObservableCollection<string> MenuPoints { get; } = new();




    public TourDetailsCollectionBase()
    {
        string[] menuItems = { overview, details, planning, more };


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
                case overview:
                    NavigateToOverview().SafeFireAndForget();
                    break;
                case details:
                    NavigateToDetails().SafeFireAndForget();
                    break;
                case planning:
                    NavigateToPlanning().SafeFireAndForget();
                    break;
                case more:
                    NavigateToMore().SafeFireAndForget();
                    break;
            }
        }
    }

    //virtual so we can override in details themselves, so we never navigate to ourself.
    protected virtual async Task NavigateToOverview()
    {
        if (IsBusy)
            return;

        if (Tour.TourId == 0)
            return;

        var idx = CheckIfExistsInShellStack(overview);

        if (idx < 0) //not found
        {
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
        else
        {
            string path = GetShellBackPath(idx);
            await Shell.Current.GoToAsync(path, false);
        }
    }

    protected virtual async Task NavigateToDetails()
    {
        if (IsBusy)
            return;

        if (Tour.TourId == 0)
            return;

        var idx = CheckIfExistsInShellStack(details);

        if (idx < 0) //not found
        {
            await Shell.Current.GoToAsync(nameof(DailyPlannerView), false, new Dictionary<string, object>
            {
                {"Tour", Tour},
            });
        }
        else
        {
            string path = GetShellBackPath(idx);
            await Shell.Current.GoToAsync(path, false);
        }
    }

    protected virtual async Task NavigateToPlanning()
    {
        if (IsBusy)
            return;

        if (Tour.TourId == 0)
            return;

        var idx = CheckIfExistsInShellStack(planning);

        if (idx < 0) //not found
        {
            var allPlanningItems = await App.DatabaseService.ListAll<PlanningItem>();
            var planningItems = allPlanningItems.Where(x => x.TourId == Tour.TourId).ToList();
            //it is necessary to hand over the vehicles and tourtypes to ensure correct loading of details
            //since async loading otherwise causes delays and stutters. 
            await Shell.Current.GoToAsync(nameof(PlanningView), false, new Dictionary<string, object>
            {
                {"Tour", Tour},
                {"PlanningItems", planningItems }
            });
        }
        else
        {
            string path = GetShellBackPath(idx);
            await Shell.Current.GoToAsync(path, false);
        }

    }

    protected virtual async Task NavigateToMore()
    {

    }

    public override bool Validate()
    {
        throw new NotImplementedException();
    }

    protected async Task GoBackToMainPage()
    {
        //index of main page is always 1 before starting the count in shellstack
        var path = GetShellBackPath(-1); 
        await Shell.Current.GoToAsync(path, true);
    }


    int CheckIfExistsInShellStack(string viewName)
    {
        return App.ShellNavigationService.CheckIfExistsInShellStack(viewName);
    }

    string GetShellBackPath(int idx)
    {
        var currentIdx = App.ShellNavigationService.GetCurrentIndex(idx);

        string path = "";
        int i = 0;

        while(i < currentIdx-idx)
        {
            path += "/..";
            i++;
        }

        return path[1..];
    }
}

