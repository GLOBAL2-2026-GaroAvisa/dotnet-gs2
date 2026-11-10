using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface ICompromissoRepository
    {
        Task<List<Compromisso>> ListarPorLocaisAsync(List<int> localIds);
        Task<Compromisso?> BuscarPorIdAsync(int id);
        Task<Compromisso> CriarAsync(Compromisso compromisso);
        Task AtualizarAsync(Compromisso compromisso);
        Task ExcluirAsync(Compromisso compromisso);
    }
}