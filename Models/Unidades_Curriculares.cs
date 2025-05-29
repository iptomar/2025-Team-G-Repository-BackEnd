namespace BackEndHorario.Models {
    public class Unidades_Curriculares {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Plano { get; set; }
        public int Semestre { get; set; }

        public int CursoId { get; set; }
        public Cursos? Curso { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
