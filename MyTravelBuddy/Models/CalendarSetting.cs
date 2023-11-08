using System;
using SQLite;

namespace MyTravelBuddy.Models;

[Table("CalendarSettings")]
public class CalendarSetting : IDomainObject
{
    [PrimaryKey, AutoIncrement]
    public int CalendarSettingId { get; set; }

    public string Name { get; set; }
    public string Id { get; set; }

    public int GetId()
    {
        return CalendarSettingId;
    }
}

