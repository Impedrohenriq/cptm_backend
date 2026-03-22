namespace CPTM_Backend.Models;

public class UsuarioApp
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public bool IsGestor { get; set; }
    public string NomeExibicao { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Linha { get; set; } = string.Empty;
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
}
