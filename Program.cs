using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using CPTM_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Oracle via Entity Framework Core ────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDB")));

// ── CORS – permite chamadas do frontend Vue.js ───────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.SetIsOriginAllowed(origin =>
              {
                  if (allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                  {
                      return true;
                  }

                  if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                  {
                      return false;
                  }

                  var isLocalHost = uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                                 || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase);

                  var isHttp = uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                            || uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);

                  return isLocalHost && isHttp;
              })
              .AllowAnyMethod()
              .AllowAnyHeader()));

// ── Controllers + Swagger ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "CPTM – API Efluentes (FDC-EEA.EF)",
        Version     = "v1",
        Description = "API REST para o Formulário de Cadastramento/Caracterização de Efluentes da CPTM."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions());
app.MapControllers();
app.Run();
