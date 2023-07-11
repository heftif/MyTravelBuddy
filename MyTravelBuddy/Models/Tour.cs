using System;
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
    public int VehicleId { get; set; }

    public string GeneralLocation { get; set; }
    public DateTime StartsOn { get; set; }
    public DateTime? EndsOn { get; set; }

    public string StartsAt { get; set; }
    public string EndsAt { get; set; }

    public Tour()
	{
		
	}

    public int GetId()
    {
        return TourId;
    }
}

