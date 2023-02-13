namespace SecurityJwt.Application.IServices;

public interface ICurrentUserService
{
    Task<string> GetCurrentUserId();
    Task<string> GetCurrentUserEmail();
}
