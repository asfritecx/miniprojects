using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
                return Ok(new { response = $"Hello World! You sent a Post Request with {message}" });
            }
            return BadRequest(new { error = "Invalid message" });
        }

        [HttpPost("restricted")]
        public IActionResult Restricted([FromHeader] string clientId, [FromHeader] string clientSecret)
        {
            var validClientId = _configuration["ClientSettings:ClientId"];
            var validClientSecret = _configuration["ClientSettings:ClientSecret"];

            if (clientId == validClientId && clientSecret == validClientSecret)
            {
                return Ok(new { response = "Access granted to restricted area" });
            }

            return Unauthorized(new { error = "Invalid clientId or clientSecret" });
        }

    }
}


