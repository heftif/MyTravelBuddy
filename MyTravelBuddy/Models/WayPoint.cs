﻿using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MyTravelBuddy.Models;

[Table("waypoints")]
public class WayPoint : IDomainObject
{

    [PrimaryKey, AutoIncrement]
    public int WayPointId { get; set; }

    public string Name { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public bool IsStartPoint { get; set; }
    public bool IsEndPoint { get; set; }

    [ForeignKey(typeof(DayPlan))]
    public int DayPlanId { get; set; }

    public int SortOrder { get; set; }

    public int GetId()
    {
        return WayPointId;
    }

    public string GetAddress()
    {
        string[] str = new string[] { Street, City, Country };

        return string.Join(", ", str);
    }

}

