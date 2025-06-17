namespace BackEndHorario.Models {
    public class BlocoDTO {
        public int Id { get; set; }
        public string TipoAula { get; set; } = string.Empty;
        public int NumeroSlots { get; set; }

        public string UnidadeCurricular { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public int RepetirSemanas { get; set; }

        // Novos campos para permitir PUT com dados reais
        public int UnidadeCurricularId { get; set; }
        public int TurmaId { get; set; }
        public int DocenteId { get; set; }
        public int SalaId { get; set; }
        public int HorarioId { get; set; }
        public int UtilizadorId { get; set; }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

    }
}