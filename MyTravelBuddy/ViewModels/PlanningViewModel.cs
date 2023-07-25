using System;

namespace MyTravelBuddy.ViewModels;

public class PlanningViewModel : TourDetailsCollectionBase, IQueryAttributable
{
    public PlanningViewModel()
    {
        IsLoaded = false;

        SelectedMenuItem = "Planning";

        IsLoaded = true;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Tour = query["Tour"] as Tour;

        tourId = Tour.TourId;
    }

    public override bool Validate()
    {
        return true;
    }
}

