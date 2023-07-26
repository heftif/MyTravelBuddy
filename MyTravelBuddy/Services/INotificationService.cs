using System;
namespace MyTravelBuddy.Services;

public interface INotificationService
{
    bool Cancel(int id);
    Task<bool> AreNotificationsEnabled();
    bool RequestNotificationPermission();
}

