using DBManager.Implementers;
using DBManager.Implementers.LiteDb;
using DBManager.Implementers.SQLLite;
using DBManager.Infrastructure;
using LiteDB;

namespace DBManager
{
  public class DBManager
  {
    private DBManager()
    {
    }
    private static readonly Lazy<DBManager> lazy = new Lazy<DBManager>(() => new DBManager());
    public static DBManager Instance => lazy.Value;

    public IStorage Storage { get; set; } 



    public void UseLiteDB(string connectionString)
    {
      Storage = new LiteDbImplementation(connectionString);
    }

    public void UseSQLite(string connectionString)
    {
      Storage = new SQLiteImplementation(connectionString);
    }
  }
}