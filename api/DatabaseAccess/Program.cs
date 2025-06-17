using DatabaseAccess.Data;
using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers(); // Enable MVC controllers

// Adicionar o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Database Access API", 
        Version = "v1",
        Description = "API para acessar dados de advogados e registros de horas"
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Database Access API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers(); // Map attribute-routed controllers

// Simple API endpoints without Swagger
app.MapGet("/", () => "API running successfully. Use /api/advogados to test database connection.")
    .WithName("GetRoot")
    .WithOpenApi();

// Advogados endpoints
app.MapGet("/api/advogados", async (ApplicationDbContext db) =>
{
    var advogados = await db.Advogados.ToListAsync();
    return Results.Ok(advogados);
})
.WithName("GetAdvogados")
.WithOpenApi();

app.MapGet("/api/advogados/{id}", async (ApplicationDbContext db, int id) =>
{
    var advogado = await db.Advogados.FindAsync(id);
    if (advogado == null)
        return Results.NotFound();
    return Results.Ok(advogado);
})
.WithName("GetAdvogadoById")
.WithOpenApi();

app.MapGet("/api/advogados/departamento/{departamento}", async (ApplicationDbContext db, string departamento) =>
{
    var advogados = await db.Advogados
        .Where(a => a.Departamento == departamento)
        .ToListAsync();
    return Results.Ok(advogados);
})
.WithName("GetAdvogadosByDepartamento")
.WithOpenApi();

// Horas endpoints
app.MapGet("/api/horas", async (ApplicationDbContext db) =>
{
    var horas = await db.Horas.ToListAsync();
    return Results.Ok(horas);
})
.WithName("GetHoras")
.WithOpenApi();

app.MapGet("/api/horas/{id}", async (ApplicationDbContext db, int id) =>
{
    var hora = await db.Horas.FirstOrDefaultAsync(h => h.ID == id);
    if (hora == null)
        return Results.NotFound();
    return Results.Ok(hora);
})
.WithName("GetHoraById")
.WithOpenApi();

app.MapGet("/api/horas/advogado/{id}", async (ApplicationDbContext db, int id) =>
{
    var horas = await db.Horas
        .Where(h => h.ID_Advogado == id)
        .ToListAsync();
    return Results.Ok(horas);
})
.WithName("GetHorasByAdvogado")
.WithOpenApi();

// Dashboard endpoints
app.MapGet("/api/dashboard/topadvogados", async (ApplicationDbContext db) =>
{
    var topAdvogados = await db.Horas
        .GroupBy(h => h.ID_Advogado)
        .Select(g => new
        {
            AdvogadoID = g.Key,
            TotalMinutos = g.Sum(h => h.Minutos_Registados)
        })
        .OrderByDescending(a => a.TotalMinutos)
        .Take(5)
        .ToListAsync();

    var result = new List<object>();
    foreach (var adv in topAdvogados)
    {
        var advogado = await db.Advogados.FirstOrDefaultAsync(a => a.ID == adv.AdvogadoID);
        if (advogado != null)
        {
            result.Add(new
            {
                ID = advogado.ID,
                Nome = advogado.Nome,
                Sobrenome = advogado.Sobrenome,
                Departamento = advogado.Departamento,
                TotalMinutos = adv.TotalMinutos
            });
        }
    }

    return Results.Ok(result);
})
.WithName("GetTopAdvogados")
.WithOpenApi();

app.Run();
