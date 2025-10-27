using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CheckPesagem.Models;
using CheckPesagem.Controls;
using CheckPesagem.Services;

namespace CheckPesagem.Forms
{
    public class MainForm : Form
    {
        private Perfil? perfilAtual;

        // Declare os controles como anuláveis
        private Button? btnNovoPerfil;
        private Button? btnGerenciarPerfis;
        private Button? btnNovoRegistro;
        private Button? btnTema;
        private Button? btnAjuda;

        private Panel? pnlPerfil;
        private Label? lblPerfilNome;
        private Label? lblIMC;
        private Label? lblSituacaoIMC;

        private TabelaRegistrosControl? tabelaControl;
        private bool temaEscuro = false;

        public MainForm()
        {
            // Inicializa TODOS os controles no construtor
            btnNovoPerfil = new Button();
            btnGerenciarPerfis = new Button();
            btnNovoRegistro = new Button();
            btnTema = new Button();
            btnAjuda = new Button();
            pnlPerfil = new Panel();
            lblPerfilNome = new Label();
            lblIMC = new Label();
            lblSituacaoIMC = new Label();
            
            CriarInterface();
            CriarPainelPerfil();
            CarregarPerfilAtual();
        }

        private void CriarInterface()
        {
            this.Text = "CheckPesagem - Controle de Peso";
            this.Width = 1280;
            this.Height = 720;
            this.MinimumSize = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Configuração dos botões
            if (btnNovoPerfil != null)
            {
                btnNovoPerfil.Text = "Criar Perfil";
                btnNovoPerfil.Width = 120;
                btnNovoPerfil.Height = 40;
                btnNovoPerfil.Top = 30;
                btnNovoPerfil.Left = 30;
                btnNovoPerfil.Click += BtnNovoPerfil_Click;
                Controls.Add(btnNovoPerfil);
            }

            if (btnGerenciarPerfis != null)
            {
                btnGerenciarPerfis.Text = "Gerenciar Perfis";
                btnGerenciarPerfis.Width = 150;
                btnGerenciarPerfis.Height = 40;
                btnGerenciarPerfis.Top = 30;
                btnGerenciarPerfis.Left = 170;
                btnGerenciarPerfis.Click += BtnGerenciarPerfis_Click;
                Controls.Add(btnGerenciarPerfis);
            }

            if (btnNovoRegistro != null)
            {
                btnNovoRegistro.Text = "Adicionar Registro";
                btnNovoRegistro.Width = 150;
                btnNovoRegistro.Height = 40;
                btnNovoRegistro.Top = 30;
                btnNovoRegistro.Left = 340;
                btnNovoRegistro.Click += BtnNovoRegistro_Click;
                Controls.Add(btnNovoRegistro);
            }

            if (btnTema != null)
            {
                btnTema.Text = "🌙";
                btnTema.Width = 40;
                btnTema.Height = 40;
                btnTema.Top = 30;
                btnTema.Left = this.Width - 200;
                btnTema.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnTema.Click += AlternarTema;
                Controls.Add(btnTema);
            }

            if (btnAjuda != null)
            {
                btnAjuda.Text = "❓ Ajuda";
                btnAjuda.Width = 100;
                btnAjuda.Height = 40;
                btnAjuda.Top = 30;
                btnAjuda.Left = this.Width - 140;
                btnAjuda.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnAjuda.Click += BtnAjuda_Click;
                Controls.Add(btnAjuda);
            }

            AplicarTema();
        }

        private void CriarPainelPerfil()
        {
            if (pnlPerfil == null || lblPerfilNome == null || lblIMC == null || lblSituacaoIMC == null) return;

            pnlPerfil.Left = 30;
            pnlPerfil.Top = 100;
            pnlPerfil.Width = this.ClientSize.Width - 60;
            pnlPerfil.Height = 60;
            pnlPerfil.BackColor = Color.LightGray;

            pnlPerfil.Paint += (s, e) =>
            {
                GraphicsPath gp = new GraphicsPath();
                int radius = 20;
                gp.AddArc(0, 0, radius, radius, 180, 90);
                gp.AddArc(pnlPerfil.Width - radius, 0, radius, radius, 270, 90);
                gp.AddArc(pnlPerfil.Width - radius, pnlPerfil.Height - radius, radius, radius, 0, 90);
                gp.AddArc(0, pnlPerfil.Height - radius, radius, radius, 90, 90);
                gp.CloseAllFigures();
                pnlPerfil.Region = new Region(gp);
            };

            lblPerfilNome.Left = 10;
            lblPerfilNome.Top = 15;
            lblPerfilNome.AutoSize = true;
            lblPerfilNome.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            lblIMC.Left = 300;
            lblIMC.Top = 15;
            lblIMC.AutoSize = true;
            lblIMC.Font = new Font("Segoe UI", 12);

            lblSituacaoIMC.Left = 500;
            lblSituacaoIMC.Top = 15;
            lblSituacaoIMC.AutoSize = true;
            lblSituacaoIMC.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            pnlPerfil.Controls.Add(lblPerfilNome);
            pnlPerfil.Controls.Add(lblIMC);
            pnlPerfil.Controls.Add(lblSituacaoIMC);
            Controls.Add(pnlPerfil);
        }

        private void CarregarPerfilAtual()
        {
            perfilAtual = PerfilService.CarregarPerfilAtual();
            AtualizarPerfilNaTela();
            
            if (perfilAtual != null)
            {
                MostrarTabela();
            }
        }

