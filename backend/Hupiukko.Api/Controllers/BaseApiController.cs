using Microsoft.AspNetCore.Mvc;
using Hupiukko.Api.BusinessLogic.Models;

namespace Hupiukko.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected User? CurrentUser => HttpContext.Items["CurrentUser"] as User;
} 