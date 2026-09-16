using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Zavala.Api.Auth;

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

// 1. Inyectar JwtOpciones y TokenService
var jwtOpciones = builder.Configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>() ?? new JwtOpciones();
if (builder.Environment.IsDevelopment() && string.IsNullOrEmpty(jwtOpciones.Key))
{
    jwtOpciones.Key = TokenService.GenerarClaveEfimera();
}
builder.Services.AddSingleton(jwtOpciones);
builder.Services.AddSingleton<TokenService>();

// 2. Registrar el middleware de Autenticación con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOpciones.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOpciones.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpciones.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

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