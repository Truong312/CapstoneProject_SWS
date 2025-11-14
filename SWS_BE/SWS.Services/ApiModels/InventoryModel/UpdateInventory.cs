using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWS.Services.ApiModels.InventoryModel
{
    public class UpdateInventory
    {
        //public int InventoryId { get; set; }

        public int ProductId { get; set; }

        public int QuantityAvailable { get; set; }

        public int AllocatedQuantity { get; set; }

        public int LocationId { get; set; }
    }
}
