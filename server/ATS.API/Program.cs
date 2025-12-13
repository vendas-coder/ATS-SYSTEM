using ATS.Application.Interfaces;
using ATS.Application.Services;
using ATS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Application services
builder.Services.AddScoped<CandidateService>();

// Infrastructure (temporary in-memory repo)
builder.Services.AddSingleton<ICandidateRepository, InMemoryCandidateRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
