﻿namespace MagicVilla_API.Model.Dto.VillaNumber
{
    public class VillaNumberUpdateDTO
    {
        public required int VillaNo { get; set; }
        public required int VillaID { get; set; }
        public string? SpecialDetails { get; set; }
    }
}