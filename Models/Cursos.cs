using System.ComponentModel.DataAnnotations;

namespace BackEndHorario.Models {
    public class Cursos {
        public int Id { get; set; }
       
        public required string CodigoCurso { get; set; }
        
        public required string Nome { get; set; }

        public ICollection<Turmas> Turmas { get; set; } = new List<Turmas>();
        public ICollection<Unidades_Curriculares> UnidadesCurriculares { get; set; } = new List<Unidades_Curriculares>();
    }
}
