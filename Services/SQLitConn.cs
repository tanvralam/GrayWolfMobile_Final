using GrayWolf.Interfaces;
using SQLite;
using System.IO;
using Microsoft.Maui.Storage;
namespace GrayWolf.Services
{
    public class SQLiteConn : ISQLite
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var dbName = "mydatabase.sqlite";
            var dbPath = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
            var path = Path.Combine(dbPath, dbName);
            return new SQLiteAsyncConnection(path);
        }
    }
}
