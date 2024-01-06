using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaCreateDTO
    {
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
        public double Sqft { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public string? Amenity { get; set; }
        public string? Details { get; set; }
        [Required]
        public double Rate { get; set; }
    }
}

// there are built in data annotatiosn which helps in add validation around a model state
// now if the name is empty it givees an error

// separate the DTOs for specfic operations probably can be avoided sometimes with annotations