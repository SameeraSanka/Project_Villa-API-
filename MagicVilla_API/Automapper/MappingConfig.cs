using AutoMapper;
using MagicVilla_API.Model;
using MagicVilla_API.Model.Dto;
using MagicVilla_API.Model.Dto.VillaNumber;

namespace MagicVilla_API.Automapper
{
    public class MappingConfig : Profile
    {
        //me auto mapper eka use krnna kalin packages dekak install krnna one ipasse meka program.cs eke configer krnna one. itpasse thama Controller eke use krnne
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>().ReverseMap();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
        }
    }
}
