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
        

        Task<int> AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment); //  Fixed method signature
        Task<IEnumerable<Appointment>> GetAppointmentsByNameAsync(string name);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<bool> DeleteAppointmentAsync(int id);

        //  Newly added methods
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id);
        Task<bool> CompleteAppointmentsByNameAsync(string name);
        Task<bool> RescheduleAppointmentAsync(int id, DateTime newDate);
        Task<bool> RescheduleAppointmentsByNameAsync(string name, DateTime newDate);
    }
}
