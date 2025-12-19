using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace GrayWolf.Common
{
    public class Database : IDatabase
    {
        private readonly SQLiteAsyncConnection _sqlconnection;

        private readonly TaskCompletionSource<bool> _initTCS = new TaskCompletionSource<bool>();

        public Database()
        {
            //Getting conection and Creating table
           // _sqlconnection = DependencyService.Get<ISQLite>().GetConnection();
            _sqlconnection =  Ioc.Default.GetService<ISQLite>().GetConnection();

            InitTables();
        }

        private async void InitTables()
        {
            try
            {
                await Task.WhenAll(
                    _sqlconnection.CreateTableAsync<LogFileDBO>(),
                    _sqlconnection.CreateTableAsync<AttachmentDBO>(),
                    _sqlconnection.CreateTableAsync<GrayWolfDeviceDBO>(),
                    _sqlconnection.CreateTableAsync<LogRowDBO>(),
                    _sqlconnection.CreateTableAsync<ReadingDBO>(),
                    _sqlconnection.CreateTableAsync<UnitConversionDBO>()
                );
                _initTCS.TrySetResult(true);
            }
            catch (Exception ex)
            {
                await AlertService.Instance.DisplayError(ex);
                _initTCS.TrySetException(ex);
            }
        }

        public async Task InsertAsync<T>(T item) where T : IDbo
        {
            await _initTCS.Task;
            await _sqlconnection.InsertAsync(item);
        }

        public async Task UpsertAsync<T>(T item) where T : IDbo
        {
            await _initTCS.Task;
            await _sqlconnection.InsertOrReplaceAsync(item);
        }

        public async Task UpsertAllAsync<T>(IEnumerable<T> items) where T : IDbo
        {
            await _initTCS.Task;
            var tasks = new List<Task>();
            foreach (var item in items)
            {
                tasks.Add(UpsertAsync(item));
            }
            await Task.WhenAll(tasks);
        }

        public async Task InsertAsync<T>(IEnumerable<T> items) where T : IDbo
        {
            await _initTCS.Task;
            await _sqlconnection.InsertAllAsync(items);
        }

        public async Task UpdateAsync<T>(T item) where T : IDbo
        {
            await _initTCS.Task;
            await _sqlconnection.UpdateAsync(item);
        }

        public async Task UpdateAllAsync<T>(IEnumerable<T> items) where T : IDbo
        {
            await _initTCS.Task;
            await _sqlconnection.UpdateAllAsync(items);
        }

        public async Task DeleteItemAsync<T>(object id)
            where T : IDbo
        {
            await _initTCS.Task;
            await _sqlconnection.DeleteAsync<T>(id);
        }

        public async Task DeleteAllItemsAsync<T>()
            where T : IDbo, new()
        {
            await _initTCS.Task;
            await _sqlconnection.DeleteAllAsync<T>();
        }

        public async Task DeleteItemsAsync<T>(Func<T, bool> predicate)
            where T : IDbo, new()
        {
            await _initTCS.Task;
            await _sqlconnection.Table<T>().DeleteAsync((item) => predicate.Invoke(item));
        }

        public async Task DeleteItemsAsync<T>(IEnumerable<object> ids)
            where T : IDbo
        {
            await _initTCS.Task;
            var tasks = new List<Task>();
            foreach (var id in ids)
            {
                tasks.Add(_sqlconnection.DeleteAsync<T>(id));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<T> GetItemAsync<T>(object id)
            where T : IDbo, new()
        {
            await _initTCS.Task;
            return await _sqlconnection.FindAsync<T>(id);
        }

        public async Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate)
            where T : IDbo, new()
        {
            await _initTCS.Task;
            return await _sqlconnection.Table<T>()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<List<T>> GetItemsAsync<T>()
            where T : IDbo, new()
        {
            await _initTCS.Task;
            return await _sqlconnection.Table<T>()
                .ToListAsync();
        }

        public async Task<bool> IsExistAsync<TDbo, TKey>(TKey id)
            where TDbo : IDbo<TKey>, new()
        {
            await _initTCS.Task;
            var count = await _sqlconnection.Table<TDbo>().CountAsync(x => $"{x.Id}" == $"{id}");
            return count != 0;
        }
    }
}
