using System;

namespace MyTravelBuddy.ViewModels;

public static class Mapper
{
	public static WayPoint Map(WayPoint wayPoint, Place place, string wayPointType, int dayPlanId)
	{
        wayPoint.Latitude = place.Location.Latitude;
        wayPoint.Longitude = place.Location.Longitude;
        wayPoint.DayPlanId = dayPlanId;
        wayPoint.Name = place.Description;
        wayPoint.Address = place.Address;

        if (wayPointType == "start")
        {
            wayPoint.SortOrder = 0;
            wayPoint.IsStartPoint = true;
            wayPoint.IsEndPoint = false;
        }

        if (wayPointType == "end")
        {
            wayPoint.SortOrder = 1;
            wayPoint.IsStartPoint = false;
            wayPoint.IsEndPoint = true;
        }

        return wayPoint;
    }

    public static PlanningItem Map(PlanningItem planningItem, PlanningItemViewModel vm)
    {
        planningItem.Name = vm.Name;
        planningItem.Description = vm.Description;
        planningItem.IsDone = vm.IsDone;
        planningItem.DaysBeforeEvent = vm.DaysBeforeEvent;
        planningItem.NotificationEnabled = vm.NotificationEnabled;

        return planningItem;
    }

    public static PushSetting Map(PushSetting currentPushSetting, SettingsPageViewModel vm)
    {
        currentPushSetting.CloseReminders = vm.CloseRemindersEnabled;
        currentPushSetting.FarReminders = vm.FarRemindersEnabled;

        return currentPushSetting;
    }

    public static Tour Map(Tour tour, TourOverviewCollectionViewModel vm)
    {
        tour.VehicleToAndFromId = vm.SelectedVehicleToAndFrom.VehicleId;
        tour.VehicleAtLocationId = vm.SelectedVehicleAtLocation.VehicleId;
        tour.TourTypeId = vm.SelectedTourType.TourTypeId;

        tour.Name = vm.Name;
        tour.GeneralLocation = vm.GeneralLocation;
        tour.StartsOn = vm.StartsOn;
        tour.EndsOn = vm.EndsOn;
        tour.Image = vm.TourImage;

        return tour;
    }
}

