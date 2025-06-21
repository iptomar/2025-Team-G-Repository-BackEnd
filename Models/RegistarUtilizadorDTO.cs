using static BackEndHorario.Models.PerfilUtilizador;

namespace BackEndHorario.Models
{
    public class RegistarUtilizadorDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? EscolaId { get; set; }
        public int? CursoId { get; set; }
        public PerfilUtilizador Perfil { get; set; }
    }
}