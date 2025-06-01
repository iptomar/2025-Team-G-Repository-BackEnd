namespace BackEndHorario.Models {
    public class Horarios {
        public int Id { get; set; }
        public required int Ano { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
