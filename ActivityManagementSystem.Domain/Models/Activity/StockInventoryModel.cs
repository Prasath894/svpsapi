using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StockInventoryModel
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public string InventoryDescription { get; set; }
        public string InventoryName { get; set; }
        public string Specification { get; set; }
        public DateTime StockDate { get; set; }
        public string Store { get; set; }
        
        public string AvailableQuantity { get; set; }
       /* public string PurchasedBy { get; set; }
        public string RequestedBy { get; set; }
        public string IndentQuantity { get; set; }
        public string PrintedQuantity { get; set; }
        public string ReceivedQuantity { get; set; }
        public string VendarDetails { get; set; }
        public string UnitPrize { get; set; }
        public string TotalPrize { get; set; }
        public string Discount { get; set; }*/
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
