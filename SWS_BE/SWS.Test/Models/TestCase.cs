using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Models
{
    public class TestCase
    {
        public string Id { get; set; }
        public string SheetName { get; set; } = "";
        public int ColumnIndex { get; set; }
        public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();
        public TestExpected Expected { get; set; } = new TestExpected();
    }

}
