using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Helpers
{
    public static class TestDebug
    {
        public static void Log(string msg)
        {
            TestContext.WriteLine($"[DEBUG] {msg}");
        }
    }

}
