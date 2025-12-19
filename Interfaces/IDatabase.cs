using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IDatabase
    {
        //Task<string> BeginTransactionAsync();

        //Task Commit(string transactionId);

        //Task CancelTransaction(string transactionId);

        Task InsertAsync<T>(T item)
            where T : IDbo;

        Task InsertAsync<T>(IEnumerable<T> items)
            where T : IDbo;

        Task UpsertAsync<T>(T item)
            where T : IDbo;

        Task UpsertAllAsync<T>(IEnumerable<T> items) where T : IDbo;

        Task UpdateAsync<T>(T item)
            where T : IDbo;

        Task UpdateAllAsync<T>(IEnumerable<T> items)
            where T : IDbo;

        Task DeleteItemAsync<T>(object id)
            where T : IDbo;

        Task DeleteItemsAsync<T>(Func<T, bool> predicate)
            where T : IDbo, new();

        Task DeleteItemsAsync<T>(IEnumerable<object> ids)
            where T : IDbo;

        Task DeleteAllItemsAsync<T>()
            where T : IDbo, new();

        Task<T> GetItemAsync<T>(object id)
            where T : IDbo, new ();

        Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate)
            where T : IDbo, new ();

        Task<List<T>> GetItemsAsync<T>()
            where T : IDbo, new();

        Task<bool> IsExistAsync<TDbo, TKey>(TKey id)
            where TDbo : IDbo<TKey>, new ();
    }
}
