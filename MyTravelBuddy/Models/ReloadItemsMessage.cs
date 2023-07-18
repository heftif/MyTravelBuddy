using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models;

public class ReloadItemsMessage : ValueChangedMessage<bool>
{
    public ReloadItemsMessage(bool value) : base(value) { }
}


