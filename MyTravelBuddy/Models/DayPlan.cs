using System;
using SQLiteNetExtensions.Attributes;
using SQLite;

namespace MyTravelBuddy.Models;

[Table("DayPlans")]
public class DayPlan : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int DayPlanId { get; set; }

    [ForeignKey(typeof(Tour))] // define foreign key
    public int TourId { get; set; }

    public string Location { get; set; }
    public double TourDay { get; set; }
    public DateTime? Date { get; set; }

    public bool HasDocuments { get; set; }

    public bool InActive { get; set; }

    public int GetId()
    {
        return DayPlanId;
    }

    public static DayPlan Empty(double tourDay, int tourId, DateTime date)
    {
        return new DayPlan
        {
            TourId = tourId,
            TourDay = tourDay,
            Location = "",
            InActive = false,
            Date = date,
            HasDocuments = false,
        };
    }
}

