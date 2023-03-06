using DBManager.Helpers;
using DBManager.Infrastructure;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Implementers.LiteDb
{
  internal class LiteDbImplementation : IStorage
  {
    public void Delete<T>(T[] items) where T : class
    {

      var coll = Database.GetCollection<T>(typeof(T).Name);
      foreach (var item in items)
      {
        coll.Delete(new BsonValue((item as Infrastructure.IStorageItem).Id));
      }
    }

    public IEnumerable<T> List<T>(List<Expression<Func<T, bool>>> Filters = null) where T : class
    {
      var coll = Database.GetCollection<T>(typeof(T).Name);
      var q = coll.Query();
      foreach (var filter in Filters)
      {
        q = q.Where(filter);
      }

      var res = q.ToArray();

      return res;
    }

    public void Save()
    {
      //null
    }

    public void Store<T>(T[] items) where T : class
    {
      var test = new BsonDocument();

      var coll = Database.GetCollection(items.First().GetType().Name);
      var test2 = Database.Mapper.ToDocument(items.First());
      coll.Upsert(test2);
    }

    public LiteDbImplementation(string connectionString)
    {
      Initialize(connectionString);
    }

    private LiteDatabase Database { get; set; }


    public void Initialize(string connectionString)
    {
      Database = new LiteDatabase(connectionString);
      new RecordBsonMapper(Database);
    }
  }
}
