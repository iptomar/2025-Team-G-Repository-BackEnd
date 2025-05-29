using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEndHorario.Data;
using BackEndHorario.Models;

namespace BackEndHorario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlocosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Blocos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blocos>>> GetBlocos()
        {
            return await _context.Blocos.ToListAsync();
        }

        // GET: api/Blocos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blocos>> GetBlocos(int id)
        {
            var blocos = await _context.Blocos.FindAsync(id);

            if (blocos == null)
            {
                return NotFound();
            }

            return blocos;
        }

        // PUT: api/Blocos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlocos(int id, Blocos blocos)
        {
            if (id != blocos.Id)
            {
                return BadRequest();
            }

            _context.Entry(blocos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlocosExists(id))
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

        // POST: api/Blocos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Blocos>> PostBlocos(Blocos blocos)
        {
            _context.Blocos.Add(blocos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlocos", new { id = blocos.Id }, blocos);
        }

        // DELETE: api/Blocos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlocos(int id)
        {
            var blocos = await _context.Blocos.FindAsync(id);
            if (blocos == null)
            {
                return NotFound();
            }

            _context.Blocos.Remove(blocos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlocosExists(int id)
        {
            return _context.Blocos.Any(e => e.Id == id);
        }
    }
}
