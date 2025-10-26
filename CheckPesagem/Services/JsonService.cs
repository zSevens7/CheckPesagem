using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CheckPesagem.Models;

namespace CheckPesagem.Services
{
    public static class JsonService
    {
        private static readonly string PastaPerfis = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "perfis");
        private static readonly string PastaRegistros = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "registros");

        // Salvar qualquer objeto em JSON
        public static void Salvar<T>(string nomeArquivo, T dados, bool perfil = true)
        {
            string pasta = perfil ? PastaPerfis : PastaRegistros;
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string caminho = Path.Combine(pasta, nomeArquivo);
            string json = JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminho, json);
        }

        // Ler JSON
        public static T? Ler<T>(string nomeArquivo, bool perfil = true)
        {
            string pasta = perfil ? PastaPerfis : PastaRegistros;
            string caminho = Path.Combine(pasta, nomeArquivo);
            if (!File.Exists(caminho)) return default;
            string json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<T>(json);
        }

        // Adicionar registro diário
        public static void AdicionarRegistro(string nomeArquivo, Registro registro)
        {
            List<Registro> registros = Ler<List<Registro>>(nomeArquivo, perfil: false) ?? new List<Registro>();
            registros.Add(registro);
            Salvar(nomeArquivo, registros, perfil: false);
        }
    }
}
