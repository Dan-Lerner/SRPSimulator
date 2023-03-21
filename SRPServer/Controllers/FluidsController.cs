using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRPConfig;
using SRPServer.Model;

namespace SRPServer.Controllers
{
    [Route("SRPapi/{controller}")]
    [ApiController]
    public class FluidConfigEmbeddedController : Controller
    {
        SRPContext db;

        public FluidConfigEmbeddedController(SRPContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConfigItem>> Index()
        {
            return new ObjectResult(from t in db.FluidConfig
                                    select new ConfigItem(t.Id, t.Name));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FluidConfig>> Get(int id)
        {
            FluidConfig? congig = await db.FluidConfig.FirstOrDefaultAsync(t => t.Id == id);

            if (congig is null)
            {
                return NotFound();
            }

            return new ObjectResult(congig);
        }

        [HttpPost]
        public async Task<ActionResult<FluidConfig>> Post(FluidConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            db.FluidConfig.Add(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<FluidConfig>> Put(FluidConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            if (!db.FluidConfig.Any(x => x.Id == config.Id))
            {
                return NotFound();
            }

            db.Update(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<FluidConfig>> Delete(int id)
        {
            FluidConfig? config = db.FluidConfig.FirstOrDefault(x => x.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            db.FluidConfig.Remove(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }
    }
}
