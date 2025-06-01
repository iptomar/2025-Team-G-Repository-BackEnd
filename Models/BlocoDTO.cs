namespace BackEndHorario.Models {
    public class BlocoDTO {
        public int Id { get; set; }
        public string TipoAula { get; set; } = string.Empty;
        public int NumeroSlots { get; set; }

        public string UnidadeCurricular { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public int RepetirSemanas { get; set; }

    }
}