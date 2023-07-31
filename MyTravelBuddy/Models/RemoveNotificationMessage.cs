using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models;

public class RemoveNotificationMessage : ValueChangedMessage<int>
{
    public RemoveNotificationMessage(int value) : base(value) { }
}

