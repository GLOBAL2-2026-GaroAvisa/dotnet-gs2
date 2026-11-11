using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Application.Services
{
    public class LocalService : ILocalService
    {
        private readonly ILocalRepository _repository;

        public LocalService(ILocalRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<LocalRespostaDto>> ListarAsync(int usuarioId)
        {
            var locais = await _repository.ListarAsync(usuarioId);
            return locais.Select(ParaResposta).ToList();
        }

        public async Task<LocalRespostaDto?> BuscarPorIdAsync(int id, int usuarioId)
        {
            var local = await _repository.BuscarPorIdAsync(id, usuarioId);
            return local is null ? null : ParaResposta(local);
        }

        public async Task<LocalRespostaDto> CriarAsync(CriarLocalDto dto, int usuarioId)
        {
            var apelido = dto.Apelido.Trim();

            if (await _repository.ApelidoExisteAsync(apelido, usuarioId))
            {
                throw new InvalidOperationException("Você já tem um local com esse apelido. Escolha outro apelido.");
            }

            if (dto.Principal)
            {
                await _repository.RemoverPrincipalAsync(usuarioId);
            }

            var local = new Local
            {
                UsuarioId = usuarioId,
                Apelido = apelido,
                Nome = dto.Nome.Trim(),
                Estado = dto.Estado,
                Pais = dto.Pais,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Fuso = dto.Fuso,
                Principal = dto.Principal,
                ReceberAlertas = dto.ReceberAlertas
            };

            var criado = await _repository.CriarAsync(local);
            return ParaResposta(criado);
        }

        public async Task<LocalRespostaDto?> AtualizarAsync(int id, AtualizarLocalDto dto, int usuarioId)
        {
            var local = await _repository.BuscarPorIdAsync(id, usuarioId);
            if (local is null)
            {
                return null;
            }

            var apelido = dto.Apelido.Trim();

            if (await _repository.ApelidoExisteAsync(apelido, usuarioId, id))
            {
                throw new InvalidOperationException("Você já tem um local com esse apelido. Escolha outro apelido.");
            }

            if (dto.Principal)
            {
                await _repository.RemoverPrincipalAsync(usuarioId, id);
            }

            local.Apelido = apelido;
            local.Principal = dto.Principal;
            local.ReceberAlertas = dto.ReceberAlertas;

            await _repository.AtualizarAsync(local);
            return ParaResposta(local);
        }

        public async Task<bool> ExcluirAsync(int id, int usuarioId)
        {
            var local = await _repository.BuscarPorIdAsync(id, usuarioId);
            if (local is null)
            {
                return false;
            }

            await _repository.ExcluirAsync(local);
            return true;
        }

        private static LocalRespostaDto ParaResposta(Local local)
        {
            return new LocalRespostaDto
            {
                Id = local.Id,
                Apelido = local.Apelido,
                Nome = local.Nome,
                Estado = local.Estado,
                Pais = local.Pais,
                Latitude = local.Latitude,
                Longitude = local.Longitude,
                Fuso = local.Fuso,
                Principal = local.Principal,
                ReceberAlertas = local.ReceberAlertas,
                CriadoEm = local.CriadoEm
            };
        }
    }
}
