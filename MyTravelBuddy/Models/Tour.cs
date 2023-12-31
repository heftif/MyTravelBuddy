﻿using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MyTravelBuddy.Models;

[Table("Tours")]
public class Tour : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int TourId { get; set; }

    [ForeignKey(typeof(TourType))] // define foreign key
    public int TourTypeId { get; set; }

    [ForeignKey(typeof(Vehicle))] // define foreign key
    public int VehicleToAndFromId { get; set; }

    [ForeignKey(typeof(Vehicle))] // define foreign key
    public int VehicleAtLocationId { get; set; }

    public string Name { get; set; }
    public string GeneralLocation { get; set; }
    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }

    public byte[] Image { get; set; }

    public bool Active { get; set; }
    public bool Current { get; set; }
    //public string StartsAt { get; set; }
    //public string EndsAt { get; set; }

    public Tour()
	{
		
	}

    public int GetId()
    {
        return TourId;
    }
}

