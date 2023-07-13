using System;
using SQLite;

namespace MyTravelBuddy.Models;

public class Vehicle : TourCollectionBase, IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int VehicleId { get; set; }

    public Vehicle()
	{
	}

    public int GetId()
    {
        return VehicleId;
    }
}

