using System;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementAPI.DTOs
{
    public class RescheduleRequestDTO
    {
        [Required]
        public string RequestorName { get; set; } = string.Empty;

        [Required]
        public DateTime NewAppointmentDate { get; set; }
    }
}
