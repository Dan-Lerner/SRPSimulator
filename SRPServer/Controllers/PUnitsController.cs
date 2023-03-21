using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRPServer.Model;
using SRPConfig;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SRPServer.Controllers
{
    public record ConfigItem(int Id, string? Name);

    [Route("SRPapi/{controller}")]
    [ApiController]
    public class PUnitConfigEmbeddedController : Controller
    {
        readonly SRPContext db;

        public PUnitConfigEmbeddedController(SRPContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConfigItem>> Index()
        {
            return new ObjectResult(from u in db.PUnitConfig
                                    select new ConfigItem(u.Id, u.Name));
        }
        //public async Task<ActionResult<IEnumerable<ConfigItem>>> Index()
        //{
        //    return new ObjectResult(from u in db.PUnitConfig
        //                            select new ConfigItem(u.Id, u.Name));
        //}

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PUnitConfig>> Get(int id)
        {
            PUnitConfig? config = await db.PUnitConfig.FirstOrDefaultAsync(x => x.Id == id);

            if (config is null)
            {
                return NotFound($"{id} is not found");
            }

            return new ObjectResult(config);
        }

        [HttpGet("{Name}")]
        public ActionResult<PUnitConfig> Get(string Name)
        {
            var config = from Unit in db.PUnitConfig
                         where Unit.Name == Name
                         select Unit;

            if (config is null)
            {
                return NotFound();
            }

            return new ObjectResult(config);
        }

        [HttpPost]
        public async Task<ActionResult<PUnitConfig>> Post(PUnitConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            db.PUnitConfig.Add(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PUnitConfig>> Put(PUnitConfig config)
        {
            if (config is null)
            {
                return BadRequest();
            }

            if (!db.PUnitConfig.Any(x => x.Id == config.Id))
            {
                return NotFound();
            }

            db.Update(config);
            await db.SaveChangesAsync();

            return Ok(config);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<PUnitConfig>> Delete(int id)
        {
            PUnitConfig? unit = db.PUnitConfig.FirstOrDefault(x => x.Id == id);

            if (unit is null)
            {
                return NotFound();
            }
            
            db.PUnitConfig.Remove(unit);
            await db.SaveChangesAsync();

            return Ok(unit);
        }
    }
}
