using Microsoft.EntityFrameworkCore;
using Api.Infrastructure.Persistence;
using Api.Application;
using Api.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using System.IO;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIParent", Version = "v1" });
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

builder.Services.AddDbContext<Api.Infrastructure.Persistence.AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://phuhuynh.hub.edu.vn", "https://phuhuynh.hub.edu.vn")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IScoreQueryService, ScoreQueryService>();
builder.Services.AddScoped<IStudentQueryService, StudentQueryService>();
builder.Services.AddScoped<IScheduleQueryService, ScheduleQueryService>();

var app = builder.Build();

// Enable Swagger for all development-style environments (Development, Dev*, Local).
var isDevelopmentEnvironment =
    app.Environment.IsDevelopment()
    || app.Environment.EnvironmentName.StartsWith("Dev", StringComparison.OrdinalIgnoreCase)
    || app.Environment.IsEnvironment("Local");

if (isDevelopmentEnvironment)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIParent v1");
        c.RoutePrefix = "swagger";
    });
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

// Use static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Enable CORS
app.UseCors("Default");


app.MapControllers();

app.Run();
