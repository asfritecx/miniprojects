using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new string[] { "Hello", "World" });
        }

        [HttpPost]
        public IActionResult Post([FromBody] string message)
        {
            if (message == "Hello")
            {
                return Ok(new { response = "Hello World" });
            }
            return BadRequest(new { error = "Invalid message" });
        }
    }
}


