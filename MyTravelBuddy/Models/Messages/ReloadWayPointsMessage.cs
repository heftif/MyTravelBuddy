using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models.Messages;

    public class ReloadWayPointsMessage : ValueChangedMessage<WayPoint>
    {
        public ReloadWayPointsMessage(WayPoint value) : base(value) { }
    }






