using System;
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

        public PerfilForm()
        {
            this.Text = "Criar/Editar Perfil";
            this.Width = 450;
            this.Height = 360;
            this.StartPosition = FormStartPosition.CenterParent;

            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            int labelLeft = 20;
            int inputLeft = 200;
            int topInicial = 20;
            int espacamento = 40;
            int labelWidth = 180;
            int inputWidth = 200;

            Label lblNome = new Label { Text = "Nome:", Left = labelLeft, Top = topInicial, Width = labelWidth };
            txtNome = new TextBox { Left = inputLeft, Top = topInicial, Width = inputWidth };

            Label lblNascimento = new Label { Text = "Data de Nascimento:", Left = labelLeft, Top = topInicial + espacamento, Width = labelWidth };
            dtpNascimento = new DateTimePicker
            {
                Left = inputLeft,
                Top = topInicial + espacamento,
                Width = inputWidth,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };

            Label lblAltura = new Label { Text = "Altura (m):", Left = labelLeft, Top = topInicial + espacamento * 2, Width = labelWidth };
            txtAltura = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 2, Width = inputWidth };

            Label lblPeso = new Label { Text = "Peso (kg):", Left = labelLeft, Top = topInicial + espacamento * 3, Width = labelWidth };
            txtPeso = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 3, Width = inputWidth };

            Label lblCintura = new Label { Text = "Cintura (cm, opcional):", Left = labelLeft, Top = topInicial + espacamento * 4, Width = labelWidth };
            txtCintura = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 4, Width = inputWidth };

            btnSalvar = new Button { Text = "Salvar", Left = inputLeft, Top = topInicial + espacamento * 5 + 10, Width = 100 };
            btnSalvar.Click += BtnSalvar_Click;

            this.Controls.Add(lblNome);
            this.Controls.Add(txtNome);
            this.Controls.Add(lblNascimento);
            this.Controls.Add(dtpNascimento);
            this.Controls.Add(lblAltura);
            this.Controls.Add(txtAltura);
            this.Controls.Add(lblPeso);
            this.Controls.Add(txtPeso);
            this.Controls.Add(lblCintura);
            this.Controls.Add(txtCintura);
            this.Controls.Add(btnSalvar);
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (txtNome == null || dtpNascimento == null || txtAltura == null || txtPeso == null || txtCintura == null)
                    throw new Exception("Campos não inicializados corretamente.");

                // Parse seguro
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

                var perfil = new Perfil
                {
                    Nome = txtNome.Text,
                    DataNascimento = dtpNascimento.Value,
                    Altura = altura,
                    PesoAtual = peso,
                    Cintura = cintura
                };

                // Salvar JSON
                JsonService.Salvar("perfil_atual.json", perfil);

                PerfilCriado = perfil;

                MessageBox.Show("Perfil salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
