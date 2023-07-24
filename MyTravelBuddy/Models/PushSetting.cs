using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MyTravelBuddy.Models;

[Table("PushSettings")]
public class PushSetting : IDomainObject
{

    [PrimaryKey, AutoIncrement]
    public int PushSettingId { get; set; }

    [ForeignKey(typeof(Tour))] // define foreign key
    public int TourId { get; set; }

    public bool FarReminders { get; set; }
    public bool CloseReminders { get; set; }

    public int GetId()
    {
        return PushSettingId;
    }
}

