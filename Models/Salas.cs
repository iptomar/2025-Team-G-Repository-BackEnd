﻿namespace BackEndHorario.Models {
    public class Salas {
        public int Id { get; set; }
        public required string Nome { get; set; }

        public int EscolaId { get; set; }
        public Escolas? Escola { get; set; }

        public ICollection<Blocos> Blocos { get; set; } = new List<Blocos>();
    }
}
