using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public DateTime Created { get; set; }
        public string CustomerId { get; set; }

        //Navigation Properites
        public virtual Item Item { get; set; } //child of Item model
        public virtual ApplicationUser Customer { get; set; } //child of ApplicationUser : IdentityModels model
    }
}