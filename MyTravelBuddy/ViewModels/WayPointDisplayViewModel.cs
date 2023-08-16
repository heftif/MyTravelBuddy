using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using MyTravelBuddy.Models.Messages;

namespace MyTravelBuddy.ViewModels;

public partial class WayPointDisplayViewModel : DomainObjectViewModel, IQueryAttributable
{
    DayPlan dayPlan;

    public ObservableCollection<WayPoint> WayPoints { get; } = new();

    [ObservableProperty]
    bool hasStart;

    [ObservableProperty]
    bool hasEnd;

    public WayPointDisplayViewModel()
	{
        WeakReferenceMessenger.Default.Register<ReloadWayPointsMessage>(this, ReloadWayPoints);
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
            var sortedWayPoints = wayPoints.OrderBy(x => x.SortOrder);

            if (wayPoints.Any(x => x.IsStartPoint))
                HasStart = true;

            if (wayPoints.Any(x => x.IsEndPoint))
                HasEnd = true;


            foreach (var wayPoint in wayPoints)
                WayPoints.Add(wayPoint);

            Load();

            IsLoaded = true;

        }
    }

    private void Load()
    {

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
            }
            else if (wayPoint.IsEndPoint)
            {
                var oldEndPointIdx = WayPoints.IndexOf(WayPoints.Where(x => x.IsEndPoint).FirstOrDefault());

                SetWayPoint(oldEndPointIdx, wayPoint);

            }
        }
    }

    private void SetWayPoint(int oldIdx, WayPoint wayPoint)
    {
        var newIdx = wayPoint.SortOrder;

        if (oldIdx >= 0)
        {
            WayPoints.RemoveAt(oldIdx);
            WayPoints.Insert(newIdx, wayPoint);
        }
        else
        {
            WayPoints.Insert(newIdx, wayPoint);
        }
    }
   

    public override bool Validate()
    {
        return true;
    }
}

