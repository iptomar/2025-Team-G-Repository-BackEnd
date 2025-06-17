using Microsoft.AspNetCore.SignalR;

namespace BackEndHorario.Hubs {
    public class HorarioHub : Hub {
        // Método para notificar todos os outros clientes (menos o emissor)
        public async Task EnviarBlocoAtualizado(object blocoDTO) {
            await Clients.Others.SendAsync("BlocoAtualizado", blocoDTO);
        }

        public async Task RemoverBloco(string blocoId) {
            await Clients.Others.SendAsync("BlocoRemovido", blocoId);
        }
    }
}
