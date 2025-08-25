namespace Models;

public class Session : AppModelBase
{
    public User User { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
}