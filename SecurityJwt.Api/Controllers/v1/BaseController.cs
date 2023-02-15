using Microsoft.AspNetCore.Mvc;
using SecurityJwt.Application.IConfiguration;

namespace SecurityJwt.Api.Controllers.v1;

//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[ApiController]
//[ApiVersion("1.0")]
public class BaseController : ControllerBase
{
    protected IUnitOfWork _unitOfWork;

    public BaseController(
        IUnitOfWork unitOfWork
    )
    {
        _unitOfWork = unitOfWork;
    }
}
