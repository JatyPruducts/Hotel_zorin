using Hotel.Domain.Interfaces;
using Hotel.Domain.Services;
using Hotel.Infrastructure.Data;
using Hotel.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Ручная настройки Serilog для логов в файл "logs/log-.txt" с ежедневным файлом.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Заменяем стандартный логгер на Serilog
builder.Host.UseSerilog(Log.Logger);

// Подключаем контроллеры
builder.Services.AddControllers();

// Читаем строку подключения из appsettings.json
var configuration = builder.Configuration;
string connectionString = configuration.GetConnectionString("DefaultConnection")
                          ?? throw new Exception("Connection string 'DefaultConnection' not found in appsettings.json");

// Регистрируем DbContext (EF + PostgreSQL)
builder.Services.AddDbContext<ToursDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Регистрируем репозитории 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<IDepartureRepository, DepartureRepository>();

// Регистрируем сервисы
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IDepartureService, DepartureService>();

// Регистрируем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
