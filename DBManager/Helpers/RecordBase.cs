using DBManager.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DBManager.Helpers
{
  public class RecordBase : IStorageItem, IEquatable<RecordBase>
  {

    /// <summary>
    /// The storage-specific ID.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="id">The storage-specific ID of the model.</param>
    protected RecordBase(string id)
    {
      Id = id;
    }


    /// <summary>
    /// Private constructor for Activator.CreateInstance
    /// </summary>
    protected RecordBase()
    {

    }


    /// <summary>
    /// Writes the model to the storage, updating or (if not yet existing) inserts it.
    /// </summary>
    public void Store()
    {
      RecordFactory.CreateGeneric().Update(this).Save();
    }

    /// <summary>
    /// Deletes the model from the storage.
    /// </summary>
    public void Delete()
    {
      RecordFactory.CreateGeneric().Delete(this).Save();
    }


    /// <inheritdoc />
    public bool Equals(RecordBase other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Id == other.Id;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((RecordBase)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return (Id != null ? Id.GetHashCode() : 0);
    }

  }
}
