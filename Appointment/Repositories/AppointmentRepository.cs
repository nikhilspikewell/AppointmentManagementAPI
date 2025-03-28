//using AppointmentManagementAPI.Data;
//using AppointmentManagementAPI.Models;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AppointmentManagementAPI.Repositories
//{
//    public class AppointmentRepository : IAppointmentRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public AppointmentRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
//        {
//            return await _context.Appointments.ToListAsync();
//        }

//        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
//        {
//            return await _context.Appointments.FindAsync(id);
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByNameAsync(string name)
//        {
//            return await _context.Appointments.Where(a => a.RequestorName.Contains(name)).ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
//        {
//            return await _context.Appointments
//                .Where(a => a.AppointmentDate.Date == date.Date)
//                .ToListAsync();
//        }

//        public async Task AddAppointmentAsync(Appointment appointment)
//        {
//            await _context.Appointments.AddAsync(appointment);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAppointmentAsync(Appointment appointment)
//        {
//            _context.Appointments.Update(appointment);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAppointmentAsync(int id)
//        {
//            var appointment = await _context.Appointments.FindAsync(id);
//            if (appointment != null)
//            {
//                _context.Appointments.Remove(appointment);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}









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

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        // 🔹 Implement missing methods

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

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
