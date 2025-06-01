namespace BackEndHorario.Models {
    public class Unidades_Curriculares {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Plano { get; set; }
        public int Semestre { get; set; }
        public int Ano { get; set; }
        public string? DocentePL { get; set; }  // Ex: "L. Merca / C. Perq."
        public string? DocenteTP { get; set; }  // Ex: "João / Maria"
        public int HorasTP { get; set; }
        public int HorasPL { get; set; }

        public int? SalaPLId { get; set; }
        public Salas? SalaPL { get; set; }

        public int? SalaTPId { get; set; }
        public Salas? SalaTP { get; set; }

        public int CursoId { get; set; }
        public Cursos? Curso { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
