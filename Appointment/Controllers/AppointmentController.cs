using AppointmentManagementAPI.DTOs;
using AppointmentManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return Ok(await _service.GetAppointmentsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointment(int id)
        {
            var appointment = await _service.GetAppointmentByIdAsync(id);
            return appointment == null ? NotFound() : Ok(appointment);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByName(string name)
        {
            var appointments = await _service.GetAppointmentsByNameAsync(name);
            return Ok(appointments);
        }

        [HttpGet("by-date/{date}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointmentsByDate(DateTime date)
        {
            var appointments = await _service.GetAppointmentsByDateAsync(date);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDTO appointmentDto)
        {
            await _service.AddAppointmentAsync(appointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointmentDto.Id }, appointmentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDto)
        {
            await _service.UpdateAppointmentAsync(id, appointmentDto);
            return NoContent();
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var result = await _service.CancelAppointmentAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            await _service.DeleteAppointmentAsync(id);
            return NoContent();
        }
    }
}

