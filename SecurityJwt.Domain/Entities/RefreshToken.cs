using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public string IdentityUserId { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;
    public DateTime DateExpire { get; set; }
}
