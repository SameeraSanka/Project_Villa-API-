using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Model.Dto.VillaNumber
{
    public class VillaNumberDTO
    {
        public required int VillaNo { get; set; }
        public required int VillaID { get; set; }
        public string? SpecialDetails { get; set; }
    }
}
