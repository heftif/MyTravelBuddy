using System;

namespace MyTravelBuddy.ViewModels;

public partial class BaseViewModel : ObservableObject
{ 
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	bool isBusy;

	[ObservableProperty]
	string title;

	[ObservableProperty]
	bool isRefreshing;

	public bool IsNotBusy => !IsBusy;

	public bool IsLoaded { get; set; }

    public BaseViewModel()
	{
    }
}

