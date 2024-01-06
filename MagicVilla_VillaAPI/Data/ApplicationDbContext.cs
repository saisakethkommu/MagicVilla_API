using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        // pass connection to db context as we will be using properties from db context mosst of timee
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
            
        }
        
        public DbSet<Villa> Villas { get; set; } // this  creates a table in the database when the migration is run

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                    Occupancy = 4,
                    Rate = 200,
                    Sqft = 550,
                    Amenity = "",
                    CreatedDate = DateTime.Now,
                },
                  new Villa
                  {
                      Id = 2,
                      Name = "Premium Pool Villa",
                      Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                      ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                      Occupancy = 4,
                      Rate = 300,
                      Sqft = 550,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  },
                  new Villa
                  {
                      Id = 3,
                      Name = "Luxury Pool Villa",
                      Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                      ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                      Occupancy = 4,
                      Rate = 400,
                      Sqft = 750,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  },
                  new Villa
                  {
                      Id = 4,
                      Name = "Diamond Villa",
                      Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                      ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                      Occupancy = 4,
                      Rate = 550,
                      Sqft = 900,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  },
                  new Villa
                  {
                      Id = 5,
                      Name = "Diamond Pool Villa",
                      Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                      ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                      Occupancy = 4,
                      Rate = 600,
                      Sqft = 1100,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  }
            );
        }
    }
}

// in entity framework we dont perform any operations within the database we do it through code
// which includes creating table or crud operations and all
// we need a db context class to manage all the entities in the application
// this  class should inherit db context
// when querying the databse we use linq statements.
// we should like the application db context to use the connection string in appsettings and register the dependency injection
// connection string should be passed to db context along with application db context
// these aare the basic steps to setup entity framework in any project
// we add db chnaages from the code iteself in entity framework