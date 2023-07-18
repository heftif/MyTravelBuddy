using System;
namespace MyTravelBuddy.ViewModels
{
	public abstract partial class DomainObjectViewModel : BaseViewModel
	{


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

