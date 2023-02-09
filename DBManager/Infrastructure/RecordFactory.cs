using DBManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DBManager.Infrastructure
{
  public static class RecordFactory
  {

    public static Record<T> Open<T>() where T : class
    {
      return new Record<T>();
    }

    public static T Get<T>(string id) where T : class
    {
      return new Record<T>().Get(id);
    }

    public static Record<object> CreateGeneric()
    {
      return Open<object>();
    }

    public static T CreateModel<T>()
    {
      return CreateModel<T>(XmlConvert.ToString(Guid.NewGuid()));
    }

    public static T CreateModel<T>(string id)
    {
      var constructors = typeof(T).GetConstructors();

      if (!string.IsNullOrEmpty(id))
      {
        var constructor = constructors.FirstOrDefault(c =>
        {
          var parameters = c.GetParameters();
          return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
        });
        if (constructor != null)
          return (T)constructor.Invoke(new object[] { id });
      }
      else
      {
        var constructor = constructors.FirstOrDefault(c => !c.GetParameters().Any());
        if (constructor != null)
          return (T)constructor.Invoke(new object[] { });
      }

      throw new NotSupportedException($"No valid constructor found on type '{typeof(T)}'.");
    }
  }
}