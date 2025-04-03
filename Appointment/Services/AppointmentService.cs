using AppointmentManagementAPI.DTOs;
using AppointmentManagementAPI.Repositories;
using AppointmentManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsAsync()
        {
            var appointments = await _repository.GetAppointmentsAsync();
            return appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                RequestorName = a.RequestorName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status
            });
        }

        public async Task<AppointmentDTO?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            if (appointment == null) return null;

            return new AppointmentDTO
            {
                Id = appointment.Id,
                RequestorName = appointment.RequestorName,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status
            };
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByNameAsync(string name)
        {
            var appointments = await _repository.GetAppointmentsByNameAsync(name);
            return appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                RequestorName = a.RequestorName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status
            });
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date)
        {
            var appointments = await _repository.GetAppointmentsByDateAsync(date);
            return appointments.Select(a => new AppointmentDTO
            {
                Id = a.Id,
                RequestorName = a.RequestorName,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status
            });
        }



        public async Task<int> AddAppointmentAsync(AppointmentDTO appointmentDto)
        {
            var appointment = new Appointment
            {
                RequestorName = appointmentDto.RequestorName,
                AppointmentDate = appointmentDto.AppointmentDate,
                Status = appointmentDto.Status ?? "Scheduled"
            };

            int appointmentId = await _repository.AddAppointmentAsync(appointment);
            return appointmentId;
        }

        public async Task UpdateAppointmentAsync(int id, AppointmentDTO appointmentDto)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                appointment.RequestorName = appointmentDto.RequestorName;
                appointment.AppointmentDate = appointmentDto.AppointmentDate;
                appointment.Status = appointmentDto.Status ?? appointment.Status; // Preserve status if not provided

                await _repository.UpdateAppointmentAsync(appointment);
            }
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _repository.DeleteAppointmentAsync(id);
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            return await _repository.CancelAppointmentAsync(id);
        }


        public async Task<bool> CompleteAppointmentAsync(int id)
        {
            return await _repository.CompleteAppointmentAsync(id);
        }



        public async Task<bool> CompleteAppointmentByNameAsync(string name)
        {
            return await _repository.CompleteAppointmentsByNameAsync(name);
        }



        public async Task<bool> RescheduleAppointmentAsync(int id, RescheduleRequestDTO requestDto)
        {
            var appointment = await _repository.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                appointment.AppointmentDate = requestDto.NewAppointmentDate;
                appointment.Status = "Rescheduled";  // Set status to "Rescheduled"
                await _repository.UpdateAppointmentAsync(appointment);
                return true;
            }
            return false;
        }

        public async Task<bool> RescheduleAppointmentByNameAsync(string name, RescheduleRequestDTO requestDto)
        {
            var appointments = await _repository.GetAppointmentsByNameAsync(name);
            bool updated = false;

            foreach (var appointment in appointments)
            {
                appointment.AppointmentDate = requestDto.NewAppointmentDate;
                appointment.Status = "Rescheduled";  // Set status to "Rescheduled"
                await _repository.UpdateAppointmentAsync(appointment);
                updated = true;
            }

            return updated;
        }
    }
}







