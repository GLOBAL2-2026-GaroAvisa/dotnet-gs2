using System.Globalization;
using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Helpers;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;

namespace GaroAvisa.Application.Services
{
    public class AlertaService : IAlertaService
    {
        private static readonly string[] TiposDeRisco = { "CHUVA_FORTE", "VENTO_FORTE", "CALOR", "FRIO", "TEMPESTADE" };

        private readonly IAlertaRepository _alertaRepository;
        private readonly ILocalRepository _localRepository;
        private readonly IPreferenciaRepository _preferenciaRepository;
        private readonly IRegraAlertaRepository _regraRepository;
        private readonly IPrevisaoSalvaRepository _previsaoRepository;
        private readonly IOpenMeteoClient _openMeteo;

        public AlertaService(
            IAlertaRepository alertaRepository,
            ILocalRepository localRepository,
            IPreferenciaRepository preferenciaRepository,
            IRegraAlertaRepository regraRepository,
            IPrevisaoSalvaRepository previsaoRepository,
            IOpenMeteoClient openMeteo)
        {
            _alertaRepository = alertaRepository;
            _localRepository = localRepository;
            _preferenciaRepository = preferenciaRepository;
            _regraRepository = regraRepository;
            _previsaoRepository = previsaoRepository;
            _openMeteo = openMeteo;
        }

        public async Task<List<AlertaRespostaDto>> ListarAsync(int usuarioId, bool somenteNaoLidos)
        {
            var locais = await _localRepository.ListarAsync(usuarioId);
            var apelidos = locais.ToDictionary(l => l.Id, l => l.Apelido);

            var alertas = await _alertaRepository.ListarPorLocaisAsync(apelidos.Keys.ToList(), somenteNaoLidos);
            return alertas.Select(a => ParaResposta(a, apelidos[a.LocalId])).ToList();
        }

        public async Task<AlertaRespostaDto?> BuscarPorIdAsync(int id, int usuarioId)
        {
            var alerta = await _alertaRepository.BuscarPorIdAsync(id);
            if (alerta is null)
            {
                return null;
            }

            var local = await _localRepository.BuscarPorIdAsync(alerta.LocalId, usuarioId);
            if (local is null)
            {
                return null;
            }

            return ParaResposta(alerta, local.Apelido);
        }

        public async Task<AlertaRespostaDto?> MarcarComoLidoAsync(int id, int usuarioId)
        {
            var alerta = await _alertaRepository.BuscarPorIdAsync(id);
            if (alerta is null)
            {
                return null;
            }

            var local = await _localRepository.BuscarPorIdAsync(alerta.LocalId, usuarioId);
            if (local is null)
            {
                return null;
            }

            alerta.Lido = true;
            await _alertaRepository.AtualizarAsync(alerta);
            return ParaResposta(alerta, local.Apelido);
        }

        public async Task<bool> ExcluirAsync(int id, int usuarioId)
        {
            var alerta = await _alertaRepository.BuscarPorIdAsync(id);
            if (alerta is null)
            {
                return false;
            }

            var local = await _localRepository.BuscarPorIdAsync(alerta.LocalId, usuarioId);
            if (local is null)
            {
                return false;
            }

            await _alertaRepository.ExcluirAsync(alerta);
            return true;
        }

