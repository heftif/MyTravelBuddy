using System;
using SQLite;

namespace MyTravelBuddy.Models;

public class TourType : TourCollectionBase, IDomainObject
{

    [PrimaryKey, AutoIncrement]
    public int TourTypeId { get; set; }

    public TourType()
	{
	}

    public int GetId()
    {
        return TourTypeId;
    }

    public bool IsHike()
    {
        return Usage == TourUsage.HikeUsage;
    }

    public bool IsBeach()
    {
        return Usage == TourUsage.BeachUsage;
    }

    public bool IsBike()
    {
        return Usage == TourUsage.BikeUsage;
    }

    public bool IsRoadTrip()
    {
        return Usage == TourUsage.RoadTripUsage;
    }

    public bool IsCityTrip()
    {
        return Usage == TourUsage.CityTripUsage;
    }

    public bool IsBusinessTrip()
    {
        return Usage == TourUsage.BusinessTripUsage;
    }

    public bool IsCruise()
    {
        return Usage == TourUsage.CruiseUsage;
    }
}

