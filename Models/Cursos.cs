namespace BackEndHorario.Models {
    public class Cursos {
        public int Id { get; set; }
        public string CodigoCurso { get; set; }
        public string Nome { get; set; }

        public ICollection<Turmas> Turmas { get; set; }
        public ICollection<Unidades_Curriculares> UnidadesCurriculares { get; set; }
    }
}
