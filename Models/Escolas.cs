namespace BackEndHorario.Models {
    public class Escolas {
        public int Id { get; set; }
        public required string Nome { get; set; }

        public ICollection<Salas> Salas { get; set; } = new List<Salas>();
    }
}
