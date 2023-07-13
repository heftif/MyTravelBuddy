using System;
using System.Linq.Expressions;

namespace MyTravelBuddy.Services;

public interface ISqlDatabase
{
    Task<int> SaveItemAsync<T>(T item) where T : IDomainObject;
    Task<IList<TObject>> ListAll<TObject>() where TObject : IDomainObject, new();
    Task<TObject> GetObject<TObject>(int pk) where TObject : new();
}

