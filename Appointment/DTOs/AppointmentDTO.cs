using System;

namespace AppointmentManagementAPI.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public string RequestorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Scheduled";
    }
}
