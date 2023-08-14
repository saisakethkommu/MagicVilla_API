using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaDTO
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
        public double Sqft { get; set; }
        public int Occupancy { get; set; }
    }
}

// there are built in data annotatiosn which helps in add validation around a model state
// now if the name is empty it givees an error