using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RateMyProduction.Core.Entities;
using RateMyProduction.Infrastructure.Data;

namespace RateMyProduction.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionsController : ControllerBase
    {
        private readonly RateMyProductionContext _context;

        public ProductionsController(RateMyProductionContext context)
        {
            _context = context;
        }

        // GET: api/Productions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Production>>> GetProductions()
        {
            return await _context.Productions.ToListAsync();
        }

        // GET: api/Productions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Production>> GetProduction(int id)
        {
            var production = await _context.Productions.FindAsync(id);

            if (production == null)
            {
                return NotFound();
            }

            return production;
        }

        // PUT: api/Productions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduction(int id, Production production)
        {
            if (id != production.ProductionID)
            {
                return BadRequest();
            }

            _context.Entry(production).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Productions
        [HttpPost]
        public async Task<ActionResult<Production>> PostProduction(Production production)
        {
            _context.Productions.Add(production);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduction", new { id = production.ProductionID }, production);
        }

        // DELETE: api/Productions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduction(int id)
        {
            var production = await _context.Productions.FindAsync(id);
            if (production == null)
            {
                return NotFound();
            }

            _context.Productions.Remove(production);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductionExists(int id)
        {
            return _context.Productions.Any(e => e.ProductionID == id);
        }
    }
}
