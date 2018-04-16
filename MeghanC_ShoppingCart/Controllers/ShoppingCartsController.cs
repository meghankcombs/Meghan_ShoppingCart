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

namespace MeghanC_ShoppingCart.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCarts
        [Authorize] //have to keep because of line 23 below
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());//getting user from User table based on current logged in user
            var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();//get shopping cart based on that id
            if (shoppingCarts != null)
            {
                return View(shoppingCarts);//index view filled with shoppingCarts data
            }
            ViewBag.NoItem = "No item has been added";
            return View();//back to the index

            //var shoppingCarts = db.ShoppingCarts.Include(s => s.Customer).Include(s => s.Item);
            //return View(shoppingCarts.ToList()); //old code
        }

        public PartialViewResult CartIndex(string id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == id).ToList();
            return PartialView("~/Views/Shared/_CartIndex.cshtml", shoppingCarts);
        }

        //GET: Shared/_CartItem
        [Authorize]
        public PartialViewResult Cart()//returns view inside of another view = partial view --> example is Register and Login on page
                                       //can reuse in other views
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user != null)
            {
                var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id);

                decimal shopTotal = 0;
                int shopCount = 0;

                foreach (var count in shoppingCarts)
                {
                    shopCount += count.Count;
                    shopTotal += db.Items.Where(t => t.Id == count.ItemId).Sum(t => t.Price) * count.Count;
                }
                ViewBag.itemCart = shopCount;
                ViewBag.itemTotal = shopTotal;
            }
            return PartialView("~/Views/Shared/_CartItems.cshtml");//Partial View doesn't exist yet
        }

        // GET: ShoppingCarts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Create //calling POST Create from AccountController so don't need a GET Create
        //public ActionResult Create()
        //{
        //    ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName");
        //    ViewBag.ItemId = new SelectList(db.Items, "Id", "Name");
        //    return View();
        //}

        // POST: ShoppingCarts/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult Create(int Itemid)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    var exShopping = db.ShoppingCarts.Where(s => s.CustomerId == user.Id && s.ItemId == Itemid).ToList();
        //    if (exShopping.Count == 0) //existing shopping cart
        //    {
        //        ShoppingCart shoppingCart = new ShoppingCart();
        //        shoppingCart.CustomerId = user.Id;
        //        shoppingCart.ItemId = Itemid;
        //        shoppingCart.Item = db.Items.FirstOrDefault(i => i.Id == Itemid);
        //        shoppingCart.Count = 1;
        //        shoppingCart.Created = System.DateTime.Now;
        //        db.ShoppingCarts.Add(shoppingCart);
        //        db.SaveChanges();
        //        TempData["Message"] = "This piece has been added to your cart!";
        //        return RedirectToAction("Index", "Items", new { artist = shoppingCart.Item.Artist}); //going back to Artist Index action in Items Controller; where shopping cart is created
        //    }

        //    foreach (var items in exShopping)
        //    {
        //        items.Count++;
        //        db.Entry(items).Property("Count").IsModified = true; //allowing to modify only the count of items in the cart
        //    };

        //    db.SaveChanges();
        //    Item item = db.Items.Find(Itemid);
        //    TempData["Message"] = "This piece has been added to your cart!";
        //    return RedirectToAction("Index", "Items", new { artist = item.Artist }); //going back to Items action in Items Controller; where shopping cart is created
        //}

        //[HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(int Itemid)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var exShopping = db.ShoppingCarts.Where(s => s.CustomerId == user.Id && s.ItemId == Itemid).ToList();
            if (exShopping.Count == 0) //existing shopping cart
            {
                ShoppingCart shoppingCart = new ShoppingCart();
                shoppingCart.CustomerId = user.Id;
                shoppingCart.ItemId = Itemid;
                shoppingCart.Item = db.Items.FirstOrDefault(i => i.Id == Itemid);
                //shoppingCart.Count = count;
                shoppingCart.Count += 1;
                shoppingCart.Created = System.DateTime.Now;
                db.ShoppingCarts.Add(shoppingCart);
                db.SaveChanges();
                TempData["Message"] = "This piece has been added to your cart!";
                return RedirectToAction("Index", "Items", new { artist = shoppingCart.Item.Artist }); //going back to Artist Index action in Items Controller; where shopping cart is created
            }

            foreach (var items in exShopping)
            {
                //items.Count += count;
                items.Count++;
                db.Entry(items).Property("Count").IsModified = true; //allowing to modify only the count of items in the cart
            }

            db.SaveChanges();
            Item item = db.Items.Find(Itemid);
            TempData["Message"] = "This piece has been added to your cart!";
            return RedirectToAction("Index", "Items", new { artist = item.Artist }); //going back to Index action in Items Controller; where shopping cart is created
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Refresh(int id, int count)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var item = db.ShoppingCarts.FirstOrDefault(s => s.CustomerId == user.Id && s.ItemId == id);
            if (count >= 1 && item != null)
            {
                item.Count = count;
                db.Entry(item).Property("Count").IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index", "ShoppingCarts");
            }
            else
            {
                return RedirectToAction("Delete", "ShoppingCarts");
            }
        }

        //OLD CODE ([Bind(Include = "Id,ItemId,Count,Created,CustomerId")] ShoppingCart shoppingCart)
        //{
        //if (ModelState.IsValid)
        //{
        //    db.ShoppingCarts.Add(shoppingCart);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName", shoppingCart.CustomerId);
        //ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", shoppingCart.ItemId);
        //return View(shoppingCart);
        //}

        // GET: ShoppingCarts/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName", shoppingCart.CustomerId);
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", shoppingCart.ItemId);
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemId,Count,Created")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shoppingCart).State = EntityState.Modified;//everything can be modified (all columns in table)
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CustomerId = new SelectList(db.Users, "Id", "FirstName", shoppingCart.CustomerId);
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", shoppingCart.ItemId);//can select items by name; 4th parameter will already be selected
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
        //    if (shoppingCart == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(shoppingCart);
        //}

        // POST: ShoppingCarts/Delete/5
        [/*HttpPost, */ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            db.ShoppingCarts.Remove(shoppingCart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var shoppingCarts = db.ShoppingCarts.Where(s => s.CustomerId == user.Id).ToList();
            if (shoppingCarts != null)
            {
                foreach (var del in shoppingCarts)
                {
                    db.ShoppingCarts.Remove(del);
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index"/*, "ShoppingCarts"*/);//already in ShoppingCarts controller so don't need this extra line
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
