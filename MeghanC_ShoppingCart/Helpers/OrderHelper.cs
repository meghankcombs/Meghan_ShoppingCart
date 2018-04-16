using MeghanC_ShoppingCart.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Helpers
{
    public class OrderHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static int OrderHistoryCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var count = 0;
            if (userId != null)
            {
                count = db.Orders.Where(o => o.CustomerId == userId).Count();
            }
            return count;
        }

        public static decimal GetOrderSubTotal(int orderId)
        {
            //Get a single value back from the Orders table  (i.e. Total or  Sub-Total
            var subTotal = db.Orders.FirstOrDefault(o => o.Id == orderId).Total;
            return subTotal;
        }

        public static decimal GetOrderTax(int orderId)
        {
            //Get a single value back from the Orders table  (i.e. Total or  Sub-Total
            var tax = db.Orders.FirstOrDefault(o => o.Id == orderId).Tax;
            return tax;
        }

        public static decimal GetOrderShipping(int orderId)
        {
            //Get a single value back from the Orders table  (i.e. Total or  Sub-Total
            var shipping = db.Orders.FirstOrDefault(o => o.Id == orderId).Shipping;
            return shipping;
        }

        public static decimal GetOrderTotal(int orderId)
        {
            //Get a single value back from the Orders table  (i.e. Total or  Sub-Total
            var total = GetOrderSubTotal(orderId) + GetOrderTax(orderId) + GetOrderShipping(orderId);
            return total;
        }
    }
}