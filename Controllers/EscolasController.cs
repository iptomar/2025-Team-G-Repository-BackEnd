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
    public class EscolasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EscolasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Escolas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Escolas>>> GetEscolas()
        {
            return await _context.Escolas.ToListAsync();
        }

        // GET: api/Escolas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Escolas>> GetEscolas(int id)
        {
            var escolas = await _context.Escolas.FindAsync(id);

            if (escolas == null)
            {
                return NotFound();
            }

            return escolas;
        }

        // PUT: api/Escolas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEscolas(int id, Escolas escolas)
        {
            if (id != escolas.Id)
            {
                return BadRequest();
            }

            _context.Entry(escolas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EscolasExists(id))
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

        // POST: api/Escolas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Escolas>> PostEscolas(Escolas escolas)
        {
            _context.Escolas.Add(escolas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEscolas", new { id = escolas.Id }, escolas);
        }

        // DELETE: api/Escolas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEscolas(int id)
        {
            var escolas = await _context.Escolas.FindAsync(id);
            if (escolas == null)
            {
                return NotFound();
            }

            _context.Escolas.Remove(escolas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EscolasExists(int id)
        {
            return _context.Escolas.Any(e => e.Id == id);
        }
    }
}
