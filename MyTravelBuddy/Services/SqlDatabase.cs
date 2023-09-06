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
        //await Database.DeleteAllAsync<WayPoint>();

        await CreateTables();
        await AddPlanningItems();

        initiated = true;

        return;
    }

    async Task CreateTables()
    {
        try
        {

            await Database.CreateTableAsync<Tour>();
            await Database.CreateTableAsync<PushSetting>();
            await Database.CreateTableAsync<PlanningItem>();
            await Database.CreateTableAsync<DayPlan>();
            await Database.CreateTableAsync<WayPoint>();

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

    async Task AddPlanningItems()
    {
        var tours = await ListAll<Tour>();

        var tourIds = tours.Select(x => x.TourId).ToList();


        foreach (var tourId in tourIds)
        {
            await AddPlanningItems(tourId);
        }
            
    }

    public async Task AddPlanningItems(int tourId)
    {
        var planningItems = await ListAll<PlanningItem>();

        //planning items already exists, we don't need to create them
        if (planningItems.Where(x => x.TourId == tourId).Any())
            {
                return;
            }
            else
            {
                //for fancier method, we could then do these reminders with the type of the travel in mind and look them up
                //these must then also be changed when the type of travel is changed.
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Book Flights", Description="Fix Start and End points of Destinations and Dates, then book flights", DaysBeforeEvent = 92, IsDone = false });
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Check Passport Valid Dates", Description="Check that all your traveling documents are up to date and valid more than half a year after your travel (required by some countries)", DaysBeforeEvent = 92, IsDone = false });
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Book Accomodations", Description="Fix a route and book awesome accomodations", DaysBeforeEvent = 60, IsDone = false });
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Check Medication", Description="Check that you have all your necessary medication, get refills and check if all medication is allowed in the location you're travelling to",DaysBeforeEvent = 30, IsDone = false });
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Print Documents", Description="Print all necessary documents and also, make a copy of your passport", DaysBeforeEvent = 7, IsDone = false });
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Power Adapter", Description = "Check Adapter at location and see if you have the right one at home", DaysBeforeEvent = 7, IsDone = false });
                await SaveItemAsync(new PlanningItem { TourId = tourId, Name = "Double Check Flights", Description="Check if all flights are departing as scheduled and no further information is available", DaysBeforeEvent = 3, IsDone = false });
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

    //todo: implement method to list all with a condition, to use optimised sql!
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

