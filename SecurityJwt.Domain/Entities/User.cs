namespace SecurityJwt.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    // Id to link this user to an identityUser (provided by .net core)
    public Guid IdentityUserId { get; set; }
}
