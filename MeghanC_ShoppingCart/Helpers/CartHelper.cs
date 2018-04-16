using MeghanC_ShoppingCart.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Helpers
{
    public class CartHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static int CartCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myCount = 0;
            var myCarts = db.ShoppingCarts.AsNoTracking().Where(c => c.CustomerId == userId).ToList();
            if (myCarts.Count > 0)
            {
                myCount = myCarts.Sum(s => s.Count);
            }
            return myCount;
        }

        public static decimal CalcSubTotal()
        {
            var subTotal = 0.00m;
            var userId = HttpContext.Current.User.Identity.GetUserId(); //getting logged in user outside the Controller using HttpContext
            var myCarts = db.ShoppingCarts.Where(sc => sc.CustomerId == userId).ToList();//shopping carts associated with particular 
                                                                                         //customer id that matches user id
            foreach (var cart in myCarts)
            {
                subTotal += cart.Item.Price * cart.Count;
            }

            return subTotal;
        }

        public static decimal CalcTotal()
        {
            var total = 0.00m;
            total += CalcSubTotal() + Shipping() + Tax();
            return decimal.Round(total, 2);
            //return decimal.Round(CalcSubTotal() + Shipping() + Tax());
        }

        public static decimal Shipping()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId(); //getting logged in user outside the Controller using HttpContext
            var cartCount = db.ShoppingCarts.Where(sc => sc.CustomerId == userId).Count();
            var shipping = 0.00m;
            for (var loop = 0; loop < cartCount; loop++)
            {
                shipping += 10.00m;
            }
            return shipping;
        }

        public static decimal Tax()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId(); //getting logged in user outside the Controller using HttpContext
            var myCarts = db.ShoppingCarts.Where(sc => sc.CustomerId == userId).ToList();
            var tax = 0.00m;
            foreach (var cart in myCarts)
            {
                tax += decimal.Round(0.15m * cart.Item.Price * cart.Count);
            }
            return tax;
        }
    }
}