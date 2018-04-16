using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string MediaUrl { get; set; }
        public string Description { get; set; }
        public string Artist { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //Navigation Properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }

        public Item()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.ShoppingCarts = new HashSet<ShoppingCart>();
        }
    }
}