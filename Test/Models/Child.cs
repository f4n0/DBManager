using DBManager.Helpers;
using DBManager.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    public class Child : RecordBase
    {
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public int Data3 { get; set; }

        public string ParentId { get; set; }


        public Child(string id) : base(id)
        {
        }
    }
}
