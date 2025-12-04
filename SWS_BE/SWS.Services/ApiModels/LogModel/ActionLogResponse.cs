using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS.BusinessObjects.Enums;

namespace SWS.Services.ApiModels.LogModel
{
    public class ActionLogResponse
    {
        public int ActionId { get; set; }

        public int UserId { get; set; }

        public ActionType ActionType { get; set; }//Create,Update,Delete,...

        public string? EntityType { get; set; }//database table in which the data is changed

        public DateTime Timestamp { get; set; }

        public string? Description { get; set; }
    }
}
