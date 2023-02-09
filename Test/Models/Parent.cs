using DBManager.Helpers;
using DBManager.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
  public class Parent : RecordBase
  {
    public string TestString { get; set; }
    public int TestInt { get; set; }
    public DateTime TestDateTime { get; set; }
    public myEnum TestEnum { get; set; }

    public List<Child> TestChild => RecordFactory.Open<Child>().SetRangeCs(o=> o.ParentId, Id).Read().ToList();


    public Parent(string id) : base(id)
    {
    }
  }

  public enum myEnum
  {
    None = 0,
    First = 1,
    Second = 2,
    Third = 3
  }
}
