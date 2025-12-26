using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Test.Helpers
{
    public static class CellHelper
    {
        public static string Get(this IList<IList<object>> sheet, int row, int col)
        {
            if (row >= sheet.Count) return string.Empty;
            if (col >= sheet[row].Count) return string.Empty;
            return sheet[row][col]?.ToString()?.Trim() ?? string.Empty;
        }
    }
}
