using BackEndHorario.Data;
using BackEndHorario.Models;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ImportadorExcelService {
    private readonly ApplicationDbContext _context;

    public ImportadorExcelService(ApplicationDbContext context) {
        _context = context;
    }

    public async Task ImportarUCsAsync(string caminhoFicheiro) {
        try {
            using var workbook = new XLWorkbook(caminhoFicheiro);
            var worksheet = workbook.Worksheet("UCs");
            int adicionadas = 0;

            foreach (var row in worksheet.RowsUsed().Skip(1)) {
                var nome = row.Cell(1).GetString();
                var codigo = row.Cell(2).GetString();
                var codigoCurso = row.Cell(3).GetString();
                var horasPL = row.Cell(4).GetValue<int>();
                var horasTP = row.Cell(5).GetValue<int>();
                var docentePL = row.Cell(6).GetString(); // Agora contém nomes
                var docenteTP = row.Cell(7).GetString(); // Agora contém nomes
                var ano = row.Cell(8).GetValue<int>();
                var semestre = row.Cell(9).GetValue<int>();
                var nomeSalaPL = row.Cell(10).GetString().Trim(); // Nome_Sala_PL
                var nomeSalaTP = row.Cell(11).GetString().Trim(); // Nome_Sala_TP

                var salaPL = !string.IsNullOrWhiteSpace(nomeSalaPL)
                    ? _context.Salas.FirstOrDefault(s => s.Nome == nomeSalaPL)
                    : null;

                var salaTP = !string.IsNullOrWhiteSpace(nomeSalaTP)
                    ? _context.Salas.FirstOrDefault(s => s.Nome == nomeSalaTP)
                    : null;

                var curso = _context.Cursos.FirstOrDefault(c => c.CodigoCurso == codigoCurso);
                if (curso == null) {
                    Console.WriteLine($"⚠️ Curso não encontrado para código: {codigoCurso}");
                    continue;
                }

                if (!_context.UnidadesCurriculares.Any(u =>
                    u.Nome == nome && u.CursoId == curso.Id && u.Semestre == semestre && u.Ano == ano)) {
                    var uc = new Unidades_Curriculares {
                        Nome = nome,
                        Plano = "PL+TP",
                        Semestre = semestre,
                        Ano = ano,
                        CursoId = curso.Id,
                        HorasPL = horasPL,
                        HorasTP = horasTP,
                        DocentePL = docentePL,
                        DocenteTP = docenteTP,
                        SalaPLId = salaPL?.Id,
                        SalaTPId = salaTP?.Id
                    };

                    _context.UnidadesCurriculares.Add(uc);
                    adicionadas++;
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Total de UCs adicionadas: {adicionadas}");
        }
        catch (Exception ex) {
            Console.WriteLine($"❌ Erro ao importar UCs: {ex.Message}");
        }
    }

    public async Task ImportarCursosAsync(string caminhoFicheiro) {
        try {
            using var workbook = new XLWorkbook(caminhoFicheiro);
            var worksheet = workbook.Worksheet("Cursos");

            foreach (var row in worksheet.RowsUsed().Skip(1)) {
                var codigo = row.Cell(1).GetString();
                var nome = row.Cell(2).GetString();
                if (!_context.Cursos.Any(c => c.CodigoCurso == codigo)) {
                    var curso = new Cursos {
                        Nome = nome,
                        CodigoCurso = codigo
                    };
                    _context.Cursos.Add(curso);
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Importação de cursos concluída.");
        }
        catch (Exception ex) {
            Console.WriteLine($"Erro ao importar cursos: {ex.Message}");
        }
    }

    public async Task ImportarDocentesAsync(string caminhoFicheiro) {
        using var workbook = new XLWorkbook(caminhoFicheiro);
        var worksheet = workbook.Worksheet("Docentes");

        foreach (var row in worksheet.RowsUsed().Skip(1)) {
            var nome = row.Cell(2).GetString().Trim();   // Coluna B
            var email = row.Cell(3).GetString().Trim();  // Coluna C

            if (await _context.Docentes.AnyAsync(d => d.Email == email)) {
                Console.WriteLine($"⚠️ Docente já existente: {email}");
                continue;
            }

            var docente = new Docentes {
                Nome = nome,
                Email = email
            };

            _context.Docentes.Add(docente);
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Importação de docentes concluída.");
    }

    public async Task ImportarEscolasAsync(string caminhoFicheiro) {
        using var workbook = new XLWorkbook(caminhoFicheiro);
        var worksheet = workbook.Worksheet("Escolas");

        foreach (var row in worksheet.RowsUsed().Skip(1)) {
            var nome = row.Cell(2).GetString().Trim(); // Coluna B — "Escola"

            if (_context.Escolas.Any(e => e.Nome == nome)) {
                Console.WriteLine($"⚠️ Escola já existente: {nome}");
                continue;
            }

            var escola = new Escolas {
                Nome = nome
            };

            _context.Escolas.Add(escola);
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Importação de escolas concluída.");
    }

    public async Task ImportarSalasAsync(string caminhoFicheiro) {
        using var workbook = new XLWorkbook(caminhoFicheiro);
        var worksheet = workbook.Worksheet("Salas");

        foreach (var row in worksheet.RowsUsed().Skip(1)) {
            var id = row.Cell(1).GetValue<int>();
            var nome = row.Cell(2).GetString();
            var escolaId = row.Cell(3).GetValue<int>();

            // Verifica se a escola existe
            var escola = await _context.Escolas.FindAsync(escolaId);
            if (escola == null) {
                Console.WriteLine($"❌ Escola com ID {escolaId} não encontrada para sala {nome}");
                continue;
            }

            // Verifica se já existe uma sala com o mesmo ID
            if (_context.Salas.Any(s => s.Id == id)) {
                Console.WriteLine($"⚠️ Sala com ID {id} já existe. Ignorada.");
                continue;
            }

            var sala = new Salas {
                Nome = nome,
                EscolaId = escolaId
            };

            _context.Salas.Add(sala);
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Importação de salas concluída.");
    }

    public async Task ImportarTurmasAsync(string caminhoFicheiro) {
        using var workbook = new XLWorkbook(caminhoFicheiro);
        var worksheet = workbook.Worksheet("Turmas");

        foreach (var row in worksheet.RowsUsed().Skip(1)) {
            var nome = row.Cell(1).GetString();            // Coluna A — Nome
            var ano = row.Cell(2).GetValue<int>();         // Coluna B — Ano
            var codigoCurso = row.Cell(3).GetString();     // Coluna C — CodigoCurso

            var curso = _context.Cursos.FirstOrDefault(c => c.CodigoCurso == codigoCurso);
            if (curso == null) {
                Console.WriteLine($"❌ Curso não encontrado para código: {codigoCurso}");
                continue;
            }

            // Evita duplicados
            if (_context.Turmas.Any(t => t.Nome == nome && t.Ano == ano && t.CursoId == curso.Id)) {
                Console.WriteLine($"⚠️ Turma já existente: {nome} ({ano})");
                continue;
            }

            var turma = new Turmas {
                Nome = nome,
                Ano = ano,
                CursoId = curso.Id
            };

            _context.Turmas.Add(turma);
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Importação de turmas concluída.");
    }

    public async Task ImportarHorariosAsync(string caminhoFicheiro) {
        using var workbook = new XLWorkbook(caminhoFicheiro);
        var worksheet = workbook.Worksheet("Horarios");

        foreach (var row in worksheet.RowsUsed().Skip(1)) {
            var ano = row.Cell(2).GetValue<int>();

            if (!_context.Horarios.Any(h => h.Ano == ano)) {
                var horario = new Horarios {
                    Ano = ano
                };
                _context.Horarios.Add(horario);
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("Importação de horários concluída.");
    }
}
