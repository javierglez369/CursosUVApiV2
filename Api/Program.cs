using Api.Extensions;
using Api.Middleware;
using Application;
using Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Agregar OpenAPI y documentación
builder.Services.AddOpenApiDocumentation(builder.Configuration);

// Agregar CORS
builder.Services.AddCorsConfiguration(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

await app.SeedRolesAsync();

app.UseMiddleware<ExceptionMiddleware>();


app.UseHttpsRedirection();

// ✅ Aplicar CORS
app.UseCorsConfiguration(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseOpenApiDocumentation(builder.Configuration);

var cadena = "hola mundo";
var cadenaMayusculas = cadena.AMayusculas();



app.Run();
