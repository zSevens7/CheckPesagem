using System;
using System.Windows.Forms;
using CheckPesagem.Models;
using CheckPesagem.Services;

namespace CheckPesagem.Forms
{
    public class RegistroForm : Form
    {
        private Perfil perfil; // Guarda o perfil passado
        public Registro? RegistroCriado { get; private set; }

        private DateTimePicker dtpData;
        private TextBox? txtPeso;
        private TextBox? txtCintura;
        private Button? btnSalvar;

        // Construtor que recebe o Perfil
        public RegistroForm(Perfil perfil)
        {
            this.perfil = perfil;

            this.Text = "Adicionar Registro Diário";
            this.Width = 450;   // aumentei a largura
            this.Height = 300;  // aumentei a altura
            this.StartPosition = FormStartPosition.CenterParent;

            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            int labelLeft = 20;
            int inputLeft = 200;   // mais espaço entre label e input
            int topInicial = 20;
            int espacamento = 50;  // espaço entre linhas
            int labelWidth = 160;  // largura da label
            int inputWidth = 200;  // largura do textbox

            Label lblData = new Label { Text = "Data:", Left = labelLeft, Top = topInicial, Width = labelWidth };
            dtpData = new DateTimePicker { Left = inputLeft, Top = topInicial, Width = inputWidth, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today };

            Label lblPeso = new Label { Text = "Peso (kg):", Left = labelLeft, Top = topInicial + espacamento, Width = labelWidth };
            txtPeso = new TextBox { Left = inputLeft, Top = topInicial + espacamento, Width = inputWidth };

            Label lblCintura = new Label { Text = "Cintura (cm, opcional):", Left = labelLeft, Top = topInicial + espacamento * 2, Width = labelWidth };
            txtCintura = new TextBox { Left = inputLeft, Top = topInicial + espacamento * 2, Width = inputWidth };

            btnSalvar = new Button { Text = "Salvar", Left = inputLeft, Top = topInicial + espacamento * 3, Width = 100 };
            btnSalvar.Click += BtnSalvar_Click;

            this.Controls.Add(lblData);
            this.Controls.Add(dtpData);
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
                if (txtPeso == null || txtCintura == null || dtpData == null)
                    throw new Exception("Campos não inicializados corretamente.");

                var registro = new Registro
                {
                    Data = dtpData.Value.Date,
                    Peso = double.Parse(txtPeso.Text),
                    Cintura = string.IsNullOrWhiteSpace(txtCintura.Text) ? null : double.Parse(txtCintura.Text)
                };

                // Salvar JSON
                string caminho = "Data/registros.json";
                JsonService.AdicionarRegistro(caminho, registro);

                RegistroCriado = registro;

                MessageBox.Show("Registro salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Erro: verifique os valores digitados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
