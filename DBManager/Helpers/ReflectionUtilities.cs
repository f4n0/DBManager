using DBManager.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DBManager.Helpers
{
  internal static class ReflectionUtilities
  {
    public static IEnumerable<Type> getAllTables() =>
                        AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes())
                       .Where(type => type.IsSubclassOf(typeof(RecordBase)));

    public static IEnumerable<PropertyInfo> GetAllFields<T>() where T:RecordBase=>
      typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);


    public static IEnumerable<PropertyInfo> GetAllFields(Type T) =>
      T.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public static IEnumerable<PropertyInfo> GetAllBaseFieldsExcludedSystem(Type T) =>
      T.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(o=> o.Name != "Id");

    public static ConstructorInfo[] GetConstructors(Type T) => T.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

    public static object CreateInstanceForRecord(Type obj, string id)
    {
      var constructors = obj.GetConstructors();

      if (!string.IsNullOrEmpty(id))
      {
        var constructor = constructors.FirstOrDefault(c =>
        {
          var parameters = c.GetParameters();
          return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
        });
        if (constructor != null)
          return constructor.Invoke(new object[] { id });
      }
      else
      {
        var constructor = constructors.FirstOrDefault(c => !c.GetParameters().Any());
        if (constructor != null)
          return constructor.Invoke(new object[] { });
      }

      throw new NotSupportedException($"No valid constructor found on type '{obj}'.");
    }

    public static MemberInfo GetRecordPK(TypeInfo ti) =>
      ti.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(o => o.Name == "Id").First();


    public static bool isIStorage(Type t) =>
                    t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IStorageItem));
  }
}
