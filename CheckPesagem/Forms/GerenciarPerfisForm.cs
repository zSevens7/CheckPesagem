using System;
using System.Drawing;
using System.Windows.Forms;
using CheckPesagem.Models;
using CheckPesagem.Services;

namespace CheckPesagem.Forms
{
    public class GerenciarPerfisForm : Form
    {
        private ListBox lstPerfis;
        private Button btnSelecionar;
        private Button btnDefinirAtual;
        private Button btnExcluir;
        private Button btnFechar;
        private Button btnTema;
        private bool temaEscuro = false;

        public Perfil? PerfilSelecionado { get; private set; }

        public GerenciarPerfisForm()
        {
            // INICIALIZAR TODOS OS CONTROLES NO CONSTRUTOR
            lstPerfis = new ListBox();
            btnSelecionar = new Button();
            btnDefinirAtual = new Button();
            btnExcluir = new Button();
            btnFechar = new Button();
            btnTema = new Button();

            this.Text = "Gerenciar Perfis";
            this.Width = 460;
            this.Height = 430;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InicializarComponentes();
            AplicarTema();
            CarregarPerfis();
        }

        private void InicializarComponentes()
        {
            int topInicial = 20;
            int inputLeft = 20;
            int inputWidth = 400;

            // --- Título ---
            Label lblTitulo = new Label
            {
                Text = "Perfis disponíveis:",
                Left = 20,
                Top = topInicial,
                Width = 300,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            // --- Lista de perfis ---
            lstPerfis.Left = inputLeft;
            lstPerfis.Top = topInicial + 30;
            lstPerfis.Width = inputWidth;
            lstPerfis.Height = 200;
            lstPerfis.Font = new Font("Segoe UI", 10);

            // --- Botões ---
            btnSelecionar.Text = "Carregar Perfil";
            btnSelecionar.Left = 20;
            btnSelecionar.Top = 250;
            btnSelecionar.Width = 120;
            btnSelecionar.Height = 40;
            btnSelecionar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSelecionar.FlatStyle = FlatStyle.Flat;
            btnSelecionar.BackColor = Color.LightGray;
            btnSelecionar.Click += BtnSelecionar_Click;

            btnDefinirAtual.Text = "Definir Atual";
            btnDefinirAtual.Left = 160;
            btnDefinirAtual.Top = 250;
            btnDefinirAtual.Width = 120;
            btnDefinirAtual.Height = 40;
            btnDefinirAtual.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDefinirAtual.FlatStyle = FlatStyle.Flat;
            btnDefinirAtual.BackColor = Color.LightGray;
            btnDefinirAtual.Click += BtnDefinirAtual_Click;

            btnExcluir.Text = "Excluir";
            btnExcluir.Left = 300;
            btnExcluir.Top = 250;
            btnExcluir.Width = 120;
            btnExcluir.Height = 40;
            btnExcluir.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnExcluir.FlatStyle = FlatStyle.Flat;
            btnExcluir.BackColor = Color.LightGray;
            btnExcluir.Click += BtnExcluir_Click;

            btnFechar.Text = "Fechar";
            btnFechar.Left = 160;
            btnFechar.Top = 300;
            btnFechar.Width = 120;
            btnFechar.Height = 40;
            btnFechar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnFechar.FlatStyle = FlatStyle.Flat;
            btnFechar.BackColor = Color.LightGray;
            btnFechar.Click += (s, e) => this.Close();

            // --- Botão tema ---
            btnTema.Text = "🌙";
            btnTema.Width = 40;
            btnTema.Height = 40;
            btnTema.Top = 10;
            btnTema.Left = this.Width - 80;
            btnTema.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTema.FlatStyle = FlatStyle.Flat;
            btnTema.Font = new Font("Segoe UI", 10);
            btnTema.FlatAppearance.BorderSize = 0;
            btnTema.Click += AlternarTema;

            // Adicionar controles
            Controls.Add(lblTitulo);
            Controls.Add(lstPerfis);
            Controls.Add(btnSelecionar);
            Controls.Add(btnDefinirAtual);
            Controls.Add(btnExcluir);
            Controls.Add(btnFechar);
            Controls.Add(btnTema);
        }

        private void CarregarPerfis()
        {
            lstPerfis.Items.Clear();
            var perfis = PerfilService.ListarPerfis();

            if (perfis.Count == 0)
            {
                lstPerfis.Items.Add("Nenhum perfil encontrado.");
                lstPerfis.Enabled = false;
                btnSelecionar.Enabled = false;
                btnExcluir.Enabled = false;
                btnDefinirAtual.Enabled = false;
            }
            else
            {
                lstPerfis.Enabled = true;
                btnSelecionar.Enabled = true;
                btnExcluir.Enabled = true;
                btnDefinirAtual.Enabled = true;

                foreach (var nome in perfis)
                    lstPerfis.Items.Add(nome);
            }
        }

        private void BtnSelecionar_Click(object? sender, EventArgs e)
        {
            if (lstPerfis.SelectedItem == null)
            {
                MessageBox.Show("Selecione um perfil para continuar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string nome = lstPerfis.SelectedItem.ToString()!;
            var perfil = PerfilService.CarregarPerfil(nome);
            if (perfil != null)
            {
                PerfilSelecionado = perfil;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnDefinirAtual_Click(object? sender, EventArgs e)
        {
            if (lstPerfis.SelectedItem == null)
            {
                MessageBox.Show("Selecione um perfil para definir como atual.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string nome = lstPerfis.SelectedItem.ToString()!;
            var perfil = PerfilService.CarregarPerfil(nome);
            if (perfil != null)
            {
                PerfilService.SalvarPerfilAtual(perfil);
                MessageBox.Show($"O perfil '{nome}' foi definido como o perfil atual.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnExcluir_Click(object? sender, EventArgs e)
        {
            if (lstPerfis.SelectedItem == null)
            {
                MessageBox.Show("Selecione um perfil para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nome = lstPerfis.SelectedItem.ToString()!;
            var confirmar = MessageBox.Show($"Deseja realmente excluir o perfil '{nome}'?", "Confirmação",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmar == DialogResult.Yes)
            {
                if (PerfilService.ExcluirPerfil(nome))
                {
                    MessageBox.Show("Perfil excluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarPerfis();
                }
                else
                {
                    MessageBox.Show("Erro ao excluir perfil.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AlternarTema(object? sender, EventArgs e)
        {
            temaEscuro = !temaEscuro;
            AplicarTema();
        }

        private void AplicarTema()
        {
            if (temaEscuro)
            {
                this.BackColor = Color.FromArgb(30, 30, 30);
                this.ForeColor = Color.WhiteSmoke;
                btnTema.Text = "☀️";
            }
            else
            {
                this.BackColor = Color.WhiteSmoke;
                this.ForeColor = Color.Black;
                btnTema.Text = "🌙";
            }

            foreach (Control c in Controls)
            {
                if (c is TextBox tb) tb.BackColor = temaEscuro ? Color.FromArgb(45, 45, 45) : Color.White;
                if (c is Button b)
                {
                    b.BackColor = temaEscuro ? Color.FromArgb(45, 45, 45) : Color.LightGray;
                    b.ForeColor = temaEscuro ? Color.WhiteSmoke : Color.Black;
                    b.FlatStyle = temaEscuro ? FlatStyle.Flat : FlatStyle.Standard;
                    if (temaEscuro) b.FlatAppearance.BorderSize = 0;
                }
                if (c is ListBox lb)
                {
                    lb.BackColor = temaEscuro ? Color.FromArgb(45, 45, 45) : Color.White;
                    lb.ForeColor = temaEscuro ? Color.WhiteSmoke : Color.Black;
                }
            }
        }
    }
}