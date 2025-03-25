using AppointmentManagementAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentManagementAPI.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsAsync();
        Task<AppointmentDTO?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByNameAsync(string name);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date);
        Task AddAppointmentAsync(AppointmentDTO appointmentDto);
        Task UpdateAppointmentAsync(int id, AppointmentDTO appointmentDto);
        Task<bool> CancelAppointmentAsync(int id);
        Task DeleteAppointmentAsync(int id);
    }
}
