﻿using AppointmentManagementAPI.DTOs;
using AppointmentManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace AppointmentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments()
        {
            var appointments = await _service.GetAppointmentsAsync();
            if (appointments == null || !appointments.Any())
            {
                return NotFound("No appointments found.");
            }
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(int id)
        {
            var appointment = await _service.GetAppointmentByIdAsync(id);
            return appointment == null ? NotFound($"No appointment found with ID: {id}") : Ok(appointment);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByName(string name)
        {
            var appointments = await _service.GetAppointmentsByNameAsync(name);
            if (appointments == null || !appointments.Any())
            {
                return NotFound($"No appointments found for name: {name}");
            }
            return Ok(appointments);
        }

        [HttpGet("by-date/{date}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByDate(DateTime date)
        {
            var appointments = await _service.GetAppointmentsByDateAsync(date);
            if (appointments == null || !appointments.Any())
            {
                return NotFound($"No appointments found for date: {date:yyyy-MM-dd}");
            }
            return Ok(appointments);
        }






        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDTO appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Invalid appointment data.");
            }

            var validStatuses = new HashSet<string> { "Scheduled", "Completed", "Pending", "Cancelled", "Rescheduled" };

            if (!validStatuses.Contains(appointmentDto.Status))
            {
                return BadRequest($"Invalid status: {appointmentDto.Status}. Allowed values are: {string.Join(", ", validStatuses)}.");
            }

            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime appointmentPST = TimeZoneInfo.ConvertTimeFromUtc(appointmentDto.AppointmentDate.ToUniversalTime(), pstZone);
            DateTime currentPST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pstZone);

            //  Prevent past appointments
            if (appointmentPST < currentPST)
            {
                return BadRequest($"Appointments cannot be scheduled in the past. Current time: {currentPST:yyyy-MM-dd hh:mm tt} PST.");
            }

            //  Restrict appointment times (9 AM - 7 PM PST)
            if (appointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || appointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be scheduled between 9 AM and 7 PM PST. Requested time: {appointmentPST:yyyy-MM-dd hh:mm tt} PST.");
            }

            int appointmentId = await _service.AddAppointmentAsync(appointmentDto);

            return CreatedAtAction(nameof(GetAppointment), new { id = appointmentId }, new
            {
                Id = appointmentId,
                RequestorName = appointmentDto.RequestorName,
                AppointmentDate = appointmentDto.AppointmentDate,
                Status = appointmentDto.Status
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Invalid appointment data.");
            }

            var existingAppointment = await _service.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                return NotFound($"No appointment found with ID: {id}");
            }

            if (existingAppointment.Status == "Completed" || existingAppointment.Status == "Cancelled")
            {
                return BadRequest($"Cannot update an appointment that is already {existingAppointment.Status}.");
            }

            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime appointmentPST = TimeZoneInfo.ConvertTimeFromUtc(appointmentDto.AppointmentDate.ToUniversalTime(), pstZone);

            if (appointmentPST.Date < DateTime.UtcNow.Date)
            {
                return BadRequest($"Cannot update to a past date. Provided date: {appointmentPST:yyyy-MM-dd}");
            }

            if (appointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || appointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be scheduled between 9 AM and 7 PM PST. Requested time: {appointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            try
            {
                await _service.UpdateAppointmentAsync(id, appointmentDto);
                return Ok(new
                {
                    message = "Appointment updated successfully",
                    id = id,
                    updatedTime = appointmentPST.ToString("yyyy-MM-dd hh:mm tt") + " PST"
                });
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("CHK_Status") == true)
            {
                return BadRequest("Status must be one of: Scheduled, Completed, Pending, Cancelled, or Rescheduled.");
            }
            catch (Exception ex)
            {
                // For any other unexpected errors
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }



        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var result = await _service.CompleteAppointmentAsync(id);

            //if (result == null)
            //{
            //    return NotFound($"No appointment found with ID: {id}.");
            //}

            if (!result)
            {
                return NotFound($"No appointment found with ID: {id} or it is cancelled.");
            }

            return Ok($"Appointment {id} marked as completed.");
        }






        //[HttpPut("reschedule/{id}")]
        //public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleRequestDTO request)
        //{
        //    // Convert to PST
        //    TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        //    DateTime newAppointmentPST = TimeZoneInfo.ConvertTimeFromUtc(request.NewAppointmentDate.ToUniversalTime(), pstZone);

        //    // Validate reschedule time in PST
        //    if (newAppointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || newAppointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
        //    {
        //        return BadRequest($"Appointments can only be rescheduled between 9 AM and 7 PM PST. Requested time: {newAppointmentPST:yyyy-MM-dd hh:mm tt} PST");
        //    }

        //    var result = await _service.RescheduleAppointmentAsync(id, request);
        //    return result ? Ok($"Appointment {id} rescheduled successfully.") : NotFound($"Rescheduling failed. Check appointment ID or requestor name.");
        //}

        [HttpPut("reschedule/{id}")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleRequestDTO request)
        {
            var existingAppointment = await _service.GetAppointmentByIdAsync(id);

            if (existingAppointment == null)
            {
                return NotFound($"No appointment found with ID: {id}");
            }

            if (existingAppointment.Status == "Completed" || existingAppointment.Status == "Cancelled")
            {
                return BadRequest($"Cannot reschedule a {existingAppointment.Status.ToLower()} appointment.");
            }

            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime newAppointmentPST = TimeZoneInfo.ConvertTimeFromUtc(request.NewAppointmentDate.ToUniversalTime(), pstZone);

            if (newAppointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || newAppointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be rescheduled between 9 AM and 7 PM PST. Requested time: {newAppointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            var result = await _service.RescheduleAppointmentAsync(id, request);
            return result ? Ok($"Appointment {id} rescheduled successfully.") : BadRequest("Rescheduling failed.");
        }



        [HttpPut("complete/by-name/{name}")]
        public async Task<IActionResult> CompleteAppointmentByName(string name)
        {
            var result = await _service.CompleteAppointmentByNameAsync(name);

            //if (result == null)
            //{
            //    return NotFound($"No appointments found for {name}.");
            //}
            if (!result)
            {
                return NotFound($"No appointments found for {name} or his/her appointments are cancelled.");
            }

            return Ok($"Appointments for {name} marked as completed.");
        }




        //[HttpPut("reschedule/by-name/{name}")]
        //public async Task<IActionResult> RescheduleAppointmentByName(string name, [FromBody] RescheduleRequestDTO request)
        //{
        //    // Convert to PST
        //    TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        //    DateTime newAppointmentPST = TimeZoneInfo.ConvertTimeFromUtc(request.NewAppointmentDate.ToUniversalTime(), pstZone);

        //    // Validate reschedule time in PST
        //    if (newAppointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || newAppointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
        //    {
        //        return BadRequest($"Appointments can only be rescheduled between 9 AM and 7 PM PST. Requested time: {newAppointmentPST:yyyy-MM-dd hh:mm tt} PST");
        //    }

        //    var result = await _service.RescheduleAppointmentByNameAsync(name, request);
        //    return result ? Ok($"Appointments for {name} rescheduled successfully.") : NotFound($"Rescheduling failed. No appointments found for {name}.");
        //}

        [HttpPut("reschedule/by-name/{name}")]
        public async Task<IActionResult> RescheduleAppointmentByName(string name, [FromBody] RescheduleRequestDTO request)
        {
            var appointments = await _service.GetAppointmentsByNameAsync(name);

            if (appointments == null || !appointments.Any())
            {
                return NotFound($"No appointments found for {name}.");
            }

            if (appointments.Any(a => a.Status == "Completed" || a.Status == "Cancelled"))
            {
                return BadRequest($"Cannot reschedule appointments for {name} because his appointment is either completed or cancelled.");
            }

            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime newAppointmentPST = TimeZoneInfo.ConvertTimeFromUtc(request.NewAppointmentDate.ToUniversalTime(), pstZone);

            if (newAppointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || newAppointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be rescheduled between 9 AM and 7 PM PST. Requested time: {newAppointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            var result = await _service.RescheduleAppointmentByNameAsync(name, request);
            return result ? Ok($"Appointments for {name} rescheduled successfully.") : BadRequest($"Rescheduling failed.");
        }


        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var result = await _service.CancelAppointmentAsync(id);
            return result ? Ok($"Appointments for {id} successfully cancelled.") : NotFound($"No appointment found with ID: {id} to cancel.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var existingAppointment = await _service.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                return NotFound($"No appointment found with ID: {id} to delete.");
            }
            await _service.DeleteAppointmentAsync(id);

            return Ok(new
            {
                message = "Appointment deleted successfully",
                
            });
        }
    }
}




