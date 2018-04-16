using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Models
{
    public class Order
    {
        //Properties
        public int Id { get; set; }
        public string ShipToFirstName { get; set; }
        public string ShipToLastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; } //what ApplicationUser property below is named after
        public decimal Total { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public bool Completed { get; set; }

        //Navigation Properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } //ICollection = parent to Order Detail model
        public virtual ApplicationUser Customer { get; set; } //child of ApplicationUser : IdentityModels model

        public Order()//initializing ICollection<Order_Detail> to new HashSet data structure
        {
            this.OrderDetails = new HashSet<OrderDetail>(); //HashSet eliminates duplicates and allows to access data efficiently
        }
    }
}