using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models;

public class ReloadItemMessage : ValueChangedMessage<Tour>
{
    public ReloadItemMessage(Tour value) : base(value) { }
}


