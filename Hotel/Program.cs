using Hotel.Domain.Interfaces;
using Hotel.Domain.Services;
using Hotel.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Подключаем контроллеры
builder.Services.AddControllers();

// Регистрируем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Читаем строку подключения из appsettings.json
var configuration = builder.Configuration;
string connectionString = configuration.GetConnectionString("DefaultConnection")
                          ?? throw new Exception("Connection string 'DefaultConnection' not found in appsettings.json");

// Регистрируем репозитории  (_ - лямбда выражение
builder.Services.AddScoped<IDepartureRepository>(_ => new DepartureRepository(connectionString));

// Регистрируем сервисы
builder.Services.AddScoped<IDepartureService, DepartureService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
