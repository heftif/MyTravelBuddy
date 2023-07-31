using System;
namespace MyTravelBuddy.ViewModels;

public partial class DayPlanItemViewModel : DomainObjectViewModel
{
    DayPlan dayPlan;

    [ObservableProperty]
    string location;

    [ObservableProperty]
    int tourDay;

    [ObservableProperty]
    DateTime? date;

    [ObservableProperty]
    bool hasDocuments;

    [ObservableProperty]
    bool inActive;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string dateString;

    public DayPlanItemViewModel(DayPlan dayPlan)
    {
        this.dayPlan = dayPlan;

        Location = dayPlan.Location;
        TourDay = dayPlan.TourDay;
        Date = dayPlan.Date;
        HasDocuments = dayPlan.HasDocuments;
        InActive = dayPlan.InActive;

        Name = $"Day {TourDay}: {Location}";
        if (dayPlan.Date != null)
        {
            DateString = dayPlan.Date.Value.ToString("dd.MM.yyyy");
        }
    }

    public override bool Validate()
    {
        return true;
    }
}

