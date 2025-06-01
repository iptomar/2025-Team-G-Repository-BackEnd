namespace BackEndHorario.Models {
    public class Blocos {
        public int Id { get; set; }
        public required string TipoAula { get; set; } // Ex: Teórica, Prática
        public int NumeroSlots { get; set; }

        public int UnidadeCurricularId { get; set; }
        public Unidades_Curriculares? UnidadeCurricular { get; set; }

        public int TurmaId { get; set; }
        public Turmas? Turma { get; set; }

        public int DocenteId { get; set; }
        public Docentes? Docente { get; set; }

        public int SalaId { get; set; }
        public Salas? Sala { get; set; }

        public int HorarioId { get; set; }
        public Horarios? Horario { get; set; }

        public int UtilizadorId { get; set; }
        public Utilizadores? Utilizador { get; set; }
        public int RepetirSemanas { get; set; } // Número de semanas que este bloco se repete

    }
}
