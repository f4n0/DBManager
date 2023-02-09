using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Infrastructure
{
  public interface IStorage
  {

    void Delete<T>(T[] items) where T : class;

    void Store<T>(T[] items) where T : class;

    IEnumerable<T> List<T>() where T : class;

    void Save();


  }
}
