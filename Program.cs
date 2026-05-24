using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CPTM_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Oracle via Entity Framework Core ────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDB")));

// ── CORS – permite chamadas do frontend Vue.js ───────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];
var isDevelopment = builder.Environment.IsDevelopment();

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

                  return isDevelopment && isLocalHost && isHttp;
              })
              .AllowAnyMethod()
              .AllowAnyHeader()));

// ── Controllers + Swagger ────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("Configure Jwt:Key com no minimo 32 caracteres.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CPTM.Backend";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CPTM.Frontend";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !isDevelopment;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();
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

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Use: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions());
app.MapControllers();
app.Run();
