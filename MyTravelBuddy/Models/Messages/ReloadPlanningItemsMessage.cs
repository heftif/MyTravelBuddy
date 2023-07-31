using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models;

public class ReloadPlanningItemsMessage : ValueChangedMessage<PlanningItemViewModel>
{
    public ReloadPlanningItemsMessage(PlanningItemViewModel value) : base(value) { }
}

