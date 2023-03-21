using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRPConfig;
using SRPServer.Model;

namespace SRPServer.Controllers
{
    [Route("SRPapi/{controller}")]
    [ApiController]
    public class RodConfigEmbeddedController : Controller
    {
        SRPContext db;

        public RodConfigEmbeddedController(SRPContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConfigItem>> Index()
        {
            return new ObjectResult(from t in db.RodConfig select new ConfigItem(t.Id, t.Name));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RodConfig>> Get(int id)
        {
            RodConfig? unit = await db.RodConfig.Include(r => r.RodSectionConfigs).FirstOrDefaultAsync(t => t.Id == id);

            if (unit is null)
            {
                return NotFound();
            }

            return new ObjectResult(unit);
        }

        [HttpPost]
        public async Task<ActionResult<RodConfig>> Post(RodConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            foreach (RodSectionConfig section in config.RodSectionConfigs)
            {
                section.Id = 0;
                db.RodSectionConfig.Add(section);
            }
            //db.RodSectionConfig.AddRange(config.RodSectionConfigs);
            db.RodConfig.Add(config);

            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RodConfig>> Put(RodConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            if (!db.RodConfig.Any(x => x.Id == config.Id))
            {
                return NotFound();
            }

            var ent = db.Update(config);


//            await db.SaveChangesAsync();

            db.RemoveRange(
                db.RodSectionConfig.Where(x => config.Id == x.RodConfigId)
                .AsEnumerable()
                .Except(config.RodSectionConfigs)
                );

            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<RodConfig>> Delete(int id)
        {
            RodConfig? config = db.RodConfig.FirstOrDefault(x => x.Id == id);

            if (config is null)
            {
                return NotFound();
            }

            db.RodConfig.Remove(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }
    }
}
