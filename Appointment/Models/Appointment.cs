using System;

namespace AppointmentManagementAPI.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string RequestorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
