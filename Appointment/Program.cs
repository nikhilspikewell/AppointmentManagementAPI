//using AppointmentManagementAPI.Data;
//using AppointmentManagementAPI.Repositories;
//using AppointmentManagementAPI.Services;
//using AppointmentManagementAPI.Mappings;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//var builder = WebApplication.CreateBuilder(args);

//// Database Connection
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

//// Dependency Injection
//builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
//builder.Services.AddScoped<IAppointmentService, AppointmentService>();

//// AutoMapper
//builder.Services.AddAutoMapper(typeof(MappingProfile));

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();
//app.UseAuthorization();
//app.MapControllers();
//app.Run();








using AppointmentManagementAPI.Data;
using AppointmentManagementAPI.Repositories;
using AppointmentManagementAPI.Services;
using AppointmentManagementAPI.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Dependency Injection
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
