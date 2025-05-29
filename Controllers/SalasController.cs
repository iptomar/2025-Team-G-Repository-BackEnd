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
    public class SalasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Salas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salas>>> GetSalas()
        {
            return await _context.Salas.ToListAsync();
        }

        // GET: api/Salas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Salas>> GetSalas(int id)
        {
            var salas = await _context.Salas.FindAsync(id);

            if (salas == null)
            {
                return NotFound();
            }

            return salas;
        }

        // PUT: api/Salas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalas(int id, Salas salas)
        {
            if (id != salas.Id)
            {
                return BadRequest();
            }

            _context.Entry(salas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalasExists(id))
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

        // POST: api/Salas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Salas>> PostSalas(Salas salas)
        {
            _context.Salas.Add(salas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalas", new { id = salas.Id }, salas);
        }

        // DELETE: api/Salas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalas(int id)
        {
            var salas = await _context.Salas.FindAsync(id);
            if (salas == null)
            {
                return NotFound();
            }

            _context.Salas.Remove(salas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SalasExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }
    }
}
