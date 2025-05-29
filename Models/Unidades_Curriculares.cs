namespace BackEndHorario.Models {
    public class Unidades_Curriculares {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Plano { get; set; }
        public int Semestre { get; set; }

        public int CursoId { get; set; }
        public Cursos Curso { get; set; }

        public ICollection<Blocos> Blocos { get; set; }
    }
}
