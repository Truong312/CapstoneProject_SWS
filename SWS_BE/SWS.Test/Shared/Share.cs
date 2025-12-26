using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Shared
{
    public class Share
    {
        public int? GetNullableInt(Dictionary<string, string> inputs, string key)
        {
            if (!inputs.ContainsKey(key)) return null;

            var value = inputs[key];

            if (string.IsNullOrEmpty(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;

            return int.Parse(value);
        }

    }
}
