using AutoMapper;
using Inventario.Models;
using Inventario.DTOs;

namespace Inventario.AutoMapperConfig
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            CreateMap<Departamento, DepartamentoDTO>();
            CreateMap<DepartamentoDTO, Departamento>();
            CreateMap<Dispositivo, DispositivoDTO>();
            // .ForMember(dest => dest.Nombre_departamento, opt => opt.MapFrom(src => src.departamento.Nombre));
            // CreateMap<Dispositivo, DispositivoDTO>();
                // .ForMember(dest => dest.departamento, opt => opt.Ignore())
                // .ForMember(dest => dest.Computer, opt => opt.Ignore());
            CreateMap<DispositivoDTO, Dispositivo>();

            CreateMap<PC, PCDTO>();
            CreateMap<PCDTO, PC>();
            CreateMap<UserDTO, User>();
        }
    }
}
