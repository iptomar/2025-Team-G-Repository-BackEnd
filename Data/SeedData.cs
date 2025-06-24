using BackEndHorario.Data;
using BackEndHorario.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndHorario {
    public static class SeedData {
        public static void Inicializar(IServiceProvider serviceProvider) {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (!context.Utilizadores.Any(u => u.Email == "admin@sistema.com")) {
                var admin = new Utilizadores {
                    Nome = "Admin",
                    Email = "admin@sistema.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123Qwe!"),
                    Perfil = PerfilUtilizador.Admin

                };

                context.Utilizadores.Add(admin);
                context.SaveChanges();
                Console.WriteLine("✅ Utilizador Admin criado");
            }
        }
    }
}