using System;
using AsyncAwaitBestPractices;

namespace MyTravelBuddy.ViewModels;

public partial class MainViewModel : BaseViewModel
{
	

	public MainViewModel(SqlDatabase database) : base(database)
	{
		LoadAsync().SafeFireAndForget();
    }

	async Task LoadAsync()
	{
		var tours = await Database.ListAll<Tour>();

		var toursPlanned = tours.Count > 0;

		if(toursPlanned)
		{
			await LoadTours();
        }
	}

	async Task LoadTours()
	{

	}

	[RelayCommand]
	public async Task CreateNewTourAsync()
	{
		if (IsBusy)
			return;

		int? tourId = 0;

        await Shell.Current.GoToAsync(nameof(TourDetailsView), true, new Dictionary<string, object>
        {
            {"TourId", tourId}
        });
    }
}

