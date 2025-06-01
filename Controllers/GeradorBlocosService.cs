using BackEndHorario.Data;
using BackEndHorario.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndHorario.Services {
    public class GeradorBlocosService {
        private readonly ApplicationDbContext _context;

        public GeradorBlocosService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task GerarBlocosPadraoAsync() {
            const int semanasLetivas = 14;

            // Garante que existe pelo menos um utilizador
            if (!_context.Utilizadores.Any()) {
                _context.Utilizadores.Add(new Utilizadores {
                    Nome = "Admin",
                    Perfil = Utilizadores.PerfilUtilizador.Admin
                });

                await _context.SaveChangesAsync();
            }

            // Buscar o primeiro utilizador para associar aos blocos
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

                    // Cria 1 bloco para PL
                    if (uc.HorasPL > 0) {
                        int horasSemana = uc.HorasPL / semanasLetivas;
                        if (horasSemana > 0) {
                            int slots = horasSemana * 2;

                            var salaPL = salas.FirstOrDefault(s => s.Id == uc.SalaPLId);
                            if (salaPL == null) {
                                Console.WriteLine($"❌ Sala PL não encontrada para UC: {uc.Nome}");
                                continue;
                            }

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

                    // Cria 1 bloco para TP
                    if (uc.HorasTP > 0) {
                        int horasSemana = uc.HorasTP / semanasLetivas;
                        if (horasSemana > 0) {
                            int slots = horasSemana * 2;

                            var salaTP = salas.FirstOrDefault(s => s.Id == uc.SalaTPId);
                            if (salaTP == null) {
                                Console.WriteLine($"❌ Sala TP não encontrada para UC: {uc.Nome}");
                                continue;
                            }

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

            await _context.Blocos.AddRangeAsync(blocos);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ {blocos.Count} blocos gerados com sucesso.");
        }

    }
}
