namespace MeghanC_ShoppingCart.Migrations
{
    using MeghanC_ShoppingCart.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MeghanC_ShoppingCart.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if(!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if(!context.Users.Any(u => u.Email == "meghankcombs@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "meghankcombs@gmail.com",
                    Email = "meghankcombs@gmail.com",
                    FirstName = "Meghan",
                    LastName = "Combs",
                    DisplayName = "MCDev",
                    Created = DateTime.Now
                }, "Pa$$word");
            }

            var userId = userManager.FindByEmail("meghankcombs@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            context.Items.AddOrUpdate(
                i => i.Name,
                new Item { Created = DateTime.Now, Name = "Bridge", Description = "Contemporary artwork", Price = 500.00m, Artist = "Gregor van der Meer", MediaUrl = "~/Images/Uploads/Artist1Painting1.jpeg" },
                new Item { Created = DateTime.Now, Name = "Pink Geo", Description = "Contemporary artwork", Price = 1000.00m, Artist = "Gregor van der Meer", MediaUrl = "~/Images/Uploads/Artist1Painting2.jpeg" },
                new Item { Created = DateTime.Now, Name = "Yellow Geo", Description = "Contemporary artwork", Price = 1500.00m, Artist = "Gregor van der Meer", MediaUrl = "~/Images/Uploads/Artist1Painting3.jpeg" },
                new Item { Created = DateTime.Now, Name = "Red Geo", Description = "Contemporary artwork", Price = 2000.00m, Artist = "Gregor van der Meer", MediaUrl = "~/Images/Uploads/Artist1Painting4.jpeg" },
                new Item { Created = DateTime.Now, Name = "Laser Tag", Description = "Contemporary artwork", Price = 2500.00m, Artist = "Gregor van der Meer", MediaUrl = "~/Images/Uploads/Artist1Painting5.jpeg" },
                new Item { Created = DateTime.Now, Name = "Sky Hole", Description = "Contemporary artwork", Price = 3000.00m, Artist = "Gregor van der Meer", MediaUrl = "~/Images/Uploads/Artist1Painting6.jpeg" },

                new Item { Created = DateTime.Now, Name = "Eiffel Tower", Description = "Representative contemporary painting", Price = 1200.00m, Artist = "Olympia Hufferson", MediaUrl = "~/Images/Uploads/Artist2Painting1.jpeg" },
                new Item { Created = DateTime.Now, Name = "Buildings Relfective", Description = "Representative contemporary painting", Price = 2400.00m, Artist = "Olympia Hufferson", MediaUrl = "~/Images/Uploads/Artist2Painting2.jpeg" },
                new Item { Created = DateTime.Now, Name = "Boats", Description = "Representative contemporary painting", Price = 3600.00m, Artist = "Olympia Hufferson", MediaUrl = "~/Images/Uploads/Artist2Painting3.jpeg" },
                new Item { Created = DateTime.Now, Name = "Birds", Description = "Representative contemporary painting", Price = 4800.00m, Artist = "Olympia Hufferson", MediaUrl = "~/Images/Uploads/Artist2Painting4.jpeg" },
                new Item { Created = DateTime.Now, Name = "Still Life: Vase of Flowers", Description = "Representative contemporary painting", Price = 6000.00m, Artist = "Olympia Hufferson", MediaUrl = "~/Images/Uploads/Artist2Painting5.jpeg" },
                new Item { Created = DateTime.Now, Name = "Untitled", Description = "Random non-representative contemporary painting", Price = 7200.00m, Artist = "Olympia Hufferson", MediaUrl = "~/Images/Uploads/Artist2Painting6.jpeg" },

                new Item { Created = DateTime.Now, Name = "Autumn", Description = "Contemporary photograph", Price = 200.00m, Artist = "Synthia Wallard", MediaUrl = "~/Images/Uploads/Artist3Painting1.jpeg" },
                new Item { Created = DateTime.Now, Name = "Waterfall", Description = "Contemporary photograph", Price = 400.00m, Artist = "Synthia Wallard", MediaUrl = "~/Images/Uploads/Artist3Painting2.jpeg" },
                new Item { Created = DateTime.Now, Name = "Sneeze", Description = "Contemporary photograph", Price = 600.00m, Artist = "Synthia Wallard", MediaUrl = "~/Images/Uploads/Artist3Painting3.jpeg" },
                new Item { Created = DateTime.Now, Name = "Sneeze2", Description = "Contemporary photograph", Price = 800.00m, Artist = "Synthia Wallard", MediaUrl = "~/Images/Uploads/Artist3Painting4.jpeg" },
                new Item { Created = DateTime.Now, Name = "Girl Chasing Fireflies", Description = "Contemporary photograph", Price = 1000.00m, Artist = "Synthia Wallard", MediaUrl = "~/Images/Uploads/Artist3Painting5.jpeg" },
                new Item { Created = DateTime.Now, Name = "A Couple", Description = "Contemporary photograph", Price = 1100.00m, Artist = "Synthia Wallard", MediaUrl = "~/Images/Uploads/Artist3Painting6.jpeg" }
                );
        }
    }
}
