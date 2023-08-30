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
    bool hasStartPoint;

    public bool HasNoStartPoint => !HasStartPoint;

    public string StartAddress => HasStartPoint ? WayPoints.Where(x => x.IsStartPoint).Select(x => x.Address).FirstOrDefault() : "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoEndPoint))]
    [NotifyPropertyChangedFor(nameof(EndAddress))]
    bool hasEndPoint;

    public bool HasNoEndPoint => !HasEndPoint;

    public string EndAddress => HasEndPoint ? WayPoints.Where(x => x.IsEndPoint).Select(x => x.Address).FirstOrDefault() : "";


    [ObservableProperty]
    bool isReady;

    [ObservableProperty]
    ObservableCollection<Place> bindablePlaces;

    private List<Place> places;

    public WayPointDisplayViewModel()
	{
        WeakReferenceMessenger.Default.Register<ReloadWayPointsMessage>(this, ReloadWayPoints);

        places = new List<Place>();
    }


    [RelayCommand]
    public async Task ChooseLocationAsync(string type)
    {
        await Shell.Current.GoToAsync(nameof(MapLocationFinderView), true, new Dictionary<string, object>
        {
            {"DayPlan", dayPlan },
            {"Type", type }
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

    private void LoadWayPoints(List<WayPoint> wayPoints)
    {
        var sortedWayPoints = wayPoints.OrderBy(x => x.SortOrder);

        if (wayPoints.Any(x => x.IsStartPoint))
            HasStartPoint = true;

        if (wayPoints.Any(x => x.IsEndPoint))
            HasEndPoint = true;


        foreach (var wayPoint in wayPoints)
            WayPoints.Add(wayPoint);

        if (WayPoints.Count > 0)
        {
            var wp = WayPoints.First();
            //move map to one of the points
            //todo: move map to such a zoom factor that you can see both points
            MoveMap(-1, wp);
        }
    }

    public void ReloadWayPoints(object sender, ReloadWayPointsMessage msg)
    {
        var wayPoint = msg.Value;

        if (wayPoint != null)
        {
            if (wayPoint.IsStartPoint)
            {
                var oldStartPointIdx = WayPoints.IndexOf(WayPoints.Where(x => x.IsStartPoint).FirstOrDefault());

                SetWayPoint(oldStartPointIdx, wayPoint);

                HasStartPoint = true;
            }
            else if (wayPoint.IsEndPoint)
            {
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

        MoveMap(oldIdx, wayPoint);
    }

    private void MoveMap(int oldIdx, WayPoint wayPoint)
    {
        //replace the found location with the clickedLocation
        IsReady = false;

        if (places != null && places.Count > 0 && oldIdx >= 0)
            places.RemoveAt(oldIdx);

        //show location on map
        var place = new Place
        {
            Location = new Location { Latitude = wayPoint.Latitude, Longitude = wayPoint.Longitude },
            Address = wayPoint.Address, 
            Description = wayPoint.Name,
        };

        places.Add(place);
        BindablePlaces = new ObservableCollection<Place>(places);
        IsReady = true;

    }
   

    public override bool Validate()
    {
        return true;
    }
}

