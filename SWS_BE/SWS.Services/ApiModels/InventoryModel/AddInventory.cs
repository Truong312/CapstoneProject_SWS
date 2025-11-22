using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Models;

namespace SWS.Services.ApiModels.InventoryModel
{
    public class AddInventory
    {
        //public int InventoryId { get; set; }

        public int ProductId { get; set; }

        public int QuantityAvailable { get; set; }

        public int AllocatedQuantity { get; set; }

        public int LocationId { get; set; }
    }
}
