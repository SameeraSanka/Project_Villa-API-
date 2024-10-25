using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Model.Dto
{
    public class VillaDTO
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public required  string Name { get; set; }

        public int Occupancy { get; set; }

        public int Sqft { get; set; }
    }
}
