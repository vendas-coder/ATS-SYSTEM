using ATS.API.Authorization;
using ATS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ATS.Application.Interfaces;
using ATS.Application.Services;
using ATS.Infrastructure.Repositories.Ef;
using System.Text.Json.Serialization; // 👈 optional but clean

var builder = WebApplication.CreateBuilder(args);

// Controllers + Enum as string (IMPORTANT)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

// DbContext
builder.Services.AddDbContext<ATSDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ATS.Infrastructure")
    )
);

// Application services
builder.Services.AddScoped<CandidateService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ApplicationService>(provider =>
    new ApplicationService(
        provider.GetRequiredService<IApplicationRepository>(),
        provider.GetRequiredService<IApplicationAuditLogRepository>(),
        provider.GetRequiredService<IApplicationEventRepository>(),
        provider.GetRequiredService<IRecruiterRepository>(),
        provider.GetRequiredService<IJobApplicationAssignmentRepository>(),
        provider.GetRequiredService<IEmailService>(),
        provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailOptions>>(),
        provider.GetRequiredService<ILogger<ApplicationService>>()
    )
);
builder.Services.AddScoped<ApplicationNoteService>();
builder.Services.AddScoped<AnalyticsService>();

// Email service
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, StubEmailService>();

// EF repositories ONLY
builder.Services.AddScoped<ICandidateRepository, EfCandidateRepository>();
builder.Services.AddScoped<IJobRepository, EfJobRepository>();
builder.Services.AddScoped<IApplicationRepository, EfApplicationRepository>();
builder.Services.AddScoped<IApplicationNoteRepository, EfApplicationNoteRepository>();
builder.Services.AddScoped<IApplicationAuditLogRepository, EfApplicationAuditLogRepository>();
builder.Services.AddScoped<IApplicationEventRepository, EfApplicationEventRepository>();
builder.Services.AddScoped<IRecruiterRepository, EfRecruiterRepository>();
builder.Services.AddScoped<IJobApplicationAssignmentRepository, EfJobApplicationAssignmentRepository>(); 

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Recruiter", policy => policy.RequireRole("Recruiter"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RecruiterOrAdmin", policy => policy.RequireRole("Recruiter", "Admin"));
    options.AddPolicy("AssignedApplication", policy => policy.Requirements.Add(new AssignedApplicationRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, AssignedApplicationHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();


// Global exception handling
app.UseMiddleware<ATS.API.Middleware.ExceptionMiddleware>();
app.UseCors("AllowSwagger");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
