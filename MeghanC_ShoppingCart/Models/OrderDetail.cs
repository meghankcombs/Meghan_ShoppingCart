using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; } //two foreign keys = two parents
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        //Navigation Properties
        public virtual Order Order { get; set; } //child of Order model
        public virtual Item Item { get; set; } //child of Item model
    }
}