using System.Text.Json.Serialization;

namespace BackEndHorario.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PerfilUtilizador
    {
        Admin,
        ComissaoEscola,
        ComissaoCurso
    }

    public class Utilizadores
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public PerfilUtilizador Perfil { get; set; }

        public int? EscolaId { get; set; }
        public Escolas? Escola { get; set; }

        public int? CursoId { get; set; }
        public Cursos? Curso { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}