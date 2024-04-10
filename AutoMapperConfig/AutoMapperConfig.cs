using AutoMapper;
using Inventario.Models;
using Inventario.DTOs;

namespace Inventario.AutoMapperConfig
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            // Departamento
            CreateMap<Departamento, DepartamentoDTO>();
            CreateMap<DepartamentoDTO, Departamento>();

            // Dispositivos
            CreateMap<Dispositivo, DispositivoDTO>();
            CreateMap<DispositivoDTO, Dispositivo>();
            CreateMap<DispositivoCreateDTO, Dispositivo>();
            CreateMap<DispositivoDTO, DispositivoCreateDTO>();
            CreateMap<DispositivoCreateDTO, DispositivoDTO>();
            CreateMap<DispositivoImportDTO, DispositivoDTO>();
            CreateMap<DispositivoDTO, DispositivoImportDTO>();

            // Computadoras
            CreateMap<PC, PCDTO>();
            CreateMap<PCDTO, PcCreateDTO>();
            CreateMap<PCDTO, PC>();
            CreateMap<PcCreateDTO, PCDTO>();
            CreateMap<PcCreateDTO, PC>();

            // Usuarios
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();
            CreateMap<UserCreateDTO, UserDTO>();
            CreateMap<UserDTO, UserCreateDTO>();
            CreateMap<UserCreateDTO, User>();

        }
    }
}