        private void MostrarTabela()
        {
            if (perfilAtual == null) return;

            // Remove o controle anterior se existir
            if (tabelaControl != null)
            {
                Controls.Remove(tabelaControl);
                tabelaControl.Dispose();
                tabelaControl = null;
            }

            // Cria novo controle passando o perfil atual
            tabelaControl = new TabelaRegistrosControl(perfilAtual)
            {
                Location = new Point(30, 180),
                Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            
            Controls.Add(tabelaControl);
            tabelaControl.BringToFront();
        }

        private void BtnNovoPerfil_Click(object? sender, EventArgs e)
        {
            using (var perfilForm = new PerfilForm())
            {
                perfilForm.ShowDialog();
                if (perfilForm.PerfilCriado != null)
                {
                    perfilAtual = perfilForm.PerfilCriado;
                    AtualizarPerfilNaTela();
                    MostrarTabela();
                }
            }
        }

        private void BtnGerenciarPerfis_Click(object? sender, EventArgs e)
        {
            using (var gerenciarForm = new GerenciarPerfisForm())
            {
                if (gerenciarForm.ShowDialog() == DialogResult.OK && gerenciarForm.PerfilSelecionado != null)
                {
                    perfilAtual = gerenciarForm.PerfilSelecionado;
                    AtualizarPerfilNaTela();
                    MostrarTabela();
                }
            }
        }

        private void BtnNovoRegistro_Click(object? sender, EventArgs e)
        {
            if (perfilAtual == null)
            {
                MessageBox.Show("Crie ou selecione um perfil antes de adicionar registros.\nUse vírgula para separar decimais, ex: 1,70 m",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var registroForm = new RegistroForm(perfilAtual))
            {
                registroForm.ShowDialog();
                MostrarTabela();
            }
        }

        private void BtnAjuda_Click(object? sender, EventArgs e)
        {
            string msg = "Bem-vindo ao CheckPesagem!\n\n" +
                         "Fluxo inicial:\n" +
                         "1. Se não houver perfil, clique em 'Criar Perfil'.\n" +
                         "2. Caso já tenha perfil, clique em 'Gerenciar Perfis' -> Selecione o perfil e clique em 'Carregar'.\n" +
                         "3. Para adicionar registros, clique em 'Adicionar Registro'.\n\n" +
                         "Observações:\n" +
                         "- Use vírgula para números decimais (ex: 1,70 m, 65,5 kg).\n" +
                         "- O IMC será mostrado com situação e cores correspondentes.";

            MessageBox.Show(msg, "Ajuda / Sobre", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AtualizarPerfilNaTela()
        {
            if (perfilAtual == null || lblPerfilNome == null || lblIMC == null || lblSituacaoIMC == null || pnlPerfil == null)
            {
                if (lblPerfilNome != null) lblPerfilNome.Text = "Nenhum perfil carregado.";
                if (lblIMC != null) lblIMC.Text = "";
                if (lblSituacaoIMC != null) lblSituacaoIMC.Text = "";
                if (pnlPerfil != null) pnlPerfil.BackColor = Color.LightGray;
            }
            else
            {
                lblPerfilNome.Text = $"{perfilAtual.Nome} ({perfilAtual.Idade} anos)";
                double imc = perfilAtual.CalcularIMC();
                string classificacao = perfilAtual.ClassificarIMC(imc);
                
                lblIMC.Text = $"IMC = {imc:0.00}";
                lblSituacaoIMC.Text = $"Classificação: {classificacao}";
                
                Color cor = ObterCorIMC(imc);
                pnlPerfil.BackColor = cor;
            }
        }

        private Color ObterCorIMC(double imc)
        {
            if (imc < 17) return Color.Gray;
            if (imc < 18.5) return Color.LightGreen;
            if (imc < 25) return Color.Green;
            if (imc < 30) return Color.Yellow;
            if (imc < 35) return Color.OrangeRed;
            if (imc < 40) return Color.Red;
            return Color.DarkRed;
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
                if (btnTema != null) btnTema.Text = "☀️";
                foreach (Control c in Controls)
                {
                    if (c is Button b)
                    {
                        b.BackColor = Color.FromArgb(45, 45, 45);
                        b.ForeColor = Color.WhiteSmoke;
                        b.FlatStyle = FlatStyle.Flat;
                        b.FlatAppearance.BorderSize = 0;
                    }
                }
            }
            else
            {
                this.BackColor = Color.WhiteSmoke;
                this.ForeColor = Color.Black;
                if (btnTema != null) btnTema.Text = "🌙";
                foreach (Control c in Controls)
                {
                    if (c is Button b)
                    {
                        b.BackColor = SystemColors.Control;
                        b.ForeColor = Color.Black;
                        b.FlatStyle = FlatStyle.Standard;
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Atualiza posição dos botões de tema e ajuda
            if (btnTema != null) btnTema.Left = this.ClientSize.Width - 200;
            if (btnAjuda != null) btnAjuda.Left = this.ClientSize.Width - 140;
            
            // Atualiza largura do painel do perfil
            if (pnlPerfil != null)
            {
                pnlPerfil.Width = this.ClientSize.Width - 60;
            }
            
            // Atualiza tamanho da tabela se existir
            if (tabelaControl != null)
            {
                tabelaControl.Location = new Point(30, 180);
                tabelaControl.Size = new Size(this.ClientSize.Width - 60, this.ClientSize.Height - 220);
            }
        }
    }
}