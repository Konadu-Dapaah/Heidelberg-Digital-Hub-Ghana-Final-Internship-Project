              FINAL HEIDELBERG INTENSHIP PROJECT
COMMUTE360 :
           A Bus Pass System built with c#/ASP.NET core, designed to manage daily workplace commuting.

KEY FEATURES:
            1. Staff registration/login(auth)
            2. Route selection(Staff pick boarding and drop off points as well as days of work)
            3. Dashboard(show routes)
            4. Real-time bus proximity alerts (not sure if i can make this work)
            5. Deploy on AWS (RDS)- Devops aspect of the internship


            WEEK 1
Day 1

Objectives:
           1. Set up ASP.NET Core Project
           2. Configure Database and Depandencies-Install needed packages
           3.Define Core Data Models

Tech Stack:
          Framework: ASP.NET Core Web API
          Database: Npgsql PostreSQL
          Database Container: Docker

Accomplishment:
            1.I was able to set up the core ASP.NET Core Web API project and established a connection to a local PostgreSQL instance running inside Docker

            2.Core models were also created
            -User (Model)
                .Id (Primary Key)
                .Name, Email, PasswordHash
                .Role (Default: "Staff")
                .Bookings (1-to-Many Navigation Property)

            -Route (Model)
                .Id (Primary Key)
                .Origin, Destination, ScheduleTime, Capacity
                .bookings (1-to-Many Navigation Property)

            -Booking (Model)
                .Id (Primary Key)
                .UserId 
                .RouteId 
                .Status (Default: "Confirmed")

            3.Database Configuration(struggled with this a bit)

Difficulties faced and Fixes
1.Dotnet failed to build because the neede package for the database which is Npgsql.EntityFrameworkCore.PostgreSQL wasn't installed
Fix: installed the package Npgsql.EntityFrameworkCore.PostgreSQL

2.Dotnet also failded to build because the compiler failed to determine whether Route referred to the custom model Commute360.Models.Route or ASP.NET Core's internal routing class Microsoft.AspNetCore.Routing.Route
Fix:Changed public DbSet<Route> Routes { get; set; } to public DbSet<Commute360.Models.Route> Routes { get; set; } to resolve the ambiguous reference error between Commute360.Models.Route and Microsoft.AspNetCore.Routing.Route


Day 2

Objectives: 
            Work on Authentication - staff registration and login

Packages Installed:dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
                   dotnet add package BCrypt.Net-Next
                   dotnet add package Scalar.AspNetCore
Tech Stack:
          Framework: ASP.NET Core Web API
          Database: Npgsql PostreSQL
          Database Container: Docker


Accomplishment:
               User Authentication API: Created endpoints for user registration (POST /api/auth/register) and user authentication (POST /api/auth/login).

               Secure Password Handling:Integrated BCrypt.Net-Next to hash passwords with a random salt before database storage,ensuring plaintext passwords are never saved/exposed.

               JWT Service Implementation: Built TokenService to generate signed JWTs containing user claims (NameIdentifier, Email, Role) with an 8-hour expiration period.

               Data Protection & DTOs: Implemented RegisterDto and LoginDto to decouple internal entity structures (User model) from request/response payloads.
 
Difficulties Faced:
                  During setup, I encountered false-positive red syntax errors across my C# files. I identified that my editor was targeting the parent directory instead of the Commute360 project root where the .csproj file resides. Opening the project directly at its root folder allowed the language server to correctly recognize the dependencies and clear the highlights.

Day 3