using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;

namespace GaroAvisa.Application.Services
{
    public class PreferenciaService : IPreferenciaService
    {
        private readonly IPreferenciaRepository _repository;

        public PreferenciaService(IPreferenciaRepository repository)
        {
            _repository = repository;
        }

        public async Task<PreferenciaRespostaDto> ObterAsync(int usuarioId)
        {
            var preferencia = await _repository.BuscarPorUsuarioAsync(usuarioId);

            if (preferencia is null)
            {
                return ParaResposta(new Preferencia(), personalizada: false);
            }

            return ParaResposta(preferencia, personalizada: true);
        }

        public async Task<PreferenciaRespostaDto> SalvarAsync(SalvarPreferenciaDto dto, int usuarioId)
        {
            var preferencia = await _repository.BuscarPorUsuarioAsync(usuarioId);
            var primeiraVez = preferencia is null;

            preferencia ??= new Preferencia { UsuarioId = usuarioId };

            preferencia.ResumoDiarioAtivo = dto.ResumoDiarioAtivo;
            preferencia.ResumoDiarioHorario = dto.ResumoDiarioHorario;
            preferencia.AlertasDoDia = dto.AlertasDoDia;
            preferencia.PrevisaoMudou = dto.PrevisaoMudou;
            preferencia.Compromissos = dto.Compromissos;
            preferencia.SilencioNoturno = dto.SilencioNoturno;
            preferencia.AtualizadoEm = DateTime.UtcNow;

            if (primeiraVez)
            {
                await _repository.CriarAsync(preferencia);
            }
            else
            {
                await _repository.AtualizarAsync(preferencia);
            }

            return ParaResposta(preferencia, personalizada: true);
        }

        public async Task RestaurarPadraoAsync(int usuarioId)
        {
            var preferencia = await _repository.BuscarPorUsuarioAsync(usuarioId);
            if (preferencia is not null)
            {
                await _repository.ExcluirAsync(preferencia);
            }
        }

        private static PreferenciaRespostaDto ParaResposta(Preferencia preferencia, bool personalizada)
        {
            return new PreferenciaRespostaDto
            {
                ResumoDiarioAtivo = preferencia.ResumoDiarioAtivo,
                ResumoDiarioHorario = preferencia.ResumoDiarioHorario,
                AlertasDoDia = preferencia.AlertasDoDia,
                PrevisaoMudou = preferencia.PrevisaoMudou,
                Compromissos = preferencia.Compromissos,
                SilencioNoturno = preferencia.SilencioNoturno,
                Personalizada = personalizada,
                AtualizadoEm = personalizada ? preferencia.AtualizadoEm : null
            };
        }
    }
}