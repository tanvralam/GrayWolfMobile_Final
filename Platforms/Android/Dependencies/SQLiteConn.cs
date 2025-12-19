using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;
using SQLite;

[assembly: Dependency(typeof(SQLiteConn))]
namespace GrayWolf.Droid.Dependencies
{
    public class SQLiteConn : ISQLite
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var dbName = "mydatabase.sqlite";
            var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(dbpath, dbName);
            var conn = new SQLiteAsyncConnection(path);
            return conn;
        }
    }
}