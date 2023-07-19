using System;
namespace MyTravelBuddy.ViewModels
{
	public abstract partial class DomainObjectViewModel : BaseViewModel
	{


        public DomainObjectViewModel()
		{

		}

		protected async Task<bool> SaveDomainObject<T>(T item) where T : IDomainObject
		{
			if (Validate())
			{
				await App.DatabaseService.SaveItemAsync(item);
				return true;
			}

			return false;
		}

        public abstract bool Validate();
    }
}

