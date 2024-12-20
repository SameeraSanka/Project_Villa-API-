﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Model.Dto
{
    public class VillaUpdateDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string? Details { get; set; }
        public required double Rate { get; set; }
        public required int Sqft { get; set; }
        public required int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public required string? Amenity { get; set; }

    }
}
