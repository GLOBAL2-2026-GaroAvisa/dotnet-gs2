using GaroAvisa.Application.DTOs;
using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Application.Helpers
{
    public static class AvaliadorClima
    {
        public static double? ValorDoTipo(string tipo, PrevisaoDiaDto dia)
        {
            switch (tipo)
            {
                case "CHUVA_FORTE":
                    return dia.ChuvaMm;
                case "VENTO_FORTE":
                    return dia.RajadaVentoKmh;
                case "CALOR":
                    return dia.TemperaturaMaxima;
                case "FRIO":
                    return dia.TemperaturaMinima;
                case "TEMPESTADE":
                    return dia.CodigoTempo;
                default:
                    return null;
            }
        }

        public static bool Ultrapassou(string tipo, double valor, double limite)
        {
            return tipo == "FRIO" ? valor <= limite : valor >= limite;
        }

        public static RegraAlerta? EscolherRegra(List<RegraAlerta> regras, string tipo, PrevisaoDiaDto dia)
        {
            var valor = ValorDoTipo(tipo, dia);
            if (valor is null)
            {
                return null;
            }

            return regras
                .Where(r => r.Ativa && r.Tipo == tipo && Ultrapassou(tipo, valor.Value, r.Limite))
                .OrderByDescending(r => PesoGravidade(r.Gravidade))
                .FirstOrDefault();
        }

        public static int PesoGravidade(string gravidade)
        {
            switch (gravidade)
            {
                case "ALTA":
                    return 3;
                case "MEDIA":
                    return 2;
                default:
                    return 1;
            }
        }

        public static string? DescreverMudanca(PrevisaoSalva antes, PrevisaoDiaDto agora)
        {
            var mudancas = new List<string>();

            var chuvaAntes = antes.ChuvaMm ?? 0;
            var chuvaAgora = agora.ChuvaMm ?? 0;
            if (Math.Abs(chuvaAgora - chuvaAntes) >= 10)
            {
                mudancas.Add($"chuva de {chuvaAntes:0.#} mm para {chuvaAgora:0.#} mm");
            }

            if (antes.TemperaturaMaxima is not null && agora.TemperaturaMaxima is not null &&
                Math.Abs(agora.TemperaturaMaxima.Value - antes.TemperaturaMaxima.Value) >= 5)
            {
                mudancas.Add($"máxima de {antes.TemperaturaMaxima:0.#} °C para {agora.TemperaturaMaxima:0.#} °C");
            }

            if (antes.CodigoTempo < 51 && agora.CodigoTempo >= 51)
            {
                mudancas.Add($"tempo de \"{CodigoTempo.Descrever(antes.CodigoTempo)}\" para \"{CodigoTempo.Descrever(agora.CodigoTempo)}\"");
            }

            return mudancas.Count == 0 ? null : "A previsão mudou: " + string.Join("; ", mudancas) + ".";
        }
    }
}