        public async Task<VerificacaoResultadoDto> VerificarAsync()
        {
            var resultado = new VerificacaoResultadoDto();
            var regras = await _regraRepository.ListarAsync(somenteAtivas: true);
            var locais = await _localRepository.ListarParaAlertasAsync();

            foreach (var local in locais)
            {
                var preferencia = await _preferenciaRepository.BuscarPorUsuarioAsync(local.UsuarioId) ?? new Preferencia();

                List<PrevisaoDiaDto> previsao;
                try
                {
                    previsao = await _openMeteo.ObterPrevisaoAsync(local.Latitude, local.Longitude, local.Fuso, 2);
                }
                catch (HttpRequestException)
                {
                    resultado.LocaisComFalha++;
                    continue;
                }

                foreach (var dia in previsao)
                {
                    var data = DateTime.ParseExact(dia.Data, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    if (preferencia.AlertasDoDia)
                    {
                        resultado.AlertasCriados += await CriarAlertasDeRiscoAsync(local, data, dia, regras);
                    }

                    if (await VerificarMudancaAsync(local, data, dia, preferencia.PrevisaoMudou))
                    {
                        resultado.AlertasPrevisaoMudou++;
                    }
                }

                resultado.LocaisVerificados++;
            }

            resultado.ExecutadoEm = DateTime.UtcNow;
            return resultado;
        }

        private async Task<int> CriarAlertasDeRiscoAsync(Local local, DateTime data, PrevisaoDiaDto dia, List<RegraAlerta> regras)
        {
            var criados = 0;

            foreach (var tipo in TiposDeRisco)
            {
                var regra = AvaliadorClima.EscolherRegra(regras, tipo, dia);
                if (regra is null)
                {
                    continue;
                }

                if (await _alertaRepository.ExisteAsync(local.Id, data, tipo))
                {
                    continue;
                }

                await _alertaRepository.CriarAsync(new Alerta
                {
                    LocalId = local.Id,
                    DataPrevisao = data,
                    Tipo = tipo,
                    Titulo = regra.Titulo,
                    Descricao = regra.Descricao,
                    Gravidade = regra.Gravidade,
                    ValorPrevisto = AvaliadorClima.ValorDoTipo(tipo, dia)
                });
                criados++;
            }

            return criados;
        }

        private async Task<bool> VerificarMudancaAsync(Local local, DateTime data, PrevisaoDiaDto dia, bool usuarioQuerAviso)
        {
            var salva = await _previsaoRepository.BuscarAsync(local.Id, data);
            var criouAlerta = false;

            if (salva is null)
            {
                salva = new PrevisaoSalva { LocalId = local.Id, DataPrevisao = data };
                CopiarPrevisao(dia, salva);
                await _previsaoRepository.CriarAsync(salva);
                return false;
            }

            var mudanca = AvaliadorClima.DescreverMudanca(salva, dia);

            if (mudanca is not null && usuarioQuerAviso && !await _alertaRepository.ExisteAsync(local.Id, data, "PREVISAO_MUDOU"))
            {
                await _alertaRepository.CriarAsync(new Alerta
                {
                    LocalId = local.Id,
                    DataPrevisao = data,
                    Tipo = "PREVISAO_MUDOU",
                    Titulo = $"Previsão mudou para {data:dd/MM}",
                    Descricao = mudanca,
                    Gravidade = dia.CodigoTempo >= 95 ? "MEDIA" : "BAIXA",
                    ValorPrevisto = dia.ChuvaMm
                });
                criouAlerta = true;
            }

            CopiarPrevisao(dia, salva);
            salva.AtualizadoEm = DateTime.UtcNow;
            await _previsaoRepository.AtualizarAsync(salva);

            return criouAlerta;
        }

        private static void CopiarPrevisao(PrevisaoDiaDto dia, PrevisaoSalva salva)
        {
            salva.CodigoTempo = dia.CodigoTempo;
            salva.TemperaturaMaxima = dia.TemperaturaMaxima;
            salva.TemperaturaMinima = dia.TemperaturaMinima;
            salva.ChuvaMm = dia.ChuvaMm;
            salva.ProbabilidadeChuva = dia.ProbabilidadeChuva;
            salva.RajadaVentoKmh = dia.RajadaVentoKmh;
        }

        public static AlertaRespostaDto ParaResposta(Alerta alerta, string localApelido)
        {
            return new AlertaRespostaDto
            {
                Id = alerta.Id,
                LocalId = alerta.LocalId,
                LocalApelido = localApelido,
                DataPrevisao = alerta.DataPrevisao.ToString("yyyy-MM-dd"),
                Tipo = alerta.Tipo,
                Titulo = alerta.Titulo,
                Descricao = alerta.Descricao,
                Gravidade = alerta.Gravidade,
                ValorPrevisto = alerta.ValorPrevisto,
                Lido = alerta.Lido,
                CriadoEm = alerta.CriadoEm
            };
        }
    }
}