using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CheckPesagem.Models;

namespace CheckPesagem.Services
{
    public static class PerfilService
    {
        private static readonly string pastaPerfis;
        private static readonly string caminhoPerfilAtual;

        static PerfilService()
        {
            // caminho absoluto relativo ao executável
            pastaPerfis = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "perfis");
            if (!Directory.Exists(pastaPerfis))
                Directory.CreateDirectory(pastaPerfis);

            caminhoPerfilAtual = Path.Combine(pastaPerfis, "perfil_atual.json");
        }

        /// <summary>
        /// Salva o perfil em um arquivo JSON separado (perfil_Nome Sobrenome.json).
        /// </summary>
        public static void SalvarPerfil(Perfil perfil)
        {
            if (perfil == null || string.IsNullOrWhiteSpace(perfil.Nome))
                throw new ArgumentException("Perfil inválido para salvar.");

            string caminho = ObterCaminho(perfil.Nome);
            string json = JsonSerializer.Serialize(perfil, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminho, json);
        }

        /// <summary>
        /// Carrega um perfil específico pelo nome.
        /// </summary>
        public static Perfil? CarregarPerfil(string nome)
        {
            string caminho = ObterCaminho(nome);
            if (!File.Exists(caminho))
                return null;

            string json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<Perfil>(json);
        }

        /// <summary>
        /// Lista todos os perfis (retorna nomes limpos, sem o prefixo).
        /// </summary>
        public static List<string> ListarPerfis()
        {
            if (!Directory.Exists(pastaPerfis))
                return new List<string>();

            var arquivos = Directory.GetFiles(pastaPerfis, "perfil_*.json");
            return arquivos
                .Select(a => Path.GetFileNameWithoutExtension(a).Replace("perfil_", ""))
                .ToList();
        }

        /// <summary>
        /// Exclui um perfil existente pelo nome.
        /// </summary>
        public static bool ExcluirPerfil(string nome)
        {
            string caminho = ObterCaminho(nome);
            if (File.Exists(caminho))
            {
                File.Delete(caminho);
                // se o perfil excluído era o atual, remove o arquivo perfil_atual.json também
                if (File.Exists(caminhoPerfilAtual))
                {
                    try
                    {
                        var atual = CarregarPerfilAtual();
                        if (atual != null && string.Equals(atual.Nome, nome, StringComparison.OrdinalIgnoreCase))
                            File.Delete(caminhoPerfilAtual);
                    }
                    catch { /* não trava na exclusão */ }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Salva um arquivo que representa o perfil atualmente selecionado (perfil_atual.json).
        /// </summary>
        public static void SalvarPerfilAtual(Perfil perfil)
        {
            if (perfil == null) throw new ArgumentNullException(nameof(perfil));
            string json = JsonSerializer.Serialize(perfil, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminhoPerfilAtual, json);
        }

        /// <summary>
        /// Carrega o perfil atual salvo em perfil_atual.json (ou null se não existir).
        /// </summary>
        public static Perfil? CarregarPerfilAtual()
        {
            if (!File.Exists(caminhoPerfilAtual)) return null;
            string json = File.ReadAllText(caminhoPerfilAtual);
            return JsonSerializer.Deserialize<Perfil>(json);
        }

        /// <summary>
        /// Gera o caminho completo do arquivo JSON de um perfil a partir do nome.
        /// </summary>
        private static string ObterCaminho(string nome)
        {
            // mantém espaços no nome do arquivo (facilita leitura), mas remove caracteres inválidos
            string nomeSanitizado = string.Join("_", nome.Trim().Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
                                     .Replace("__", "_")
                                     .Trim('_');

            // garantir que ainda exista algo
            if (string.IsNullOrWhiteSpace(nomeSanitizado))
                nomeSanitizado = "perfil";

            string arquivo = $"perfil_{nomeSanitizado}.json";
            return Path.Combine(pastaPerfis, arquivo);
        }
    }
}
