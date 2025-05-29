namespace BackEndHorario.Models {
    public class Docentes {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public ICollection<Blocos> Blocos { get; set; }
    }
}
