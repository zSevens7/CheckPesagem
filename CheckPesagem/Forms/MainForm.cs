using System;
using System.Windows.Forms;
using CheckPesagem.Models;
using CheckPesagem.Services;

namespace CheckPesagem.Forms
{
    public class MainForm : Form
    {
        private Perfil? perfilAtual;

        private Button btnNovoPerfil;
        private Button btnNovoRegistro;
        private Label lblNome;
        private Label lblIMC;

        public MainForm()
        {
            CriarInterface();
        }

        private void CriarInterface()
        {
            this.Text = "CheckPesagem - Controle de Peso";
            this.Width = 720;
            this.Height = 480;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Botão Criar Perfil
            btnNovoPerfil = new Button
            {
                Text = "Criar Perfil",
                Width = 120,
                Height = 40,
                Top = 30,
                Left = 30
            };
            btnNovoPerfil.Click += BtnNovoPerfil_Click;

            // Botão Adicionar Registro Diário
            btnNovoRegistro = new Button
            {
                Text = "Adicionar Registro",
                Width = 150,
                Height = 40,
                Top = 30,
                Left = 170
            };
            btnNovoRegistro.Click += BtnNovoRegistro_Click;

            lblNome = new Label { Left = 30, Top = 100, Width = 300, Text = "Nenhum perfil carregado." };
            lblIMC = new Label { Left = 30, Top = 130, Width = 300, Text = "IMC: -" };

            Controls.Add(btnNovoPerfil);
            Controls.Add(btnNovoRegistro);
            Controls.Add(lblNome);
            Controls.Add(lblIMC);
        }

        private void BtnNovoPerfil_Click(object? sender, EventArgs e)
        {
            using (var perfilForm = new PerfilForm())
            {
                perfilForm.ShowDialog();
                if (perfilForm.PerfilCriado != null)
                {
                    perfilAtual = perfilForm.PerfilCriado;
                    lblNome.Text = $"Perfil: {perfilAtual.Nome} ({perfilAtual.Idade} anos)";
                    lblIMC.Text = $"IMC: {perfilAtual.CalcularIMC()}";
                }
            }
        }

        private void BtnNovoRegistro_Click(object? sender, EventArgs e)
        {
            if (perfilAtual == null)
            {
                MessageBox.Show("Crie um perfil antes de adicionar registros.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abre o RegistroForm passando o perfilAtual
            using (var registroForm = new RegistroForm(perfilAtual))
            {
                registroForm.ShowDialog();
                // Aqui você pode atualizar a tela ou gráfico se quiser
            }
        }
    }
}
