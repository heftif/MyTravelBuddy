using System;
namespace MyTravelBuddy.ViewModels
{
	public abstract partial class DomainObjectViewModel : BaseViewModel
	{
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNameNotValid))]
        bool isNameValid;

        public bool IsNameNotValid => !IsNameValid;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDestinationNotValid))]
        bool isDestinationValid;

        public bool IsDestinationNotValid => !IsDestinationValid;

        public DomainObjectViewModel()
		{
		}

		protected async Task SaveDomainObject<T>(T item) where T : IDomainObject
		{
			if (Validate())
			{
				await App.DatabaseService.SaveItemAsync(item);
			}
		}

        public abstract bool Validate();
    }
}

