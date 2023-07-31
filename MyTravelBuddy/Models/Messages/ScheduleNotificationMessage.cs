using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models;

public class ScheduleNotificationMessage : ValueChangedMessage<PlanningItemViewModel>
{
    public ScheduleNotificationMessage(PlanningItemViewModel value) : base(value) { }

}

