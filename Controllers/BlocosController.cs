using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEndHorario.Data;
using BackEndHorario.Models;

namespace BackEndHorario.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BlocosController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public BlocosController(ApplicationDbContext context) {
            _context = context;
        }

        private string AbreviarNome(string? nome) {
            if (string.IsNullOrWhiteSpace(nome))
                return "(UC sem nome)";

            var abreviado = nome;

            try {
                abreviado = abreviado.Replace("Análise Matemática II", "An. Matemática II")
                    .Replace("Laboratório de Microssistemas", "Lab. Microssist.")
                    .Replace("Introdução à Programação Web", "Int. à Prog. Web")
                    .Replace("Matemática Computacional", "Mat. Computac.")
                    .Replace("Programação Orientada a Objectos", "Prog. Orient. Obj.")
                    .Replace("Bases de Dados Avançadas", "Bases Dados Av.")
                    .Replace("Tópicos de Gestão de Empresas", "Tóp. Gest. Emp.")
                    .Replace("Sistemas Inteligentes", "Sist. Inteligentes")
                    .Replace("Desenvolvimento Web", "Desenvolv. Web")
                    .Replace("Sistemas Operativos", "Sistemas Operat.")
                    .Replace("Sistemas de Informação nas Organizações", "Sist. Inf. nas Org.")
                    .Replace("Desenvolvimento e Operações", "Desenv. Operações")
                    .Replace("Gestão de Projetos", "Gestão de Proj.")
                    .Replace("Análise Matemática I", "Anál. Matemática I")
                    .Replace("Introdução à Programação e à Resolução de Problemas", "Int. Prog. Res. Prob.")
                    .Replace("Introdução à Engenharia e à Tecnologia", "Intr. à Eng. e à Tec.")
                    .Replace("Estruturas de Dados e Algoritmos", "Est. Dados e Alg.")
                    .Replace("Arquitetura de Computadores", "Arq. de Comput.")
                    .Replace("Probabilidades e Estatística", "Prob. Estatística")
                    .Replace("Infraestruturas de Redes Locais", "Inf. Redes Locais")
                    .Replace("Computação Distribuída", "Comp. Distribuída")
                    .Replace("Segurança Informática", "Seg. Informática")
                    .Replace("Engenharia de Software", "Eng. Software")
                    .Replace("Desenvolvimento de Aplicações Móveis", "Des. Ap. Móveis");
            }
            catch {
                return "(Erro ao abreviar)";
            }

            return abreviado;
        }

        // ✅ GET: api/blocos (com DTO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlocoDTO>>> GetBlocos() {
            var blocos = await _context.Blocos
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Docente)
                .Include(b => b.Sala)
                .ToListAsync();

            var dtoList = blocos.Select(b => {
                var uc = b.UnidadeCurricular;
                var sala = b.Sala;

                return new BlocoDTO {
                    Id = b.Id,
                    TipoAula = b.TipoAula,
                    NumeroSlots = b.NumeroSlots,
                    UnidadeCurricular = uc != null && !string.IsNullOrWhiteSpace(uc.Nome)
                        ? AbreviarNome(uc.Nome)
                        : "(UC inválida)",
                    Docente = uc != null
                        ? FormatarDocentes(uc.DocentePL, uc.DocenteTP)
                        : "(UC inválida)",
                    Sala = sala != null && !string.IsNullOrWhiteSpace(sala.Nome)
                        ? sala.Nome
                        : "(Sala inválida)"
                };
            }).ToList();

            return Ok(dtoList);
        }

        private string FormatarDocentes(string? pl, string? tp) {
            if (string.IsNullOrWhiteSpace(pl) && string.IsNullOrWhiteSpace(tp))
                return "(Sem docente)";

            if (!string.IsNullOrWhiteSpace(pl) && pl == tp)
                return pl;

            if (!string.IsNullOrWhiteSpace(pl) && string.IsNullOrWhiteSpace(tp))
                return pl;

            if (string.IsNullOrWhiteSpace(pl) && !string.IsNullOrWhiteSpace(tp))
                return tp;

            return $"{pl} / {tp}";
        }

        // GET: api/blocos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blocos>> GetBloco(int id) {
            var bloco = await _context.Blocos
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Docente)
                .Include(b => b.Sala)
                .Include(b => b.Turma)
                .Include(b => b.Horario)
                .Include(b => b.Utilizador)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bloco == null)
                return NotFound();

            return bloco;
        }

        // POST: api/blocos
        [HttpPost]
        public async Task<ActionResult<Blocos>> PostBloco(Blocos bloco) {
            // Verifica se já existe um bloco idêntico
            var blocoExistente = await _context.Blocos.FirstOrDefaultAsync(b =>
                b.TurmaId == bloco.TurmaId &&
                b.UnidadeCurricularId == bloco.UnidadeCurricularId &&
                b.DocenteId == bloco.DocenteId &&
                b.SalaId == bloco.SalaId
            );

            if (blocoExistente != null) {
                // Já existe, retorna o mesmo bloco (evita duplicar)
                return Ok(blocoExistente);
            }

            _context.Blocos.Add(bloco);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBloco), new { id = bloco.Id }, bloco);
        }

        // PUT: api/blocos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBloco(int id, Blocos bloco) {
            if (id != bloco.Id)
                return BadRequest();

            _context.Entry(bloco).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_context.Blocos.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/blocos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBloco(int id) {
            var bloco = await _context.Blocos.FindAsync(id);
            if (bloco == null)
                return NotFound();

            _context.Blocos.Remove(bloco);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
