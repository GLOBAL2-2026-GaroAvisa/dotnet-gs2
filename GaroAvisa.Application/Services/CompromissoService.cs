using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;

namespace GaroAvisa.Application.Services
{
    public class CompromissoService : ICompromissoService
    {
        private readonly ICompromissoRepository _repository;
        private readonly ILocalRepository _localRepository;
        private readonly IAlertaRepository _alertaRepository;
        private readonly IOpenMeteoClient _openMeteo;

        public CompromissoService(
            ICompromissoRepository repository,
            ILocalRepository localRepository,
            IAlertaRepository alertaRepository,
            IOpenMeteoClient openMeteo)
        {
            _repository = repository;
            _localRepository = localRepository;
            _alertaRepository = alertaRepository;
            _openMeteo = openMeteo;
        }

        public async Task<List<CompromissoRespostaDto>> ListarAsync(int usuarioId, int? localId)
        {
            var locais = await _localRepository.ListarAsync(usuarioId);
            var etiquetas = locais.ToDictionary(l => l.Id, l => l.Etiqueta);

            var compromissos = await _repository.ListarPorLocaisAsync(etiquetas.Keys.ToList());

            return compromissos
                .Where(c => localId == null || c.LocalId == localId)
                .Select(c => ParaResposta(c, etiquetas[c.LocalId]))
                .ToList();
        }

        public async Task<CompromissoRespostaDto?> BuscarPorIdAsync(int id, int usuarioId)
        {
            var compromisso = await _repository.BuscarPorIdAsync(id);
            if (compromisso is null)
            {
                return null;
            }

            var local = await _localRepository.BuscarPorIdAsync(compromisso.LocalId, usuarioId);
            if (local is null)
            {
                return null;
            }

            return ParaResposta(compromisso, local.Etiqueta);
        }

        public async Task<CompromissoRespostaDto?> CriarAsync(SalvarCompromissoDto dto, int usuarioId)
        {
            var local = await _localRepository.BuscarPorIdAsync(dto.LocalId, usuarioId);
            if (local is null)
            {
                return null;
            }

            ValidarData(dto.DataHora!.Value);

            var compromisso = new Compromisso
            {
                LocalId = local.Id,
                Nome = dto.Nome.Trim(),
                DataHora = dto.DataHora.Value
            };

            var criado = await _repository.CriarAsync(compromisso);
            return ParaResposta(criado, local.Etiqueta);
        }

        public async Task<CompromissoRespostaDto?> AtualizarAsync(int id, SalvarCompromissoDto dto, int usuarioId)
        {
            var compromisso = await _repository.BuscarPorIdAsync(id);
            if (compromisso is null)
            {
                return null;
            }

            var localAtual = await _localRepository.BuscarPorIdAsync(compromisso.LocalId, usuarioId);
            var localNovo = await _localRepository.BuscarPorIdAsync(dto.LocalId, usuarioId);
            if (localAtual is null || localNovo is null)
            {
                return null;
            }

            ValidarData(dto.DataHora!.Value);

            compromisso.LocalId = localNovo.Id;
            compromisso.Nome = dto.Nome.Trim();
            compromisso.DataHora = dto.DataHora.Value;

            await _repository.AtualizarAsync(compromisso);
            return ParaResposta(compromisso, localNovo.Etiqueta);
        }

        public async Task<bool> ExcluirAsync(int id, int usuarioId)
        {
            var compromisso = await _repository.BuscarPorIdAsync(id);
            if (compromisso is null)
            {
                return false;
            }

            var local = await _localRepository.BuscarPorIdAsync(compromisso.LocalId, usuarioId);
            if (local is null)
            {
                return false;
            }

            await _repository.ExcluirAsync(compromisso);
            return true;
        }

        public async Task<CompromissoPrevisaoDto?> ObterPrevisaoAsync(int id, int usuarioId)
        {
            var compromisso = await _repository.BuscarPorIdAsync(id);
            if (compromisso is null)
            {
                return null;
            }

            var local = await _localRepository.BuscarPorIdAsync(compromisso.LocalId, usuarioId);
            if (local is null)
            {
                return null;
            }

            var resposta = new CompromissoPrevisaoDto
            {
                Compromisso = ParaResposta(compromisso, local.Etiqueta)
            };

            var dataCompromisso = compromisso.DataHora.Date;
            var diasAteCompromisso = (dataCompromisso - HojeEmBrasilia()).Days;

            if (diasAteCompromisso < 0 || diasAteCompromisso > 6)
            {
                resposta.Mensagem = "A previsão fica disponível nos 7 dias antes do compromisso.";
            }
            else
            {
                try
                {
                    var previsao = await _openMeteo.ObterPrevisaoAsync(local.Latitude, local.Longitude, local.Fuso, diasAteCompromisso + 1);
                    resposta.PrevisaoDoDia = previsao.FirstOrDefault(d => d.Data == dataCompromisso.ToString("yyyy-MM-dd"));
                }
                catch (HttpRequestException)
                {
                    resposta.Mensagem = "O serviço de previsão do tempo está indisponível no momento.";
                }
            }

            var alertas = await _alertaRepository.ListarPorLocalEDataAsync(local.Id, dataCompromisso);
            resposta.AlertasDoDia = alertas.Select(a => AlertaService.ParaResposta(a, local.Etiqueta)).ToList();

            return resposta;
        }

        private static void ValidarData(DateTime dataHora)
        {
            if (dataHora < HojeEmBrasilia())
            {
                throw new ArgumentException("A data do compromisso não pode estar no passado. Informe uma data a partir de hoje.");
            }
        }

        private static DateTime HojeEmBrasilia()
        {
            return DateTime.UtcNow.AddHours(-3).Date;
        }

        private static CompromissoRespostaDto ParaResposta(Compromisso compromisso, string localEtiqueta)
        {
            return new CompromissoRespostaDto
            {
                Id = compromisso.Id,
                LocalId = compromisso.LocalId,
                LocalEtiqueta = localEtiqueta,
                Nome = compromisso.Nome,
                DataHora = compromisso.DataHora,
                CriadoEm = compromisso.CriadoEm
            };
        }
    }
}