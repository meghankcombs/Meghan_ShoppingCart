using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeghanC_ShoppingCart.Models;
using Microsoft.AspNet.Identity;
using MeghanC_ShoppingCart.Helpers;

namespace MeghanC_ShoppingCart.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext(); //private level class of type ApplicationDbCntext 
                                                                      //called db which instantiates ApplicationDbContext

        //TODO: No authorization --> means anyone can shop? Or do we want the user to have to register

        //GET: Orders
        public ActionResult Index(int? id)
        {
            var userId = User.Identity.GetUserId();
            var orders = db.Orders.Include(o => o.Customer); //"eager load" (vs "lazy load") - includes related info, customer info here
                                                             //can only include Navigation property of Order Model inside this lambda expression
            var orderHistory = new List<Order>();
            if (userId == null)
            {
                //orderHistory = db.Orders.ToList();
                TempData["Message"] = "You must be logged in to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                orderHistory = db.Orders.Where(o => o.CustomerId == userId).ToList();
            }
            return View(orderHistory);
            //return View(orders.ToList());
        }

        //GET: Orders
        public ActionResult Complete(int? id)
        {
            if (id != null)
            {
                var userId = User.Identity.GetUserId(); //grabs id that is logged in and assigns it to userid var
                var completeOrder = db.Orders.Find(id); //assigns id of order found in Orders table and assigns it to completeOrder
                var orderDetail = completeOrder.OrderDetails.ToList();//using Navigation property of OrderDetails to get a 
                                                                      //reference to the id inside completeOrder var and access all it's specific order details
                var shoppingCarts = db.ShoppingCarts.Where(sc => sc.CustomerId == userId); //locating record in ShoppingCarts 
                                                                                           //table in db whose CustomerId property equals userId - pulling up shopping cart of logged in user
                                                                                           //db.Orders.Remove(completeOrder); //removing the order from Orders table - what if you want order history?
                if (shoppingCarts != null)//after order is made, remove items from shopping cart
                {
                    foreach (var shopping in shoppingCarts)//loop through each ShoppingCart record in ShoppingCarts list
                    {
                        db.ShoppingCarts.Remove(shopping);//and remove the cart
                    }
                }
                db.SaveChanges();//save the removal of the shopping cart to the db
            }
            return View();//executing code regardless of if statement because no other return statement
            //TODO: might be better to return to a receipt page instead - put in "Order Confirm" as an argument
        }

        //GET: OrderConfirm
        public ActionResult OrderConfirm()
        {
            return View();
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            if (order.CustomerId != User.Identity.GetUserId())
            {
                TempData["Message"] = "You cannot access this page.";
                return RedirectToAction("Index", "Orders");
            }
            else if (User.IsInRole("Admin"))
            {
                return View(order);
            }
            return View(order);
        }

        // GET: Orders/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //POST: Orders Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShipToFirstName,ShipToLastName,Address,City,State,ZipCode,Country,Phone")] Order order)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var shoppingcart = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            //decimal totalAmt = 0;
            if (shoppingcart.Count != 0)
            {
                if (ModelState.IsValid)
                {
                    foreach (var product in shoppingcart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.ItemId = product.ItemId;
                        orderDetail.OrderId = order.Id;
                        orderDetail.Quantity += product.Count;
                        orderDetail.UnitPrice = product.Item.Price;
                        //totalAmt += (product.Count * product.Item.Price);
                        db.OrderDetails.Add(orderDetail);
                    }

                    order.Total = CartHelper.CalcSubTotal();
                    order.Tax = CartHelper.Tax();
                    order.Shipping = CartHelper.Shipping();
                    order.Completed = false; //should this be false or true??
                    //delete items from cart after order completes somehow?
                    order.OrderDate = DateTime.Now;
                    order.CustomerId = user.Id;
                    db.Orders.Add(order);
                    db.SaveChanges();

                    this.Complete(order.Id);

                    var myOrder = db.Orders.Where(o => o.Id == order.Id).Include("Customer").Single();
                    return View("Receipt", myOrder);
                }

            }
            ViewBag.NoItem = "There are no items to order";
            return View(order);
        }

        #region Old Create, Review, Finalize code
        // POST: Orders/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ShipToFirstName,ShipToLastName,Address,City,State,ZipCode,Country,Phone")] Order order) //form values
        //{
        //    ReviewViewModel model = new ReviewViewModel();
        //    var user = db.Users.Find(User.Identity.GetUserId());//Find record in Users table in db with id of logged in user
        //    var shoppingcart = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();//grab records in ShoppingCarts table in db 
        //                                                                              //where CustomerId equals userId and turn into list
        //    decimal totalAmt = 0;//var set to 0 to start a cycle later on
        //    if (shoppingcart.Count != 0)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            foreach (var product in shoppingcart)//each record in shopping cart...
        //            {
        //                OrderDetail orderDetail = new OrderDetail();//new instance of OrderDetail class
        //                orderDetail.ItemId = product.ItemId;//setting properties of orderDetail below...
        //                orderDetail.Item = db.Items.Find(orderDetail.ItemId);
        //                orderDetail.OrderId = order.Id;
        //                orderDetail.Quantity += product.Count;
        //                orderDetail.UnitPrice = product.Item.Price;//shopping cart record, navigate to item, grab price property from that item
        //                totalAmt += (product.Count * product.Item.Price);//adds price to totalAmt
        //                //order.OrderDetails.Add(orderDetail);
        //                db.OrderDetails.Add(orderDetail);//added to OrderDetails db set
        //            }
        //            model.details = order.OrderDetails;
        //            order.Total = totalAmt;
        //            order.Shipping = CartHelper.Shipping();
        //            order.Tax = CartHelper.Tax();
        //            order.Completed = false;
        //            order.OrderDate = DateTime.Now;
        //            order.CustomerId = user.Id;//user record id property
        //            model.order = order;
        //            //db.Orders.Add(order);//added to Orders db set
        //            //db.SaveChanges();//saves it to db
        //            return RedirectToAction("Review", model);
        //        }

        //    }
        //    //ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName", order.CustomerId);
        //    //can reselect customer - not going to use, biproduct of scaffolding process
        //    ViewBag.NoItem = "There are no items to order";
        //    return View(order);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Review([Bind(Include = "ShipToFirstName,ShipToLastName,Address,City,State,ZipCode,Country,Phone")] Order order)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var orderReview = new OrderReview();
        //    orderReview.Address.Address = order.Address;
        //    orderReview.Address.City = order.City;
        //    orderReview.Address.State = order.State;
        //    orderReview.Address.ZipCode = order.ZipCode;
        //    orderReview.Address.Country = order.Country;
        //    orderReview.Address.Phone = order.Phone;
        //    orderReview.Address.ShipToFirstName = order.ShipToFirstName;
        //    orderReview.Address.ShipToLastName = order.ShipToLastName;


        //    orderReview.Carts = db.ShoppingCarts.Where(s => s.CustomerId == userId).ToList();

        ////var userId = User.Identity.GetUserId();
        //// var myCarts = db.ShoppingCarts.Where(s => s.CustomerId == userId).ToList();
        //ReviewViewModel model = new ReviewViewModel();
        //var user = db.Users.Find(User.Identity.GetUserId());//Find record in Users table in db with id of logged in user
        //var shoppingcart = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();//grab records in ShoppingCarts table in db 
        //                                                                                 //where CustomerId equals userId and turn into list
        //decimal totalAmt = 0;//var set to 0 to start a cycle later on
        //if (shoppingcart.Count != 0)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var product in shoppingcart)//each record in shopping cart...
        //        {
        //            OrderDetail orderDetail = new OrderDetail();//new instance of OrderDetail class
        //            orderDetail.ItemId = product.ItemId;//setting properties of orderDetail below...
        //            orderDetail.Item = db.Items.Find(orderDetail.ItemId);
        //            orderDetail.OrderId = order.Id;
        //            orderDetail.Quantity += product.Count;
        //            orderDetail.UnitPrice = product.Item.Price;//shopping cart record, navigate to item, grab price property from that item
        //            totalAmt += (product.Count * product.Item.Price);//adds price to totalAmt
        //            order.OrderDetails.Add(orderDetail);
        //            //db.OrderDetails.Add(orderDetail);//added to OrderDetails db set
        //        }
        //        model.details = order.OrderDetails;
        //        ViewBag.details = order.OrderDetails;
        //        order.Total = totalAmt;
        //        order.Shipping = CartHelper.Shipping();
        //        order.Tax = CartHelper.Tax();
        //        order.Completed = false;
        //        order.OrderDate = DateTime.Now;
        //        order.CustomerId = user.Id;//user record id property
        //        model.order = order;
        //        //db.Orders.Add(order);//added to Orders db set
        //        //db.SaveChanges();//saves it to db
        //        return View(model);
        //    }

        //}
        ////ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName", order.CustomerId);
        ////can reselect customer - not going to use, biproduct of scaffolding process
        //ViewBag.NoItem = "There are no items to order";
        //    return View(orderReview);
        //}


        //[HttpPost]
        //public ActionResult Finalize(CustomerAddress address)
        //{
        //    var userId = User.Identity.GetUserId();//Find record in Users table in db with id of logged in user
        //    var shoppingcarts = db.ShoppingCarts.Where(s => s.CustomerId == userId).ToList();//grab records in ShoppingCarts table in db 

        //    var order = new Order();
        //    order.Shipping = CartHelper.Shipping();
        //    order.Tax = CartHelper.Tax();
        //    order.Completed = false;
        //    order.OrderDate = DateTime.Now;
        //    order.CustomerId = userId;//user record id property

        //    //where CustomerId equals userId and turn into list
        //    decimal totalAmt = 0;//var set to 0 to start a cycle later on

        //    foreach(var cart in shoppingcarts)
        //    {
        //        totalAmt += (cart.Count * cart.Item.Price);//adds price to totalAmt
        //    }

        //    order.Total = totalAmt;
        //    db.Orders.Add(order);

        //    if (shoppingcarts.Count != 0)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            foreach (var product in shoppingcarts)//each record in shopping cart...
        //            {
        //                OrderDetail orderDetail = new OrderDetail();//new instance of OrderDetail class
        //                orderDetail.ItemId = product.ItemId;//setting properties of orderDetail below...     
        //                orderDetail.Quantity += product.Count;
        //                orderDetail.UnitPrice = product.Item.Price;//shopping cart record, navigate to item, grab price property from that item                
        //                db.OrderDetails.Add(orderDetail);                     
        //            }


        //            foreach (var cart in shoppingcarts)
        //            {
        //                db.ShoppingCarts.Remove(cart);
        //            }

        //            db.SaveChanges();
        //            return RedirectToAction("Receipt", new { id = order.Id});
        //        }

        //    }
        //    //ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName", order.CustomerId);
        //    //can reselect customer - not going to use, biproduct of scaffolding process
        //    ViewBag.NoItem = "There are no items to order";
        //    return View(order);
        //}
        #endregion

        public ActionResult Receipt(int? id)
        {
            if (id != null)
            {
                var order = db.Orders.Find(id);
                return View(order);
            }
            return RedirectToAction("Index", "Items", null);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Address,City,State,ZipCode,Country,Phone")] Order order)
        //took out other arguments because this is the user editing right?
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
