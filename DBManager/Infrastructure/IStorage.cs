using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Infrastructure
{
  public interface IStorage
  {

    void Delete<T>(T[] items) where T : class;

    void Store<T>(T[] items) where T : class;

    IEnumerable<T> List<T>(List<Expression<Func<T, bool>>> Filters = null) where T : class;

    void Save();


  }
}
