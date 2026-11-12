using System.Globalization;
using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;

namespace GaroAvisa.Application.Services
{
    public class ResumoService : IResumoService
    {
        private readonly ILocalRepository _localRepository;
        private readonly IPreferenciaRepository _preferenciaRepository;
        private readonly IAlertaRepository _alertaRepository;
        private readonly IOpenMeteoClient _openMeteo;

        public ResumoService(
            ILocalRepository localRepository,
            IPreferenciaRepository preferenciaRepository,
            IAlertaRepository alertaRepository,
            IOpenMeteoClient openMeteo)
        {
            _localRepository = localRepository;
            _preferenciaRepository = preferenciaRepository;
            _alertaRepository = alertaRepository;
            _openMeteo = openMeteo;
        }

        public async Task<ResumoDiaDto> ObterResumoAsync(int usuarioId)
        {
            var preferencia = await _preferenciaRepository.BuscarPorUsuarioAsync(usuarioId) ?? new Preferencia();
            var locais = await _localRepository.ListarAsync(usuarioId);

            var hoje = DateTime.UtcNow.AddHours(-3).Date;

            var resumo = new ResumoDiaDto
            {
                ResumoDiarioAtivo = preferencia.ResumoDiarioAtivo,
                ResumoDiarioHorario = preferencia.ResumoDiarioHorario
            };

            foreach (var local in locais)
            {
                var item = new ResumoLocalDto
                {
                    LocalId = local.Id,
                    Etiqueta = local.Etiqueta,
                    Nome = local.Nome
                };

                try
                {
                    var previsao = await _openMeteo.ObterPrevisaoAsync(local.Latitude, local.Longitude, local.Fuso, 1);
                    item.PrevisaoHoje = previsao.FirstOrDefault();

                    if (item.PrevisaoHoje is not null)
                    {
                        hoje = DateTime.ParseExact(item.PrevisaoHoje.Data, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                }
                catch (HttpRequestException)
                {
                }

                var alertas = await _alertaRepository.ListarPorLocalEDataAsync(local.Id, hoje);
                item.AlertasHoje = alertas.Select(a => AlertaService.ParaResposta(a, local.Etiqueta)).ToList();

                resumo.TotalAlertasHoje += item.AlertasHoje.Count;
                resumo.Locais.Add(item);
            }

            resumo.Data = hoje.ToString("yyyy-MM-dd");
            return resumo;
        }
    }
}