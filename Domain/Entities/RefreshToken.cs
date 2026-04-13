namespace Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string UserId { get; set; }

    // Navegación — se configura en Persistence/Configurations
    public ApplicationUser? User { get; set; }

    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Propiedad calculada — no mapeada a columna
    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}