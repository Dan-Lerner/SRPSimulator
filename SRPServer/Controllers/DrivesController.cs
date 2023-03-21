using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRPConfig;
using SRPServer.Model;

namespace SRPServer.Controllers
{
    [Route("SRPapi/{controller}")]
    [ApiController]
    public class DriveConfigEmbeddedController : Controller
    {
        SRPContext db;

        public DriveConfigEmbeddedController(SRPContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConfigItem>> Index()
        {
            return new ObjectResult(from t in db.DriveConfig
                                    select new ConfigItem(t.Id, t.Name));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DriveConfig>> Get(int id)
        {
            DriveConfig? unit = await db.DriveConfig.FirstOrDefaultAsync(t => t.Id == id);

            if (unit is null)
            {
                return NotFound();
            }

            return new ObjectResult(unit);
        }

        [HttpPost]
        public async Task<ActionResult<DriveConfig>> Post(DriveConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            db.DriveConfig.Add(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DriveConfig>> Put(DriveConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            if (!db.DriveConfig.Any(x => x.Id == config.Id))
            {
                return NotFound();
            }

            db.Update(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<DriveConfig>> Delete(int id)
        {
            DriveConfig? config = db.DriveConfig.FirstOrDefault(x => x.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            db.DriveConfig.Remove(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }
    }
}
