using SQLiteNetExtensions.Attributes;
using SQLite;

namespace MyTravelBuddy.Models;

[Table("PlanningItems")]
public class PlanningItem : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int PlanningItemId { get; set; }

    [ForeignKey(typeof(Tour))] // define foreign key
    public int TourId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }

    public int DaysBeforeEvent { get; set; }
    public DateTime DueDate { get; set; }

    public int GetId()
    {
        return PlanningItemId;
    }
}

