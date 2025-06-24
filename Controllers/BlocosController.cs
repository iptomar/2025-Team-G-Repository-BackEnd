using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEndHorario.Data;
using BackEndHorario.Models;
using Microsoft.AspNetCore.SignalR;
using BackEndHorario.Hubs;

namespace BackEndHorario.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BlocosController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<HorarioHub> _hubContext;

        public BlocosController(ApplicationDbContext context, IHubContext<HorarioHub> hubContext) {
            _context = context;
            _hubContext = hubContext;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlocoDTO>>> GetBlocos() {
            var blocos = await _context.Blocos
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Docente)
                .Include(b => b.Sala)
                .ToListAsync();

            var dtoList = blocos.Select(b => new BlocoDTO {
                Id = b.Id,
                TipoAula = b.TipoAula,
                NumeroSlots = b.NumeroSlots,
                UnidadeCurricular = b.UnidadeCurricular != null && !string.IsNullOrWhiteSpace(b.UnidadeCurricular.Nome)
                    ? AbreviarNome(b.UnidadeCurricular.Nome)
                    : "(UC inválida)",
                Docente = b.UnidadeCurricular != null
                    ? FormatarDocentes(b.UnidadeCurricular.DocentePL, b.UnidadeCurricular.DocenteTP)
                    : "(UC inválida)",
                Sala = b.Sala != null && !string.IsNullOrWhiteSpace(b.Sala.Nome)
                    ? b.Sala.Nome
                    : "(Sala inválida)",
                RepetirSemanas = b.RepetirSemanas,
                UnidadeCurricularId = b.UnidadeCurricularId,
                TurmaId = b.TurmaId,
                DocenteId = b.DocenteId,
                SalaId = b.SalaId,
                HorarioId = b.HorarioId,
                UtilizadorId = b.UtilizadorId
            }).ToList();

            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<ActionResult<BlocoDTO>> PostBloco([FromBody] BlocoDTO dto) {
            if (dto == null)
                return BadRequest("DTO nulo.");

            var bloco = new Blocos {
                TipoAula = dto.TipoAula,
                NumeroSlots = dto.NumeroSlots,
                UnidadeCurricularId = dto.UnidadeCurricularId,
                TurmaId = dto.TurmaId,
                DocenteId = dto.DocenteId,
                SalaId = dto.SalaId,
                HorarioId = dto.HorarioId,
                UtilizadorId = dto.UtilizadorId,
                RepetirSemanas = dto.RepetirSemanas
            };

            bool blocoExiste = await _context.Blocos.AnyAsync(b =>
                b.TipoAula == bloco.TipoAula &&
                b.UnidadeCurricularId == bloco.UnidadeCurricularId &&
                b.TurmaId == bloco.TurmaId &&
                b.DocenteId == bloco.DocenteId &&
                b.SalaId == bloco.SalaId &&
                b.HorarioId == bloco.HorarioId
            );

            if (blocoExiste) {
                return Conflict("⚠️ Bloco já existe na base de dados.");
            }

            try {
                _context.Blocos.Add(bloco);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) {
                if (ex.InnerException?.Message.Contains("UX_Blocos_UnidadeTurmaTipoDocenteSalaHorario") == true) {
                    return Conflict("⚠️ Bloco duplicado detectado (índice único).");
                }
                throw;
            }

            var blocoCompleto = await _context.Blocos
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Sala)
                .FirstOrDefaultAsync(b => b.Id == bloco.Id);

            if (blocoCompleto == null)
                return NotFound("Bloco criado não encontrado.");

            var blocoDTO = new BlocoDTO {
                Id = blocoCompleto.Id,
                TipoAula = blocoCompleto.TipoAula,
                NumeroSlots = blocoCompleto.NumeroSlots,
                UnidadeCurricular = blocoCompleto.UnidadeCurricular != null
                    ? AbreviarNome(blocoCompleto.UnidadeCurricular.Nome)
                    : "(UC inválida)",
                Docente = blocoCompleto.UnidadeCurricular != null
                    ? FormatarDocentes(blocoCompleto.UnidadeCurricular.DocentePL, blocoCompleto.UnidadeCurricular.DocenteTP)
                    : "(UC inválida)",
                Sala = blocoCompleto.Sala?.Nome ?? "(Sala inválida)",
                RepetirSemanas = blocoCompleto.RepetirSemanas,
                UnidadeCurricularId = blocoCompleto.UnidadeCurricularId,
                TurmaId = blocoCompleto.TurmaId,
                DocenteId = blocoCompleto.DocenteId,
                SalaId = blocoCompleto.SalaId,
                HorarioId = blocoCompleto.HorarioId,
                UtilizadorId = blocoCompleto.UtilizadorId,
                Start = dto.Start,
                End = dto.End
            };

            Console.WriteLine("📤 Enviando BlocoAdicionado via SignalR");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(blocoDTO));
            Console.WriteLine($"📤 Enviando bloco via SignalR: Id={blocoDTO.Id} Tipo={blocoDTO.TipoAula}");

            await _hubContext.Clients.All.SendAsync("BlocoAdicionado", blocoDTO);

            return CreatedAtAction(nameof(GetBlocos), new { id = blocoDTO.Id }, blocoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBloco(int id, [FromBody] BlocoDTO dto) {
            Console.WriteLine("🎯 ENTROU no PUT");

            var blocoNaDb = await _context.Blocos.FindAsync(id);
            if (blocoNaDb == null) {
                Console.WriteLine("❌ Bloco com ID não encontrado na base de dados");
                return NotFound();
            }

            // Atualizar propriedades permitidaas
            blocoNaDb.NumeroSlots = dto.NumeroSlots;
            blocoNaDb.RepetirSemanas = dto.RepetirSemanas;
            blocoNaDb.TipoAula = dto.TipoAula;
            blocoNaDb.UnidadeCurricularId = dto.UnidadeCurricularId;
            blocoNaDb.TurmaId = dto.TurmaId;
            blocoNaDb.DocenteId = dto.DocenteId;
            blocoNaDb.SalaId = dto.SalaId;
            blocoNaDb.HorarioId = dto.HorarioId;
            blocoNaDb.UtilizadorId = dto.UtilizadorId;

            try {
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ SaveChangesAsync executado com sucesso");

                var blocoAtualizado = await _context.Blocos
                    .Include(b => b.UnidadeCurricular)
                    .Include(b => b.Sala)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (blocoAtualizado == null)
                    return NotFound("Bloco atualizado não encontrado.");

                var blocoDTO = new BlocoDTO {
                    Id = blocoAtualizado.Id,
                    TipoAula = blocoAtualizado.TipoAula,
                    NumeroSlots = blocoAtualizado.NumeroSlots,
                    UnidadeCurricular = blocoAtualizado.UnidadeCurricular != null
                        ? AbreviarNome(blocoAtualizado.UnidadeCurricular.Nome)
                        : "(UC inválida)",
                    Docente = blocoAtualizado.UnidadeCurricular != null
                        ? FormatarDocentes(blocoAtualizado.UnidadeCurricular.DocentePL, blocoAtualizado.UnidadeCurricular.DocenteTP)
                        : "(UC inválida)",
                    Sala = blocoAtualizado.Sala?.Nome ?? "(Sala inválida)",
                    RepetirSemanas = blocoAtualizado.RepetirSemanas,
                    UnidadeCurricularId = blocoAtualizado.UnidadeCurricularId,
                    TurmaId = blocoAtualizado.TurmaId,
                    DocenteId = blocoAtualizado.DocenteId,
                    SalaId = blocoAtualizado.SalaId,
                    HorarioId = blocoAtualizado.HorarioId,
                    UtilizadorId = blocoAtualizado.UtilizadorId,
                    Start = dto.Start,
                    End = dto.End
                };

                Console.WriteLine("📤 Enviando BlocoAtualizado via SignalR");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(blocoDTO));

                await _hubContext.Clients.All.SendAsync("BlocoAtualizado", blocoDTO);
            }
            catch (Exception ex) {
                Console.WriteLine("❌ Erro ao fazer PUT:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Erro ao atualizar bloco");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBloco(int id) {
            var bloco = await _context.Blocos
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Sala)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bloco == null)
                return NotFound();

            _context.Blocos.Remove(bloco);
            await _context.SaveChangesAsync();

            var blocoDTO = new BlocoDTO {
                Id = bloco.Id,
                TipoAula = bloco.TipoAula,
                NumeroSlots = bloco.NumeroSlots,
                UnidadeCurricular = bloco.UnidadeCurricular != null
                    ? AbreviarNome(bloco.UnidadeCurricular.Nome)
                    : "(UC inválida)",
                Docente = bloco.UnidadeCurricular != null
                    ? FormatarDocentes(bloco.UnidadeCurricular.DocentePL, bloco.UnidadeCurricular.DocenteTP)
                    : "(UC inválida)",
                Sala = bloco.Sala?.Nome ?? "(Sala inválida)",
                RepetirSemanas = bloco.RepetirSemanas,
                UnidadeCurricularId = bloco.UnidadeCurricularId,
                TurmaId = bloco.TurmaId,
                DocenteId = bloco.DocenteId,
                SalaId = bloco.SalaId,
                HorarioId = bloco.HorarioId,
                UtilizadorId = bloco.UtilizadorId
            };

            Console.WriteLine("🗑️ Bloco removido, emitindo BlocoRemovido via SignalR");
            await _hubContext.Clients.All.SendAsync("BlocoRemovido", blocoDTO);

            return NoContent();
        }

        [HttpPost("limpar-alocacoes")]
        public async Task<IActionResult> LimparAlocacoes() {
            var blocos = await _context.Blocos
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Sala)
                .ToListAsync();

            foreach (var bloco in blocos) {
                var blocoDTO = new BlocoDTO {
                    Id = bloco.Id,
                    TipoAula = bloco.TipoAula,
                    NumeroSlots = bloco.NumeroSlots,
                    UnidadeCurricular = bloco.UnidadeCurricular != null
                        ? AbreviarNome(bloco.UnidadeCurricular.Nome)
                        : "(UC inválida)",
                    Docente = bloco.UnidadeCurricular != null
                        ? FormatarDocentes(bloco.UnidadeCurricular.DocentePL, bloco.UnidadeCurricular.DocenteTP)
                        : "(UC inválida)",
                    Sala = bloco.Sala?.Nome ?? "(Sala inválida)",
                    RepetirSemanas = bloco.RepetirSemanas,
                    UnidadeCurricularId = bloco.UnidadeCurricularId,
                    TurmaId = bloco.TurmaId,
                    DocenteId = bloco.DocenteId,
                    SalaId = bloco.SalaId,
                    HorarioId = bloco.HorarioId,
                    UtilizadorId = bloco.UtilizadorId
                };

                // Reset das datas
                blocoDTO.Start = null;
                blocoDTO.End = null;

                await _hubContext.Clients.All.SendAsync("BlocoRemovido", blocoDTO);
            }

            return NoContent();
        }
    }
}
