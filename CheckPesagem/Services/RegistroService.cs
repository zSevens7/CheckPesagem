using System;
using System.Collections.Generic;
using CheckPesagem.Models; // <- Certifique-se de que Perfil e Registro estão aqui

namespace CheckPesagem.Services
{
    public static class RegistroService
    {
        // Dicionário simulando banco de dados: chave = Perfil, valor = lista de registros
        private static Dictionary<Perfil, List<Registro>> registrosPorPerfil = new();

        /// <summary>
        /// Retorna todos os registros de um perfil
        /// </summary>
        public static List<Registro> ObterRegistros(Perfil perfil)
        {
            if (registrosPorPerfil.ContainsKey(perfil))
                return registrosPorPerfil[perfil];
            return new List<Registro>();
        }

        /// <summary>
        /// Adiciona um registro a um perfil
        /// </summary>
        public static void AdicionarRegistro(Perfil perfil, Registro registro)
        {
            if (!registrosPorPerfil.ContainsKey(perfil))
                registrosPorPerfil[perfil] = new List<Registro>();

            registrosPorPerfil[perfil].Add(registro);
        }
    }
}
