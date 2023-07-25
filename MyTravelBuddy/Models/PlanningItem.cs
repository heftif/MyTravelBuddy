using SQLite;

namespace MyTravelBuddy.Models;

[Table("PlanningItems")]
public class PlanningItem : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int PlanningItemId { get; set; }

    public int GetId()
    {
        return PlanningItemId;
    }
}

