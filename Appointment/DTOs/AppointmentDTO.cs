﻿using System;
using System.ComponentModel.DataAnnotations;
namespace AppointmentManagementAPI.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        [Required]
        public string RequestorName { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; } = "Scheduled";
    }
}
