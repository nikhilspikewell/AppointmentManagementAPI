using AppointmentManagementAPI.DTOs;
using AppointmentManagementAPI.Models;
using AutoMapper;

namespace AppointmentManagementAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Appointment, AppointmentDTO>().ReverseMap();
        }
    }
}
