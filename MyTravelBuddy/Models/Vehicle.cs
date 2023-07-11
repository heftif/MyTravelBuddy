using System;
using SQLite;

namespace MyTravelBuddy.Models;

public class Vehicle : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int VehicleId { get; set; }

    public string Text { get; set; }
    public string ImagePath { get; set; }

    public Vehicle()
	{
	}

    public int GetId()
    {
        return VehicleId;
    }
}

