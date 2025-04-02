using AppointmentManagementAPI.DTOs;
using AppointmentManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
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

            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime appointmentPST = TimeZoneInfo.ConvertTimeFromUtc(appointmentDto.AppointmentDate.ToUniversalTime(), pstZone);

            // Validate appointment time in PST
            if (appointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || appointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be scheduled between 9 AM and 7 PM PST. Requested time: {appointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            await _service.AddAppointmentAsync(appointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointmentDto.Id }, appointmentDto);
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

            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime appointmentPST = TimeZoneInfo.ConvertTimeFromUtc(appointmentDto.AppointmentDate.ToUniversalTime(), pstZone);

            // Validate appointment time in PST
            if (appointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || appointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be scheduled between 9 AM and 7 PM PST. Requested time: {appointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            await _service.UpdateAppointmentAsync(id, appointmentDto);
            return NoContent();
        }

        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var result = await _service.CompleteAppointmentAsync(id);
            return result ? Ok($"Appointment {id} marked as completed.") : NotFound($"No appointment found with ID: {id}.");
        }

        [HttpPut("reschedule/{id}")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleRequestDTO request)
        {
            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime newAppointmentPST = TimeZoneInfo.ConvertTimeFromUtc(request.NewAppointmentDate.ToUniversalTime(), pstZone);

            // Validate reschedule time in PST
            if (newAppointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || newAppointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be rescheduled between 9 AM and 7 PM PST. Requested time: {newAppointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            var result = await _service.RescheduleAppointmentAsync(id, request);
            return result ? Ok($"Appointment {id} rescheduled successfully.") : NotFound($"Rescheduling failed. Check appointment ID or requestor name.");
        }
        [HttpPut("complete/by-name/{name}")]
        public async Task<IActionResult> CompleteAppointmentByName(string name)
        {
            var result = await _service.CompleteAppointmentByNameAsync(name);
            return result ? Ok($"Appointments for {name} marked as completed.") : NotFound($"No appointments found for {name}.");
        }

        [HttpPut("reschedule/by-name/{name}")]
        public async Task<IActionResult> RescheduleAppointmentByName(string name, [FromBody] RescheduleRequestDTO request)
        {
            // Convert to PST
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime newAppointmentPST = TimeZoneInfo.ConvertTimeFromUtc(request.NewAppointmentDate.ToUniversalTime(), pstZone);

            // Validate reschedule time in PST
            if (newAppointmentPST.TimeOfDay < new TimeSpan(9, 0, 0) || newAppointmentPST.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                return BadRequest($"Appointments can only be rescheduled between 9 AM and 7 PM PST. Requested time: {newAppointmentPST:yyyy-MM-dd hh:mm tt} PST");
            }

            var result = await _service.RescheduleAppointmentByNameAsync(name, request);
            return result ? Ok($"Appointments for {name} rescheduled successfully.") : NotFound($"Rescheduling failed. No appointments found for {name}.");
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var result = await _service.CancelAppointmentAsync(id);
            return result ? NoContent() : NotFound($"No appointment found with ID: {id} to cancel.");
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
            return NoContent();
        }
    }
}

