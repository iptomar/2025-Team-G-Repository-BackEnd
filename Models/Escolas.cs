namespace BackEndHorario.Models {
    public class Escolas {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<Salas> Salas { get; set; }
    }
}
