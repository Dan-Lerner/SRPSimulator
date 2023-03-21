using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRPServer.Model;

namespace SRPServer.Controllers
{
    [Route("SRPapi/{controller}")]
    [ApiController]
    public class SRPConfigEmbeddedController : Controller
    {
        SRPContext db;

        public SRPConfigEmbeddedController(SRPContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConfigItem>> Index()
        {
            return new ObjectResult(from t in db.SRPConfig
                                    select new ConfigItem(t.Id, t.Name));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SRPConfig.SRPConfig>> Get(int id)
        {
            SRPConfig.SRPConfig? config = await db.SRPConfig.FirstOrDefaultAsync(t => t.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            return new ObjectResult(config);
        }

        [HttpPost]
        public async Task<ActionResult<SRPConfig.SRPConfig>> Post(SRPConfig.SRPConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            db.SRPConfig.Add(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SRPConfig.SRPConfig>> Put(SRPConfig.SRPConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            if (!db.SRPConfig.Any(x => x.Id == config.Id))
            {
                return NotFound();
            }

            db.Update(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<SRPConfig.SRPConfig>> Delete(int id)
        {
            SRPConfig.SRPConfig? config = db.SRPConfig.FirstOrDefault(x => x.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            db.SRPConfig.Remove(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }
    }
}
