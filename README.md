# Appointment Management API

## Packages Used
To set up the project, install the following .NET packages:

### Install Entity Framework Core tools
```sh
dotnet tool install --global dotnet-ef
```

### Add required EF Core packages
```sh
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

## Database Scaffolding Command
Use the following command to scaffold the database models:
```sh
dotnet ef dbcontext scaffold "Server=server_name;Database=database_name;User Id=user_id;Password=password;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models
```

## SQL Database Schema
```sql
USE Appointment;
CREATE TABLE Appointments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequestorName NVARCHAR(100) NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK(Status IN ('Scheduled','Completed','Pending','Cancelled')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

UPDATE Appointments
SET UpdatedAt = GETDATE()
WHERE UpdatedAt IS NULL;

SELECT TOP (1000) [Id], [RequestorName], [AppointmentDate], [Status], [CreatedAt], [UpdatedAt]
FROM [Appointment].[dbo].[Appointments];
```

## Features
- Add new appointments
- Complete appointments by id or requestor name
- Re-schedule appointments by id or requestor name
- Lookup appointments by date or requestor name
- Cancel appointments by id
- Appointments can be created from any time zone but must be scheduled between **9 AM PST to 7 PM PST**

## Implemented Functionalities
✅ **Status Constraints:**  
- Added a constraint to ensure the `Status` column only allows values: `Scheduled`, `Completed`, `Rescheduled`, `Pending`, and `Cancelled`.

✅ **Correct ID in Response:**  
- When making a `POST` request to create a new appointment, the response now returns the correct auto-generated `Id` instead of the default `0`.

✅ **PUT Request Success Response:**  
- The `PUT` request to update an appointment now provides a proper success response on successful updates.

✅ **Future Date Validation:**  
- New appointments cannot be scheduled for past dates. The system ensures that only future dates are accepted when creating an appointment.

✅ **Prevent Completing Cancelled Appointments:**  
- The system prevents marking an appointment as `Completed` if its status is `Cancelled`.

## How to Use
### Clone the repository:
```sh
git clone https://github.com/your-username/your-repository.git
```
### Install dependencies:
```sh
dotnet restore
```
### Apply migrations and update the database:
```sh
dotnet ef database update
```
### Run the project:
```sh
dotnet run
```

