using MagicVilla_API.Model.Dto;

namespace MagicVilla_API.Data
{
    public static class  VillaStore
    {
         public static List<VillaDTO> villaList = new List<VillaDTO>
            {
                new VillaDTO { Id = 1, Name = "PoolView",Sqft=100,Occupancy=4 },
                new VillaDTO { Id = 2, Name = "Beach View",Sqft=120, Occupancy=3 }
            };
    }
}
