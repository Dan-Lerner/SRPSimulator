using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRPConfig;
using SRPServer.Model;

namespace SRPServer.Controllers
{
    [Route("SRPapi/{controller}")]
    [ApiController]
    public class TubingConfigEmbeddedController : Controller
    {
        SRPContext db;

        public TubingConfigEmbeddedController(SRPContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConfigItem>> Index()
        {
            return new ObjectResult(from t in db.TubingConfig
                                    select new ConfigItem(t.Id, t.Name));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TubingConfig>> Get(int id)
        {
            TubingConfig? config = await db.TubingConfig.FirstOrDefaultAsync(t => t.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            return new ObjectResult(config);
        }

        [HttpPost]
        public async Task<ActionResult<TubingConfig>> Post(TubingConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            db.TubingConfig.Add(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TubingConfig>> Put(TubingConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            if (!db.TubingConfig.Any(x => x.Id == config.Id))
            {
                return NotFound();
            }

            db.Update(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TubingConfig>> Delete(int id)
        {
            TubingConfig? config = db.TubingConfig.FirstOrDefault(x => x.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            db.TubingConfig.Remove(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }
    }
}
