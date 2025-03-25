using AppointmentManagementAPI.DTOs;
using AppointmentManagementAPI.Models;
using AppointmentManagementAPI.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentManagementAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsAsync()
        {
            var appointments = await _repository.GetAppointmentsAsync();
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }
        public async Task<AppointmentDTO?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            return appointment == null ? null : _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByNameAsync(string name)
        {
            var appointments = await _repository.GetAppointmentsByNameAsync(name);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date)
        {
            var appointments = await _repository.GetAppointmentsByDateAsync(date);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task AddAppointmentAsync(AppointmentDTO appointmentDto)
        {
            var appointment = _mapper.Map<Appointment>(appointmentDto);
            await _repository.AddAppointmentAsync(appointment);
        }

        public async Task UpdateAppointmentAsync(int id, AppointmentDTO appointmentDto)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                _mapper.Map(appointmentDto, appointment);
                await _repository.UpdateAppointmentAsync(appointment);
            }
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                await _repository.UpdateAppointmentAsync(appointment);
                return true;
            }
            return false;
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            await _repository.DeleteAppointmentAsync(id);
        }
    }
}
