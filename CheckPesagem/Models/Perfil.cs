using System;
using System.Collections.Generic;

namespace CheckPesagem.Models
{
    public class Perfil
    {
        public string Nome { get; set; } = "";
        public DateTime DataNascimento { get; set; }
        public double Altura { get; set; } // em metros
        public double PesoAtual { get; set; } // em kg
        public double? Cintura { get; set; } // opcional

        public List<Registro> Registros { get; set; } = new List<Registro>();

        public int Idade => (int)((DateTime.Now - DataNascimento).TotalDays / 365.25);

        // IMC com peso atual
        public double CalcularIMC()
        {
            if (Altura <= 0) return 0;
            return Math.Round(PesoAtual / (Altura * Altura), 2);
        }

        // IMC com peso específico (para registros históricos)
        public double CalcularIMC(double peso)
        {
            if (Altura <= 0) return 0;
            return Math.Round(peso / (Altura * Altura), 2);
        }

        // Classificação do IMC
        public string ClassificarIMC(double imc)
        {
            if (imc < 18.5) return "Abaixo do peso";
            if (imc < 25) return "Peso normal";
            if (imc < 30) return "Sobrepeso";
            if (imc < 35) return "Obesidade Grau I";
            if (imc < 40) return "Obesidade Grau II";
            return "Obesidade Grau III";
        }
    }
}