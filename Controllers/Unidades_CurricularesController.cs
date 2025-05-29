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
    public class Unidades_CurricularesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Unidades_CurricularesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Unidades_Curriculares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unidades_Curriculares>>> GetUnidadesCurriculares()
        {
            return await _context.UnidadesCurriculares.ToListAsync();
        }

        // GET: api/Unidades_Curriculares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Unidades_Curriculares>> GetUnidades_Curriculares(int id)
        {
            var unidades_Curriculares = await _context.UnidadesCurriculares.FindAsync(id);

            if (unidades_Curriculares == null)
            {
                return NotFound();
            }

            return unidades_Curriculares;
        }

        // PUT: api/Unidades_Curriculares/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnidades_Curriculares(int id, Unidades_Curriculares unidades_Curriculares)
        {
            if (id != unidades_Curriculares.Id)
            {
                return BadRequest();
            }

            _context.Entry(unidades_Curriculares).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Unidades_CurricularesExists(id))
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

        // POST: api/Unidades_Curriculares
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Unidades_Curriculares>> PostUnidades_Curriculares(Unidades_Curriculares unidades_Curriculares)
        {
            _context.UnidadesCurriculares.Add(unidades_Curriculares);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUnidades_Curriculares", new { id = unidades_Curriculares.Id }, unidades_Curriculares);
        }

        // DELETE: api/Unidades_Curriculares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnidades_Curriculares(int id)
        {
            var unidades_Curriculares = await _context.UnidadesCurriculares.FindAsync(id);
            if (unidades_Curriculares == null)
            {
                return NotFound();
            }

            _context.UnidadesCurriculares.Remove(unidades_Curriculares);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Unidades_CurricularesExists(int id)
        {
            return _context.UnidadesCurriculares.Any(e => e.Id == id);
        }
    }
}
