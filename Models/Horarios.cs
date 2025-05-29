namespace BackEndHorario.Models {
    public class Horarios {
        public int Id { get; set; }
        public required string AnoLetivo { get; set; }
        public int Semestre { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
