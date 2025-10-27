using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CheckPesagem.Models;
using CheckPesagem.Services;

namespace CheckPesagem.Forms
{
    public class PerfilForm : Form
    {
        public Perfil? PerfilCriado { get; private set; }

        private TextBox? txtNome;
        private DateTimePicker? dtpNascimento;
        private TextBox? txtAltura;
        private TextBox? txtPeso;
        private TextBox? txtCintura;
        private Button? btnSalvar;
        private Button? btnTema;
        private bool temaEscuro = false;

        public PerfilForm()
        {
            this.Text = "Criar/Editar Perfil";
            this.Width = 500;
            this.Height = 420;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InicializarComponentes();
            AplicarTema();
        }

        private void InicializarComponentes()
        {
            string caminhoBase = Path.Combine(Application.StartupPath, "Assets", "Forms");

            int labelLeft = 60;
            int inputLeft = 200;
            int topInicial = 20;
            int espacamento = 45;
            int labelWidth = 140;
            int inputWidth = 200;
            int iconeSize = 30;

            // --- Nome ---
            var picNome = new PictureBox
            {
                Left = 20,
                Top = topInicial,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(Path.Combine(caminhoBase, "Nome.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblNome = new Label { Text = "Nome completo:", Left = labelLeft, Top = topInicial + 5, Width = labelWidth, Font = new Font("Segoe UI", 10) };
            txtNome = new TextBox { Left = inputLeft, Top = topInicial, Width = inputWidth, Font = new Font("Segoe UI", 10) };

            // --- Nascimento ---
            var picNascimento = new PictureBox
            {
                Left = 20,
                Top = topInicial + espacamento,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(Path.Combine(caminhoBase, "Nascimento.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblNascimento = new Label { Text = "Data de Nascimento:", Left = labelLeft, Top = topInicial + espacamento + 5, Width = labelWidth, Font = new Font("Segoe UI", 10) };
            dtpNascimento = new DateTimePicker { Left = inputLeft, Top = topInicial + espacamento, Width = inputWidth, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today, Font = new Font("Segoe UI", 10) };

            // --- Altura ---
            var picAltura = new PictureBox
            {
                Left = 20,
                Top = topInicial + espacamento * 2,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(Path.Combine(caminhoBase, "Altura.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblAltura = new Label { Text = "Altura (m):", Left = labelLeft, Top = topInicial + espacamento * 2 + 5, Width = labelWidth, Font = new Font("Segoe UI", 10) };
            txtAltura = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 2, Width = inputWidth, Font = new Font("Segoe UI", 10) };

            // --- Peso ---
            var picPeso = new PictureBox
            {
                Left = 20,
                Top = topInicial + espacamento * 3,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(Path.Combine(caminhoBase, "Peso.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblPeso = new Label { Text = "Peso (kg):", Left = labelLeft, Top = topInicial + espacamento * 3 + 5, Width = labelWidth, Font = new Font("Segoe UI", 10) };
            txtPeso = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 3, Width = inputWidth, Font = new Font("Segoe UI", 10) };

            // --- Cintura ---
            var picCintura = new PictureBox
            {
                Left = 20,
                Top = topInicial + espacamento * 4,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(Path.Combine(caminhoBase, "Cintura.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblCintura = new Label { Text = "Cintura (cm, opcional):", Left = labelLeft, Top = topInicial + espacamento * 4 + 5, Width = labelWidth, Font = new Font("Segoe UI", 10) };
            txtCintura = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 4, Width = inputWidth, Font = new Font("Segoe UI", 10) };

            // --- Botão Salvar ---
            btnSalvar = new Button
            {
                Text = "💾 Salvar",
                Left = inputLeft,
                Top = topInicial + espacamento * 5 + 10,
                Width = 120,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSalvar.Click += BtnSalvar_Click;

            // --- Botão Tema ---
            btnTema = new Button
            {
                Text = "🌙",
                Width = 40,
                Height = 40,
                Top = 10,
                Left = this.Width - 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnTema.Click += AlternarTema;

            Controls.Add(picNome); Controls.Add(lblNome); Controls.Add(txtNome);
            Controls.Add(picNascimento); Controls.Add(lblNascimento); Controls.Add(dtpNascimento);
            Controls.Add(picAltura); Controls.Add(lblAltura); Controls.Add(txtAltura);
            Controls.Add(picPeso); Controls.Add(lblPeso); Controls.Add(txtPeso);
            Controls.Add(picCintura); Controls.Add(lblCintura); Controls.Add(txtCintura);
            Controls.Add(btnSalvar); Controls.Add(btnTema);
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (txtNome == null || dtpNascimento == null || txtAltura == null || txtPeso == null || txtCintura == null)
                    throw new Exception("Campos não inicializados corretamente.");

                string nome = txtNome.Text.Trim();
                if (string.IsNullOrWhiteSpace(nome))
                    throw new Exception("Informe um nome válido.");

                if (!double.TryParse(txtAltura.Text, out double altura))
                    throw new Exception("Altura inválida.");
                if (!double.TryParse(txtPeso.Text, out double peso))
                    throw new Exception("Peso inválido.");

                double? cintura = null;
                if (!string.IsNullOrWhiteSpace(txtCintura.Text))
                {
                    if (!double.TryParse(txtCintura.Text, out double c))
                        throw new Exception("Cintura inválida.");
                    cintura = c;
                }

                var perfil = new Perfil { Nome = nome, DataNascimento = dtpNascimento.Value, Altura = altura, PesoAtual = peso, Cintura = cintura };

                PerfilService.SalvarPerfil(perfil);
                PerfilCriado = perfil;

                MessageBox.Show("Perfil salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar perfil: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                btnTema!.Text = "☀️";
            }
            else
            {
                this.BackColor = Color.WhiteSmoke;
                this.ForeColor = Color.Black;
                btnTema!.Text = "🌙";
            }

            foreach (Control c in Controls)
            {
                if (c is TextBox tb) tb.BackColor = temaEscuro ? Color.FromArgb(45, 45, 45) : Color.White;
                if (c is Button b)
                {
                    b.BackColor = temaEscuro ? Color.FromArgb(45, 45, 45) : SystemColors.Control;
                    b.ForeColor = temaEscuro ? Color.WhiteSmoke : Color.Black;
                    b.FlatStyle = temaEscuro ? FlatStyle.Flat : FlatStyle.Standard;
                    if (temaEscuro) b.FlatAppearance.BorderSize = 0;
                }
            }
        }
    }
}
