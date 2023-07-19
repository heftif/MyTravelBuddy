using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyTravelBuddy.Models;

public class SetSelectedItemMessage : ValueChangedMessage<string>
{
    public SetSelectedItemMessage(string value) : base(value) { }
}

