using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeghanC_ShoppingCart.Models;
using System.IO;
using MeghanC_ShoppingCart.Helpers;
using Microsoft.AspNet.Identity;

namespace MeghanC_ShoppingCart.Controllers
{
    public class ItemsController : Controller
    {
        //class properties; private means can only be used inside this class
        private ApplicationDbContext db = new ApplicationDbContext(); //creating new instance to communicate with db
        private ImageUploadValidator validator = new ImageUploadValidator();

        //Controller code to render the Partial View
        //This gets called like any other HttpGet from the view -- @Html.Action("action", "controller", parameters)
        //[ChildActionOnly]
        //[Authorize]
        public PartialViewResult _ArtistIndex(string name)
        {
            var userId = User.Identity.GetUserId();
            var myItems = db.Items.Where(s => s.Artist == name).ToList();
            return PartialView("~/Views/Shared/_ArtistIndex.cshtml", myItems);
        }


        // GET: Items
        //[Authorize] //only restricts users if they aren't a registered user; "data annotation"
        public ActionResult Index(string artist) //shows Items Index; default action when nothing is called
        {
            if (string.IsNullOrEmpty(artist))
            {
                return View(db.Items.ToList());//LINQ statement; calling Index() View inside Items view folder
            }
            else
            {
                ViewBag.Artist = artist;
                return View(db.Items.Where(i => i.Artist == artist).ToList());
            }
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id) //int? makes number nullable
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id); //creating variable of type Item and assigning it to the id found in Items table in db
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        // don't need [HttpGet] becase it's assumed
        [Authorize(Roles = "Admin")] //who is authorized to create new items
        public ActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        [HttpPost] //turns into POST; need this because not assumed
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Price,MediaUrl,Description,Artist")] Item item,
            HttpPostedFileBase Image)
        {
            if (ModelState.IsValid) //checks if properties of item are valid & can be stored back into the db later on
            {
                if (ImageUploadValidator.IsWebFriendlyImage(Image))
                {
                    var fileName = Path.GetFileName(Image.FileName); //strips out long path info to just get the file name to store in var
                    Image.SaveAs(Path.Combine(Server.MapPath("~/Images/Uploads/"), fileName)); //putting the fileName 
                    //var name into the Images/Uploads folder and therefore changes its path to this folder's name
                    item.MediaUrl = "~/Images/Uploads/" + fileName; //creating the URL's name - pointing to where item exists
                }
                item.Created = DateTime.Now; //sets created date to current date/time
                db.Items.Add(item); //adds item to db set "Items"
                db.SaveChanges(); //then saves it to SQL db here <-- need this line to save to db
                return RedirectToAction("Index"); //after saving, goes back to ActionResult Index in Items Controller 
                                                  //to show new item in list
            }

            return View(item); //shows if ModelState is not valid -- takes user back to Create View with all their data 
                               //still typed in (which is why we pass in item argument)
        }

        // GET: Items/Edit/5
        [Authorize(Roles = "Admin")] //who is authorized to edit new items
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Price,MediaUrl,Description,Artist")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.Updated = DateTime.Now;
                db.Entry(item).State = EntityState.Modified; //changes already existing record in db
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: Items/Delete/5
        [Authorize(Roles = "Admin")] //who is authorized to delete new items
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")] //referring to action as "Delete" not "DeleteConfirmed" --> i.e. nickname
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) //happens automatically; reclaim resources; garbage collection (gc)
                                                        //managed by frameworks, does whenever, don't need to worry about it
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
