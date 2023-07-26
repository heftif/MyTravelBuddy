using System;

namespace MyTravelBuddy.Services;

public class NotificationService : INotificationService
{
	public NotificationService()
	{
	}

    public Task<bool> AreNotificationsEnabled()
    {
        throw new NotImplementedException();
    }

    public bool Cancel(int id)
    {
        throw new NotImplementedException();
    }

    public bool RequestNotificationPermission()
    {
        throw new NotImplementedException();
    }
}

