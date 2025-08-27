using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Practice01.Presentation.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class TestRuleEngineController : ControllerBase
{
}