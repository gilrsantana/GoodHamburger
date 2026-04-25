using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Database.Accounts.Entities;

public sealed class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    
    public string? JwtTokenId { get; set; }
}
