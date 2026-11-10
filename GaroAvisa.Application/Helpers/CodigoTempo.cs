namespace GaroAvisa.Application.Helpers
{
    // Traduz o código do tempo (padrão WMO) que a Open-Meteo devolve.
    public static class CodigoTempo
    {
        private static readonly Dictionary<int, string> Descricoes = new()
        {
            { 0, "Céu limpo" },
            { 1, "Predominantemente limpo" },
            { 2, "Parcialmente nublado" },
            { 3, "Nublado" },
            { 45, "Nevoeiro" },
            { 48, "Nevoeiro com geada" },
            { 51, "Garoa fraca" },
            { 53, "Garoa moderada" },
            { 55, "Garoa forte" },
            { 56, "Garoa congelante" },
            { 57, "Garoa congelante forte" },
            { 61, "Chuva fraca" },
            { 63, "Chuva moderada" },
            { 65, "Chuva forte" },
            { 66, "Chuva congelante" },
            { 67, "Chuva congelante forte" },
            { 71, "Neve fraca" },
            { 73, "Neve moderada" },
            { 75, "Neve forte" },
            { 77, "Grãos de neve" },
            { 80, "Pancadas de chuva fracas" },
            { 81, "Pancadas de chuva moderadas" },
            { 82, "Pancadas de chuva fortes" },
            { 85, "Pancadas de neve" },
            { 86, "Pancadas de neve fortes" },
            { 95, "Trovoada" },
            { 96, "Trovoada com granizo" },
            { 99, "Trovoada com granizo forte" }
        };

        public static string Descrever(int codigo)
        {
            return Descricoes.TryGetValue(codigo, out var descricao) ? descricao : "Condição desconhecida";
        }
    }
}