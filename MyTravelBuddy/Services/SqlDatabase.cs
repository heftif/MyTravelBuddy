using System;
using System.Linq.Expressions;
using AsyncAwaitBestPractices;
using SQLite;

namespace MyTravelBuddy.Services;

public class SqlDatabase : ISqlDatabase
{
    public SQLiteAsyncConnection Database;

    bool initiated = false;

    public SqlDatabase()
    {

    }

    async Task Init()
    {
        if (initiated)
        {
            return;
        }

        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        //await Database.DeleteAllAsync<Vehicle>();
        //await Database.DeleteAllAsync<TourType>();


        await CreateTables();

        initiated = true;

        return;
    }

    async Task CreateTables()
    {
        try
        {

            await Database.CreateTableAsync<Tour>();

            //creates a table with the schema of the given object
            var res1 = await Database.CreateTableAsync<Vehicle>();

            if (res1 == CreateTableResult.Created) //res1 == CreateTableResult.Created
            {
                await SaveItemAsync(new Vehicle { Text = "By Foot", ImagePath = "feet.png", Usage = TourUsage.WalkUsage });
                await SaveItemAsync(new Vehicle { Text = "By Bike", ImagePath = "bike.png", Usage = TourUsage.CycleUsage });
                await SaveItemAsync(new Vehicle { Text = "By Car", ImagePath = "car.png", Usage = TourUsage.CarUsage });
                await SaveItemAsync(new Vehicle { Text = "By Van", ImagePath = "van.png", Usage = TourUsage.VanUsage });
                await SaveItemAsync(new Vehicle { Text = "By Train", ImagePath = "train.png", Usage = TourUsage.TrainUsage });
                await SaveItemAsync(new Vehicle { Text = "By Ship", ImagePath = "ship.png", Usage = TourUsage.ShipUsage });
                await SaveItemAsync(new Vehicle { Text = "By Boat", ImagePath = "boat.png", Usage = TourUsage.BoatUsage });
                await SaveItemAsync(new Vehicle { Text = "By Plane", ImagePath = "plane.png", Usage = TourUsage.PlaneUsage });
            }

            var res2 = await Database.CreateTableAsync<TourType>();

            if (res2 == CreateTableResult.Created) //res2 == CreateTableResult.Created
            {
                await SaveItemAsync(new TourType { Text = "Hike", ImagePath = "hike.png", Usage = TourUsage.HikeUsage });
                await SaveItemAsync(new TourType { Text = "Beach", ImagePath = "beachtrip.png", Usage = TourUsage.BeachUsage });
                await SaveItemAsync(new TourType { Text = "Bike Tour", ImagePath = "biketour.png", Usage = TourUsage.BikeUsage });
                await SaveItemAsync(new TourType { Text = "Road Trip", ImagePath = "roadtrip.png", Usage = TourUsage.RoadTripUsage });
                await SaveItemAsync(new TourType { Text = "City Trip", ImagePath = "citytrip.png", Usage = TourUsage.CityTripUsage });
                await SaveItemAsync(new TourType { Text = "Business Trip", ImagePath = "businesstrip.png", Usage = TourUsage.BusinessTripUsage });
                await SaveItemAsync(new TourType { Text = "Cruise", ImagePath = "cruisetrip.png", Usage = TourUsage.CruiseUsage });
            }

        }
        catch(Exception e)
        {
            await App.AlertService.ShowAlertAsync("Error", e.ToString());
        }
    }

    public async Task<int> SaveItemAsync<T>(T item) where T : IDomainObject
    {
        await Init();
        if (item.GetId() != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    // generic method for listing all domain objects of a table
    public async Task<IList<TObject>> ListAll<TObject>() where TObject : IDomainObject, new()
    {
        await Init();

        return await Database.Table<TObject>().ToListAsync();
    }


    // generic method for listing a given domain object of a table. Id must be marked as primary key!
    public async Task<TObject> GetObject<TObject>(int pk) where TObject : new()
    {
        await Init();

        return await Database.GetAsync<TObject>(pk);
        
    }

}

