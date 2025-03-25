using AppointmentManagementAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentManagementAPI.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByNameAsync(string name);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int id);
    }
}
