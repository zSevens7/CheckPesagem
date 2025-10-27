using System;
using System.Collections.Generic;
using System.Linq;
using CheckPesagem.Models;

namespace CheckPesagem.Services
{
    public static class EstatisticasService
    {
        public static Dictionary<int, double> CalcularMediaPesoPorAno(List<Registro> registros)
        {
            if (registros == null || !registros.Any())
                return new Dictionary<int, double>();

            return registros
                .GroupBy(r => r.Data.Year)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Average(r => r.Peso));
        }

        public static Dictionary<string, double> CalcularMediaPesoPorMesAno(List<Registro> registros, int ano)
        {
            var registrosDoAno = registros.Where(r => r.Data.Year == ano);
            
            if (!registrosDoAno.Any())
                return new Dictionary<string, double>();

            return registrosDoAno
                .GroupBy(r => new { r.Data.Year, r.Data.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Month:00}/{g.Key.Year}",
                    g => g.Average(r => r.Peso)
                );
        }

        public static Dictionary<string, double> CalcularMediaIMCPorMesAno(List<Registro> registros, Perfil perfil, int ano)
        {
            var registrosDoAno = registros.Where(r => r.Data.Year == ano);
            
            if (!registrosDoAno.Any())
                return new Dictionary<string, double>();

            return registrosDoAno
                .GroupBy(r => new { r.Data.Year, r.Data.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Month:00}/{g.Key.Year}",
                    g => g.Average(r => perfil.CalcularIMC(r.Peso))
                );
        }

        public static Dictionary<string, double?> CalcularMediaCinturaPorMesAno(List<Registro> registros, int ano)
        {
            var registrosDoAno = registros
                .Where(r => r.Data.Year == ano && r.Cintura.HasValue);
            
            if (!registrosDoAno.Any())
                return new Dictionary<string, double?>();

            return registrosDoAno
                .GroupBy(r => new { r.Data.Year, r.Data.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Month:00}/{g.Key.Year}",
                    g => (double?)g.Average(r => r.Cintura!.Value) // Corrigido: adicionado ! e cast para double?
                );
        }

        public static (double pesoAtual, double pesoInicial, double variacaoPercentual, int diasAcompanhamento) 
            CalcularProgresso(List<Registro> registros)
        {
            if (registros == null || !registros.Any())
                return (0, 0, 0, 0);

            var registrosOrdenados = registros.OrderBy(r => r.Data).ToList();
            var pesoAtual = registrosOrdenados.Last().Peso;
            var pesoInicial = registrosOrdenados.First().Peso;
            var variacao = ((pesoAtual - pesoInicial) / pesoInicial) * 100;
            var dias = (int)(registros.Max(r => r.Data) - registros.Min(r => r.Data)).TotalDays;

            return (pesoAtual, pesoInicial, variacao, dias);
        }

        public static List<(DateTime data, double peso)> ObterDadosPeso(List<Registro> registros)
        {
            return registros?
                .OrderBy(r => r.Data)
                .Select(r => (r.Data, r.Peso))
                .ToList() ?? new List<(DateTime, double)>();
        }

        public static List<(DateTime data, double imc)> ObterDadosIMC(List<Registro> registros, Perfil perfil)
        {
            return registros?
                .OrderBy(r => r.Data)
                .Select(r => (r.Data, perfil.CalcularIMC(r.Peso)))
                .ToList() ?? new List<(DateTime, double)>();
        }

        public static List<(DateTime data, double cintura)> ObterDadosCintura(List<Registro> registros)
        {
            return registros?
                .Where(r => r.Cintura.HasValue)
                .OrderBy(r => r.Data)
                .Select(r => (r.Data, r.Cintura!.Value)) // Corrigido: adicionado !
                .ToList() ?? new List<(DateTime, double)>();
        }
    }
}