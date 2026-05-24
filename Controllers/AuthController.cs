using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CPTM_Backend.Data;
using CPTM_Backend.DTOs;
using CPTM_Backend.Models;
using CPTM_Backend.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CPTM_Backend.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var fullName = (dto.FullName ?? string.Empty).Trim();
        var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();
        var password = dto.Password ?? string.Empty;
        var confirmPassword = dto.ConfirmPassword ?? string.Empty;

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Error = "Preencha nome completo, e-mail, senha e confirmacao de senha."
            });
        }

        if (fullName.Length < 3)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Error = "Informe o nome completo do usuario."
            });
        }

        if (!EmailValido(email))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Error = "Informe um e-mail valido."
            });
        }

        if (password.Length < 8)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Error = "A senha precisa ter no minimo 8 caracteres."
            });
        }

        if (!password.Equals(confirmPassword, StringComparison.Ordinal))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Error = "Senha e confirmacao nao conferem."
            });
        }

        var existingUser = await _db.Usuarios
            .AsNoTracking()
            .Select(u => new { u.Id, u.Email })
            .FirstOrDefaultAsync(u => u.Email == email);

        var alreadyExists = existingUser is not null;

        if (alreadyExists)
        {
            return Conflict(new AuthResponseDto
            {
                Success = false,
                Error = "Este e-mail ja esta cadastrado."
            });
        }

        var (hash, salt) = PasswordHasher.HashPassword(password);
        var isGestor = IsGestorEmail(email);

        var user = new UsuarioApp
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsGestor = isGestor,
            NomeExibicao = fullName,
            Cargo = isGestor ? "Gestor Ambiental" : "Inspetor Ambiental",
            Linha = isGestor ? "Todas as Linhas" : "Linha nao definida",
            CriadoEmUtc = DateTime.UtcNow,
        };

        _db.Usuarios.Add(user);
        await _db.SaveChangesAsync();

        var (token, expiresAtUtc) = BuildJwtToken(user);

        return StatusCode(StatusCodes.Status201Created, new AuthResponseDto
        {
            Success = true,
            User = ToAuthUserDto(user),
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();
        var password = dto.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Error = "Preencha e-mail e senha."
            });
        }

        var user = await _db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user is null || !PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            return Unauthorized(new AuthResponseDto
            {
                Success = false,
                Error = "Usuario ou senha invalida."
            });
        }

        var (token, expiresAtUtc) = BuildJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Success = true,
            User = ToAuthUserDto(user),
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        });
    }

    private (string token, DateTime expiresAtUtc) BuildJwtToken(UsuarioApp user)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "CPTM.Backend";
        var audience = _configuration["Jwt:Audience"] ?? "CPTM.Frontend";
        var key = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            throw new InvalidOperationException("Jwt:Key nao configurada corretamente.");

        var expiresAtUtc = DateTime.UtcNow.AddHours(6);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.NomeExibicao),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.NomeExibicao),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.IsGestor ? "Gestor" : "Inspetor")
        };

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expiresAtUtc);
    }

    private static bool EmailValido(string email)
    {
        return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.CultureInvariant);
    }

    private static bool IsGestorEmail(string email)
    {
        return email.StartsWith("gestor@", StringComparison.OrdinalIgnoreCase)
            || email.Contains("@gestor", StringComparison.OrdinalIgnoreCase);
    }

    private static AuthUserDto ToAuthUserDto(UsuarioApp u)
    {
        return new AuthUserDto
        {
            Id = u.Id,
            Name = u.NomeExibicao,
            Email = u.Email,
            Role = u.Cargo,
            Line = u.Linha,
            Initials = BuildInitials(u.NomeExibicao),
            IsGestor = u.IsGestor,
        };
    }

    private static string BuildInitials(string nome)
    {
        var pieces = (nome ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(p => char.ToUpperInvariant(p[0]).ToString())
            .ToArray();

        return pieces.Length == 0 ? "US" : string.Join(string.Empty, pieces);
    }
}
