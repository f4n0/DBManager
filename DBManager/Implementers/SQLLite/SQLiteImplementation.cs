using DBManager.Helpers;
using DBManager.Infrastructure;
using SQLite;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Implementers.SQLLite
{
  internal class SQLiteImplementation : IStorage
  {
    public void Delete<T>(T[] items) where T : class
    {
      foreach (var item in items)
      {
        database.Delete(item);
      }
    }

    public IEnumerable<T> List<T>(List<Expression<Func<T, bool>>> Filters = null) where T : class
    {
      
      var test = new TableQuery<T>(database);
      foreach (var filter in Filters)
      {
        test = test.Where(filter);
      }
     return test.AsEnumerable();
    }

    public void Save()
    {
      database.Commit();
    }

    public void Store<T>(T[] items) where T : class
    {
      database.InsertAll(items);
    }

    public SQLiteImplementation(string connectionString)
    {
      Initialize(connectionString);
    }

    private SQLiteConnection database;


    private void Initialize(string connectionString)
    {
      database = new SQLiteConnection(connectionString);

      foreach (var item in ReflectionUtilities.getAllTables())
      {
        database.CreateTable(item, CreateFlags.ImplicitPK|CreateFlags.ImplicitIndex);
      }

    }
  }
}
