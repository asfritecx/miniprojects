using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NormalController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello World! You have reached a normal API endpoint!" });
        }
    }
}
