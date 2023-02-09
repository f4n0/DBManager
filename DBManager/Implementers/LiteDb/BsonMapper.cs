using DBManager.Helpers;
using DBManager.Infrastructure;
using LiteDB;
using LiteDB.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DBManager.Implementers.LiteDb
{
  public class RecordBsonMapper
  {
    public RecordBsonMapper(LiteDatabase db)
    {
      var AllTables = ReflectionUtilities.getAllTables();



      foreach (var table in AllTables)
      {
        var allFields = ReflectionUtilities.GetAllFields(table);

        db.Mapper.RegisterType(table, SerializeGeneric, value => DeserializeGeneric(table, value));
      }
    }


    
    public static object DeserializeGeneric(Type obj ,BsonValue value)
    {
      if (!(value is BsonDocument doc)) throw new NotSupportedException();
      doc = (BsonDocument)value;

      object o = null;

      if (obj != null)
      {
        doc.TryGetValue("Id", out var id);
        o = ReflectionUtilities.CreateInstanceForRecord(obj, id.AsString);
        foreach (var field in ReflectionUtilities.GetAllBaseFieldsExcludedSystem(o.GetType()))
        {
          if (doc.TryGetValue(field.Name, out var bsonValue))
          {
            obj.GetProperty(field.Name).SetValue(o, bsonValue.RawValue, null);
          }
        }

      }

      return o;
    }

    


    private static BsonDocument SerializeGeneric(object item) 
    {
      var ret = new BsonDocument();
      foreach (var field in ReflectionUtilities.GetAllFields(item.GetType()))
      {
        ret.Add(field.Name, new BsonValue(field.GetValue(item, null)));
      }
      return ret;
    }


  }
}
