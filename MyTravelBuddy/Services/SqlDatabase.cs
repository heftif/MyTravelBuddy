using System;
using System.Linq.Expressions;
using AsyncAwaitBestPractices;
using SQLite;

namespace MyTravelBuddy.Services;

public class SqlDatabase
{
    public SQLiteAsyncConnection Database;

    bool initiated = false;
    bool isLoaded = false;

    public SqlDatabase()
    {
        if (!isLoaded)
        {
            Init().SafeFireAndForget();
            isLoaded = true;
        }
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

        await CreateTables();

        initiated = true;

        return;
    }

    async Task CreateTables()
    {
        //creates a table with the schema of the given object
        var res1 = await Database.CreateTableAsync<Vehicle>();

        if (res1 == CreateTableResult.Created) //
        {
            await SaveItemAsync(new Vehicle { Text = "By Foot", ImagePath = "feet.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Bike", ImagePath = "bike.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Car", ImagePath = "car.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Van", ImagePath = "van.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Train", ImagePath = "train.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Ship", ImagePath = "ship.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Boat", ImagePath = "boat.svg" });
            await SaveItemAsync(new Vehicle { Text = "By Plane", ImagePath = "plane.svg" });
        }

        var res2 = await Database.CreateTableAsync<TourType>();

        if (res2 == CreateTableResult.Created) //
        {
            await SaveItemAsync(new TourType { Text = "Hike", ImagePath = "" });
            await SaveItemAsync(new TourType { Text = "Beach", ImagePath = "" });
            await SaveItemAsync(new TourType { Text = "Bike Tour", ImagePath = "" });
            await SaveItemAsync(new TourType { Text = "Road Trip", ImagePath = "" });
            await SaveItemAsync(new TourType { Text = "City Trip", ImagePath = "" });
            await SaveItemAsync(new TourType { Text = "Business Trip", ImagePath = "" });
            await SaveItemAsync(new TourType { Text = "Cruise", ImagePath = "" });
        }

        await Database.CreateTableAsync<Tour>();
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


    // generic method for listing a given domain object of a table
    public async Task<TObject> GetObject<TObject>(Expression<Func<TObject, bool>> where) where TObject : new()
    {
        await Init();


        return await Database.Table<TObject>().Where(where).FirstOrDefaultAsync();

        
    }

}

