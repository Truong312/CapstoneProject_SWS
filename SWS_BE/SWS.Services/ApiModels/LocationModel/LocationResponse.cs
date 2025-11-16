using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.ApiModels.LocationModel
{
    public class LocationResponse
    {
        public int LocationId { get; set; }

        public string? ShelfId { get; set; }

        public int? ColumnNumber { get; set; }

        public int? RowNumber { get; set; }

        public string? Type { get; set; }

        public bool? IsFull { get; set; }
    }
}
