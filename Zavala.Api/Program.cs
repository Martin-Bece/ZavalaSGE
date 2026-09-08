using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar la conexión a MySQL / MariaDB
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ZavalaAppContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Agregar soporte para Controladores
builder.Services.AddControllers();

// Configuración de OpenAPI (Swagger)
builder.Services.AddOpenApi();

// Registrar política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();

// Mapear las rutas de los controladores
app.MapControllers();

app.Run();