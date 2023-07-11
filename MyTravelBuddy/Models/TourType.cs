using System;
using SQLite;

namespace MyTravelBuddy.Models;

public class TourType : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int TourTypeId { get; set; }

    public string Text { get; set; }
    public string ImagePath { get; set; }

    public TourType()
	{
	}

    public int GetId()
    {
        return TourTypeId;
    }
}

