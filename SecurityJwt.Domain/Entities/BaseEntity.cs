namespace SecurityJwt.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Status { get; set; } = 1; // flag for soft delete entries
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateModified { get; set; }
}
