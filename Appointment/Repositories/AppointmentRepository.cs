using AppointmentManagementAPI.Data;
using AppointmentManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentManagementAPI.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }

   

        public async Task<int> AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment.Id; // Return the generated ID
        }



        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByNameAsync(string name)
        {
            return await _context.Appointments
                .Where(a => a.RequestorName.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //  Newly added methods
        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }




        public async Task<bool> CompleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return false; // Indicating no appointment found
            }

            if (appointment.Status == "Cancelled")
            {
                return false; // Indicating appointment is cancelled
            }

            appointment.Status = "Completed";
            await _context.SaveChangesAsync();
            return true; // Successfully completed
        }






        public async Task<bool> CompleteAppointmentsByNameAsync(string name)
        {
            var appointments = await _context.Appointments
                .Where(a => a.RequestorName == name)
                .ToListAsync();

            if (!appointments.Any())
            {
                return false; // No appointments found
            }

            if (appointments.All(a => a.Status == "Cancelled"))
            {
                return false; // All appointments are cancelled
            }

            foreach (var appointment in appointments.Where(a => a.Status != "Cancelled"))
            {
                appointment.Status = "Completed";
            }

            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<bool> RescheduleAppointmentAsync(int id, DateTime newDate)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.AppointmentDate = newDate;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RescheduleAppointmentsByNameAsync(string name, DateTime newDate)
        {
            var appointments = await _context.Appointments.Where(a => a.RequestorName == name).ToListAsync();
            if (appointments.Any())
            {
                foreach (var appointment in appointments)
                {
                    appointment.AppointmentDate = newDate;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
