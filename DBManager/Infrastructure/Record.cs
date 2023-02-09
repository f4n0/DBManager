using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Infrastructure
{
  public sealed class Record<T> where T : class
  {

    private Dictionary<string, PropertyInfo> Properties { get; }

    private List<Predicate<T>> Filters { get; }


    /// <summary>
    /// </summary>
    public Record()
    {
      Properties = new Dictionary<string, PropertyInfo>();
      Filters = new List<Predicate<T>>();
    }

    private static string GetMemberName(Expression<Func<T, object>> exp)
    {
      var memberExp = exp.Body as MemberExpression;
      if (memberExp == null && exp.Body is UnaryExpression uex)
        memberExp = uex.Operand as MemberExpression;
      if (memberExp == null)
        throw new NotSupportedException($"The expression {exp} is not supported.");
      var memberName = memberExp.Member.Name;
      return memberName;
    }

    private IEnumerable<T> FilterEntities(IEnumerable<T> items)
    {
      return items.Where(item =>
      {
        return Filters.All(predicate => predicate(item));
      });
    }

    /// <summary>
    /// Clears all filters from this instance.
    /// </summary>
    /// <returns>The instance itself.</returns>
    public Record<T> Reset()
    {
      Filters.Clear();
      return this;
    }

    /// <summary>
    /// Adds the specified expression as a filter to the current instance.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRange(Predicate<T> expression)
    {
      Filters.Add(expression);
      return this;
    }

    /// <summary>
    /// Adds the specified expression as a filter to the current instance.
    /// </summary>
    /// <param name="name">An expression used to find the field name.</param>
    /// <param name="expression">The filter expression.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRange<TValue>(Expression<Func<T, object>> name, Func<TValue, bool> expression)
    {
      return SetRange(GetMemberName(name), expression);
    }

    /// <summary>
    /// Adds the specified expression as a filter to the current instance.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <param name="expression">The filter expression.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRange<TValue>(string name, Func<TValue, bool> expression)
    {
      SetRange(obj =>
      {
        var propValue = GetPropertyValue<TValue>(obj, name);
        return expression(propValue);
      });
      return this;
    }

    /// <summary>
    /// Adds a case-sensitive string filter to the current instance.
    /// </summary>
    /// <param name="name">An expression used to find the field name.</param>
    /// <param name="value">The filter value.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRangeCs(Expression<Func<T, object>> name, string value)
    {
      return SetRangeCs(GetMemberName(name), value);
    }

    /// <summary>
    /// Adds a case-sensitive string filter to the current instance.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <param name="value">The filter value.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRangeCs(string name, string value)
    {
      SetRange(obj =>
      {
        var propValue = GetPropertyValue<string>(obj, name);
        return CompareValues(propValue, value);
      });
      return this;
    }

    /// <summary>
    /// Adds a case-insensitive string filter to the current instance.
    /// </summary>
    /// <param name="name">An expression used to find the field name.</param>
    /// <param name="value">The filter value.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRangeCi(Expression<Func<T, object>> name, string value)
    {
      return SetRangeCi(GetMemberName(name), value);
    }

    /// <summary>
    /// Adds a case-insensitive string filter to the current instance.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <param name="value">The filter value.</param>
    /// <returns>The instance itself.</returns>
    public Record<T> SetRangeCi(string name, string value)
    {
      SetRange(obj =>
      {
        var propValue = GetPropertyValue<string>(obj, name);
        return CompareValues(propValue?.ToLowerInvariant(), value?.ToLowerInvariant());
      });
      return this;
    }


    private static bool CompareValues(object objValue, object value)
    {
      if (objValue == null && value == null) return true;
      if (objValue == null || value == null) return false;
      return objValue.Equals(value);
    }

    private TValue GetPropertyValue<TValue>(T item, string name)
    {
      try
      {
        var prop = GetProperty(name);
        return (TValue)prop.GetValue(item);
      }
      catch (InvalidCastException ex)
      {
        throw new InvalidOperationException($"The property '{name}' on type '{typeof(T)}' cannot be cast to '{typeof(TValue)}'.", ex);
      }
    }

    private PropertyInfo GetProperty(string name)
    {
      if (!Properties.ContainsKey(name))
      {
        var prop = typeof(T).GetProperty(name);
        if (prop == null)
          throw new InvalidOperationException($"The property '{name}' was not found on object of type '{typeof(T)}'");
        Properties.Add(name, prop);
      }

      return Properties[name];
    }

    private static void AssertIdSet(IEnumerable<object> items)
    {
      var invalidItem = items.FirstOrDefault(item => String.IsNullOrEmpty((item as IStorageItem)?.Id));
      if (invalidItem != null)
        throw new Exception($"The item {invalidItem} does not have an ID and cannot be stored.");
    }


    /// <summary>
    /// Delete items from the storage.
    /// </summary>
    /// <param name="items">The items to delete</param>
    /// <returns></returns>
    public Record<T> Delete(params T[] items)
    {
      AssertIdSet(items);
      DBManager.Instance.Storage.Delete(items);
      return this;
    }

    /// <summary>
    /// Delete all items from the storage.
    /// </summary>
    /// <returns></returns>
    public Record<T> DeleteAll()
    {
      return Delete(Read());
    }

    /// <summary>
    /// Updates items on the storage.
    /// </summary>
    /// <param name="items">The items to update</param>
    /// <returns></returns>
    public Record<T> Update(params T[] items)
    {
      AssertIdSet(items);
      DBManager.Instance.Storage.Store(items);
      return this;
    }

    /// <summary>
    /// Retrieve all items from the storage.
    /// </summary>
    /// <returns></returns>
    public T[] Read()
    {
      return FilterEntities(
        DBManager.Instance.Storage
        .List<T>()).ToArray();
    }

    /// <summary>
    /// Retrieve the first item in the filter.
    /// </summary>
    /// <param name="withError">If set, throw an error if no record is found.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T ReadFirst(bool withError = false)
    {
      var item = Read().FirstOrDefault();
      if (item == null && withError)
        throw new InvalidOperationException($"No record was found in collection {typeof(T)} for the given filters.");
      return item;
    }

    /// <summary>
    /// Explicitly read the single entity with this ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns></returns>
    public T Get(string id)
    {
      return Read().Cast<IStorageItem>().FirstOrDefault(a => a.Id == id) as T;
    }

    /// <summary>
    /// Write all modified collections to the storage to disk / persist it.
    /// </summary>
    /// <returns></returns>
    public Record<T> Save()
    {
      DBManager.Instance.Storage.Save();
      return this;
    }

  }

}