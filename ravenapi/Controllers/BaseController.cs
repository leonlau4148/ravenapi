using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ravenapi.Controllers;

public class BaseController : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}