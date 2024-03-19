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
            CreateMap<DispositivoDTO, Dispositivo>();
            CreateMap<DispositivoCreateDTO, Dispositivo>();
            CreateMap<DispositivoDTO, DispositivoCreateDTO>();
            CreateMap<DispositivoCreateDTO, DispositivoDTO>();

            CreateMap<PC, PCDTO>();
            CreateMap<PCDTO, PcCreateDTO>();
            CreateMap<PCDTO, PC>();
            CreateMap<PcCreateDTO, PCDTO>();
            CreateMap<PcCreateDTO, PC>();
            
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();
        }
    }
}
