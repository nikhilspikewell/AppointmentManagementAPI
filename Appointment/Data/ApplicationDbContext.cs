using AppointmentManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Appointment> Appointments { get; set; }
    }
}