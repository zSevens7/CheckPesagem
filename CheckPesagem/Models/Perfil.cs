using System;

namespace CheckPesagem.Models
{
    public class Perfil
    {
        public string Nome { get; set; } = "";
        public DateTime DataNascimento { get; set; }
        public double Altura { get; set; } // em metros
        public double PesoAtual { get; set; } // em kg
        public double? Cintura { get; set; } // opcional

        public int Idade => (int)((DateTime.Now - DataNascimento).TotalDays / 365.25);

        public double CalcularIMC()
        {
            if (Altura <= 0) return 0;
            return Math.Round(PesoAtual / (Altura * Altura), 2);
        }
    }
}
