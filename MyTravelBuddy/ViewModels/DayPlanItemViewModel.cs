using System;
namespace MyTravelBuddy.ViewModels;

public partial class DayPlanItemViewModel : DomainObjectViewModel
{
    public DayPlan DayPlan;

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

    public bool IsChanged;

    public DayPlanItemViewModel(DayPlan dayPlan, bool isChanged = false)
    {
        DayPlan = dayPlan;

        Location = dayPlan.Location;
        TourDay = dayPlan.TourDay;
        Date = dayPlan.Date;
        HasDocuments = dayPlan.HasDocuments;
        InActive = dayPlan.InActive;

        IsChanged = isChanged;

        //visualisation
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

