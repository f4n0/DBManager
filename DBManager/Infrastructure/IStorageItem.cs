using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Infrastructure
{
  /// <summary>
  /// An item that is store-able by a <see cref="IStorage"/> implementation.
  /// </summary>
  public interface IStorageItem 
  {

    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    [SQLite.PrimaryKey]
    string Id { get; }

  }
}
