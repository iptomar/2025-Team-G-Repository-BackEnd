using System.ComponentModel.DataAnnotations;

namespace BackEndHorario.Models {
    public class Docentes {
        public int Id { get; set; }
        public required string Nome { get; set; }
        
        [EmailAddress]
        public required string Email { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
