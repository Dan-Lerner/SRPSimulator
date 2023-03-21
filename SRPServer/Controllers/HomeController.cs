using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SRPServer.Controllers
{
    [Route("SRPapi/")]
    [ApiController]
    public class HomeController : Controller
    {
        // Ping
        [HttpGet]
        public IActionResult Index()
        {
            return new EmptyResult();
        }
    }
}
