using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using CommunityToolkit.Mvvm.Messaging;
using MyTravelBuddy.Models.Messages;

namespace MyTravelBuddy.ViewModels;

public partial class WayPointDisplayViewModel : DomainObjectViewModel, IQueryAttributable
{
    DayPlan dayPlan;

    public ObservableCollection<WayPoint> WayPoints { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoStartPoint))]
    [NotifyPropertyChangedFor(nameof(StartAddress))]
    [NotifyPropertyChangedFor(nameof(IsAddressVisible))]
    bool hasStartPoint;

    public bool HasNoStartPoint => !HasStartPoint;

    public string StartAddress => HasStartPoint ? WayPoints.Where(x => x.IsStartPoint).Select(x => x.GetAddress()).FirstOrDefault() : "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoEndPoint))]
    [NotifyPropertyChangedFor(nameof(EndAddress))]
    [NotifyPropertyChangedFor(nameof(IsAddressVisible))]
    bool hasEndPoint;

    public bool HasNoEndPoint => !HasEndPoint;

    public string EndAddress => HasEndPoint ? WayPoints.Where(x => x.IsEndPoint).Select(x => x.GetAddress()).FirstOrDefault() : "";

    public bool IsAddressVisible => HasEndPoint || HasStartPoint;


    [ObservableProperty]
    bool isReady;

    [ObservableProperty]
    ObservableCollection<Place> bindablePlaces;

    private List<Place> places;

    public WayPointDisplayViewModel()
	{
        WeakReferenceMessenger.Default.Register<ReloadWayPointsMessage>(this, ReloadWayPoints);

        places = new List<Place>();

        HasEndPoint = false;
        HasStartPoint = false;

    }


    [RelayCommand]
    public async Task ChooseLocationAsync(string type)
    {
        //if there is a start point and we set an end point, we give the start point as a place, so map is in the right place
        WayPoint wayPoint = null;
        if(type == "end" && WayPoints.Any(x => x.IsStartPoint))
        {
            wayPoint = WayPoints.Where(x => x.IsStartPoint).FirstOrDefault();
        }

        await Shell.Current.GoToAsync(nameof(MapLocationFinderView), true, new Dictionary<string, object>
        {
            {"DayPlan", dayPlan },
            {"Type", type },
            {"Action", "create"},
            {"WayPoint", wayPoint }
        });

    }

    //todo: save endpoint from one day as startpoint from next day (but it can be overridden if they want)

    [RelayCommand]
    public async Task EditLocationAsync(string type)
    {
        WayPoint wayPoint = null;
        if(type == "start")
        {
            wayPoint = WayPoints.Where(x => x.IsStartPoint).FirstOrDefault();
        }
        else if(type == "end")
        {
            wayPoint = WayPoints.Where(x => x.IsEndPoint).FirstOrDefault();
        }

        await Shell.Current.GoToAsync(nameof(MapLocationFinderView), true, new Dictionary<string, object>
        {
            {"DayPlan", dayPlan },
            {"Type", type },
            {"Action", "edit"},
            {"WayPoint", wayPoint }
        });

    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!IsLoaded)
        {
            dayPlan = query["DayPlan"] as DayPlan;

            //load necessary objects
            var wayPoints = query["WayPoints"] as List<WayPoint>;

            LoadWayPoints(wayPoints);

            IsLoaded = true;
        }

        //var stack = Shell.Current.Navigation.NavigationStack.ToArray();
    }

    //todo: when loading with waypoints, make sure that the label is visible
    private void LoadWayPoints(List<WayPoint> wayPoints)
    {
        var sortedWayPoints = wayPoints.OrderBy(x => x.SortOrder);

        foreach (var wayPoint in wayPoints)
            WayPoints.Add(wayPoint);

        if (wayPoints.Any(x => x.IsStartPoint))
            HasStartPoint = true;

        if (wayPoints.Any(x => x.IsEndPoint))
            HasEndPoint = true;

        if (WayPoints.Count > 0)
        {
            foreach(var wp in WayPoints)
            {
                AddToPlaces(-1, wp);
            }

            MoveMap();
        }
    }

    public void ReloadWayPoints(object sender, ReloadWayPointsMessage msg)
    {
        var wayPoint = msg.Value;

        if (wayPoint != null)
        {
            if (wayPoint.IsStartPoint)
            {
                //to trigger address update at the end
                HasStartPoint = false;

                var oldStartPointIdx = WayPoints.IndexOf(WayPoints.Where(x => x.IsStartPoint).FirstOrDefault());

                SetWayPoint(oldStartPointIdx, wayPoint);

                HasStartPoint = true;

            }
            else if (wayPoint.IsEndPoint)
            {
                //to trigger address update at the end
                HasEndPoint = false;

                var oldEndPointIdx = WayPoints.IndexOf(WayPoints.Where(x => x.IsEndPoint).FirstOrDefault());

                SetWayPoint(oldEndPointIdx, wayPoint);

                HasEndPoint = true;

            }
        }
    }

    private void SetWayPoint(int oldIdx, WayPoint wayPoint)
    {
        if (oldIdx >= 0)
        {
            WayPoints.RemoveAt(oldIdx);
        }

        WayPoints.Add(wayPoint);

        AddToPlaces(oldIdx, wayPoint);
        MoveMap();
    }

    private void AddToPlaces(int oldIdx, WayPoint wayPoint)
    {
        IsReady = false;

        //replace the found location with the clickedLocation
        if (places != null && places.Count > 0 && oldIdx >= 0)
            places.RemoveAt(oldIdx);

        //show location on map
        var place = new Place
        {
            Location = new Location { Latitude = wayPoint.Latitude, Longitude = wayPoint.Longitude },
            Address = wayPoint.GetAddress(),
            Description = wayPoint.Name,
        };

        places.Add(place);
    }

    private void MoveMap()
    {
        BindablePlaces = new ObservableCollection<Place>(places);
        IsReady = true;
    }
   

    public override bool Validate()
    {
        return true;
    }
}


