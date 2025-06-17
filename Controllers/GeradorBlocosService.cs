using BackEndHorario.Data;
using BackEndHorario.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndHorario.Services {
    public class GeradorBlocosService {
        private readonly ApplicationDbContext _context;

        public GeradorBlocosService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task LimparBlocosAsync() {
            var countAntes = await _context.Blocos.CountAsync();
            Console.WriteLine($"🗑️ Blocos antes de limpar: {countAntes}");

            var todosBlocos = await _context.Blocos.ToListAsync();
            if (todosBlocos.Any()) {
                _context.Blocos.RemoveRange(todosBlocos);
                await _context.SaveChangesAsync();
                Console.WriteLine("🗑️ Todos os blocos foram removidos da base de dados.");
            }
            else {
                Console.WriteLine("ℹ️ Não havia blocos para remover.");
            }

            var countDepois = await _context.Blocos.CountAsync();
            Console.WriteLine($"🗑️ Blocos após limpar: {countDepois}");
        }

        public async Task GerarBlocosPadraoAsync() {
            const int semanasLetivas = 14;

            if (!_context.Utilizadores.Any()) {
                _context.Utilizadores.Add(new Utilizadores {
                    Nome = "Admin",
                    Perfil = Utilizadores.PerfilUtilizador.Admin
                });

                await _context.SaveChangesAsync();
            }

            var utilizador = await _context.Utilizadores.FirstAsync();

            var turmas = _context.Turmas.ToList();
            var ucs = _context.UnidadesCurriculares.ToList();
            var docentes = _context.Docentes.ToList();
            var salas = _context.Salas.ToList();
            var horarios = _context.Horarios.ToList();

            Console.WriteLine($"Turmas: {turmas.Count}, UCs: {ucs.Count}, Horários: {horarios.Count}");

            var blocos = new List<Blocos>();

            foreach (var turma in turmas) {
                foreach (var uc in ucs.Where(u => u.CursoId == turma.CursoId && u.Ano == turma.Ano)) {
                    var docente = docentes.FirstOrDefault();
                    if (docente == null) continue;

                    var horario = horarios.FirstOrDefault(h => h.Ano == uc.Ano);
                    if (horario == null) {
                        Console.WriteLine($"❌ Horário não encontrado para Ano {uc.Ano}, UC: {uc.Nome}");
                        continue;
                    }

                    // Criar bloco PL, se aplicável
                    if (uc.HorasPL > 0) {
                        int horasSemana = uc.HorasPL / semanasLetivas;
                        if (horasSemana > 0) {
                            int slots = horasSemana * 2;

                            var salaPL = salas.FirstOrDefault(s => s.Id == uc.SalaPLId);
                            if (salaPL == null) {
                                Console.WriteLine($"❌ Sala PL não encontrada para UC: {uc.Nome}");
                                continue;
                            }

                            // Verifica se bloco já existe
                            bool blocoPLExiste = await _context.Blocos.AnyAsync(b =>
                                b.TipoAula == "PL" &&
                                b.UnidadeCurricularId == uc.Id &&
                                b.TurmaId == turma.Id &&
                                b.DocenteId == docente.Id &&
                                b.SalaId == salaPL.Id &&
                                b.HorarioId == horario.Id
                            );

                            if (!blocoPLExiste) {
                                var blocoPL = new Blocos {
                                    TipoAula = "PL",
                                    NumeroSlots = slots,
                                    UnidadeCurricularId = uc.Id,
                                    TurmaId = turma.Id,
                                    DocenteId = docente.Id,
                                    SalaId = salaPL.Id,
                                    HorarioId = horario.Id,
                                    UtilizadorId = utilizador.Id,
                                    RepetirSemanas = semanasLetivas
                                };

                                blocos.Add(blocoPL);
                                Console.WriteLine($"🟩 Bloco PL gerado: {uc.Nome} - {slots} slots x {semanasLetivas} semanas");
                            }
                        }
                    }

                    // Criar bloco TP, se aplicável
                    if (uc.HorasTP > 0) {
                        int horasSemana = uc.HorasTP / semanasLetivas;
                        if (horasSemana > 0) {
                            int slots = horasSemana * 2;

                            var salaTP = salas.FirstOrDefault(s => s.Id == uc.SalaTPId);
                            if (salaTP == null) {
                                Console.WriteLine($"❌ Sala TP não encontrada para UC: {uc.Nome}");
                                continue;
                            }

                            // Verifica se bloco já existe
                            bool blocoTPExiste = await _context.Blocos.AnyAsync(b =>
                                b.TipoAula == "TP" &&
                                b.UnidadeCurricularId == uc.Id &&
                                b.TurmaId == turma.Id &&
                                b.DocenteId == docente.Id &&
                                b.SalaId == salaTP.Id &&
                                b.HorarioId == horario.Id
                            );

                            if (!blocoTPExiste) {
                                var blocoTP = new Blocos {
                                    TipoAula = "TP",
                                    NumeroSlots = slots,
                                    UnidadeCurricularId = uc.Id,
                                    TurmaId = turma.Id,
                                    DocenteId = docente.Id,
                                    SalaId = salaTP.Id,
                                    HorarioId = horario.Id,
                                    UtilizadorId = utilizador.Id,
                                    RepetirSemanas = semanasLetivas
                                };

                                blocos.Add(blocoTP);
                                Console.WriteLine($"🟦 Bloco TP gerado: {uc.Nome} - {slots} slots x {semanasLetivas} semanas");
                            }
                        }
                    }
                }
            }

            if (blocos.Count > 0) {
                try {
                    await _context.Blocos.AddRangeAsync(blocos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UX_Blocos_UnidadeTurmaTipoDocenteSalaHorario")) {
                        Console.WriteLine("⚠️ Tentativa de inserir blocos duplicados evitada pelo índice único.");
                    }
                    else {
                        throw;
                    }
                }
            }
            else {
                Console.WriteLine("ℹ️ Nenhum bloco novo para gerar.");
            }
        }

    }
}
