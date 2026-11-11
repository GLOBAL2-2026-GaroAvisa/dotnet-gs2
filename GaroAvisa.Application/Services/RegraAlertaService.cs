using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;

namespace GaroAvisa.Application.Services
{
    public class RegraAlertaService : IRegraAlertaService
    {
        private readonly IRegraAlertaRepository _repository;

        public RegraAlertaService(IRegraAlertaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RegraAlertaRespostaDto>> ListarAsync(bool somenteAtivas)
        {
            var regras = await _repository.ListarAsync(somenteAtivas);
            return regras.Select(ParaResposta).ToList();
        }

        public async Task<RegraAlertaRespostaDto?> BuscarPorIdAsync(int id)
        {
            var regra = await _repository.BuscarPorIdAsync(id);
            return regra is null ? null : ParaResposta(regra);
        }

        public async Task<RegraAlertaRespostaDto> CriarAsync(SalvarRegraAlertaDto dto)
        {
            // Só pode existir uma regra para cada combinação de tipo e gravidade.
            if (await _repository.ExisteAsync(dto.Tipo, dto.Gravidade))
            {
                throw new InvalidOperationException("Já existe uma regra com esse tipo e essa gravidade. Edite a regra existente ou escolha outra gravidade.");
            }

            var regra = new RegraAlerta
            {
                Tipo = dto.Tipo,
                Limite = dto.Limite,
                Gravidade = dto.Gravidade,
                Titulo = dto.Titulo.Trim(),
                Descricao = dto.Descricao.Trim(),
                Ativa = dto.Ativa
            };

            var criada = await _repository.CriarAsync(regra);
            return ParaResposta(criada);
        }

        public async Task<RegraAlertaRespostaDto?> AtualizarAsync(int id, SalvarRegraAlertaDto dto)
        {
            var regra = await _repository.BuscarPorIdAsync(id);
            if (regra is null)
            {
                return null;
            }

            if (await _repository.ExisteAsync(dto.Tipo, dto.Gravidade, id))
            {
                throw new InvalidOperationException("Já existe uma regra com esse tipo e essa gravidade. Edite a regra existente ou escolha outra gravidade.");
            }

            regra.Tipo = dto.Tipo;
            regra.Limite = dto.Limite;
            regra.Gravidade = dto.Gravidade;
            regra.Titulo = dto.Titulo.Trim();
            regra.Descricao = dto.Descricao.Trim();
            regra.Ativa = dto.Ativa;

            await _repository.AtualizarAsync(regra);
            return ParaResposta(regra);
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var regra = await _repository.BuscarPorIdAsync(id);
            if (regra is null)
            {
                return false;
            }

            await _repository.ExcluirAsync(regra);
            return true;
        }

        private static RegraAlertaRespostaDto ParaResposta(RegraAlerta regra)
        {
            return new RegraAlertaRespostaDto
            {
                Id = regra.Id,
                Tipo = regra.Tipo,
                Limite = regra.Limite,
                Gravidade = regra.Gravidade,
                Titulo = regra.Titulo,
                Descricao = regra.Descricao,
                Ativa = regra.Ativa,
                CriadoEm = regra.CriadoEm
            };
        }
    }
}