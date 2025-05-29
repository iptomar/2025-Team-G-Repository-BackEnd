namespace BackEndHorario.Models {
    public class Utilizadores {
        public int Id { get; set; }
        public string Nome { get; set; }
        public PerfilUtilizador Perfil { get; set; }
        public enum PerfilUtilizador { Admin, ComissaoEscola, ComissaoCurso }
        public ICollection<Blocos> Blocos { get; set; }
    }
}
