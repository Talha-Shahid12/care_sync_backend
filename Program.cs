using CareSync.Data;
using CareSync.Repositories;
using CareSync.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<JWTService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "CareSync",
            ValidateAudience = true,
            ValidAudience = "CareSyncUsers",
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 400; 
                    context.Response.ContentType = "application/json";
                    var response = new { message = "Authorization token is missing." };
                    return context.Response.WriteAsJsonAsync(response);
                }
                context.HandleResponse();
                context.Response.StatusCode = 401; 
                context.Response.ContentType = "application/json";
                var defaultResponse = new { message = "Unauthorized access. Please provide a valid token." };
                return context.Response.WriteAsJsonAsync(defaultResponse);
            }
        };
    });

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.Run();
