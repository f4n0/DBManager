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

    private List<Expression<Func<T,bool>>> Filters { get; }


    /// <summary>
    /// </summary>
    public Record()
    {
      Properties = new Dictionary<string, PropertyInfo>();
      Filters = new();
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
    public Record<T> SetRange(Expression<Func<T, bool>> expression)
    {
      Filters.Add(expression);
      return this;
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
       return DBManager.Instance.Storage.List<T>(Filters).ToArray();
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