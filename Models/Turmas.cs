namespace BackEndHorario.Models {
    public class Turmas {
        public int Id { get; set; }
        public int Ano { get; set; }
        public required string Nome { get; set; }

        public int CursoId { get; set; }
        public Cursos? Curso { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
