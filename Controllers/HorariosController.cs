﻿using System;
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
    public class HorariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HorariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Horarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Horarios>>> GetHorarios()
        {
            return await _context.Horarios.ToListAsync();
        }

        // GET: api/Horarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Horarios>> GetHorarios(int id)
        {
            var horarios = await _context.Horarios.FindAsync(id);

            if (horarios == null)
            {
                return NotFound();
            }

            return horarios;
        }

        // PUT: api/Horarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHorarios(int id, Horarios horarios)
        {
            if (id != horarios.Id)
            {
                return BadRequest();
            }

            _context.Entry(horarios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HorariosExists(id))
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

        // POST: api/Horarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Horarios>> PostHorarios(Horarios horarios)
        {
            _context.Horarios.Add(horarios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHorarios", new { id = horarios.Id }, horarios);
        }

        // DELETE: api/Horarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHorarios(int id)
        {
            var horarios = await _context.Horarios.FindAsync(id);
            if (horarios == null)
            {
                return NotFound();
            }

            _context.Horarios.Remove(horarios);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HorariosExists(int id)
        {
            return _context.Horarios.Any(e => e.Id == id);
        }
    }
}
