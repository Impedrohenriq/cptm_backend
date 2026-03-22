namespace CPTM_Backend.DTOs;

public class RegisterRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public bool IsGestor { get; set; }
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public AuthUserDto? User { get; set; }
}
