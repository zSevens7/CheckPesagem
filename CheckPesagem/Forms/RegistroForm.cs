using System;
using System.Drawing;
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
        private TextBox txtPeso;
        private TextBox txtCintura;
        private Button btnSalvar;

        public RegistroForm(Perfil perfil)
        {
            this.perfil = perfil;

            // INICIALIZAR TODOS OS CONTROLES NO CONSTRUTOR
            dtpData = new DateTimePicker();
            txtPeso = new TextBox();
            txtCintura = new TextBox();
            btnSalvar = new Button();

            this.Text = "Adicionar Registro Diário";
            this.Width = 480;
            this.Height = 350;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            int labelLeft = 60;
            int inputLeft = 200;
            int topInicial = 20;
            int espacamento = 50;
            int labelWidth = 140;
            int inputWidth = 200;
            int iconeSize = 30;

            // Caminho base para os ícones
            string basePath = System.IO.Path.Combine(Application.StartupPath, "Assets", "Forms");

            // --- Data ---
            var picData = new PictureBox
            {
                Left = 20,
                Top = topInicial,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(System.IO.Path.Combine(basePath, "Data.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblData = new Label
            {
                Text = "Data:",
                Left = labelLeft,
                Top = topInicial + 5,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10)
            };
            
            dtpData.Left = inputLeft;
            dtpData.Top = topInicial;
            dtpData.Width = inputWidth;
            dtpData.Format = DateTimePickerFormat.Short;
            dtpData.MaxDate = DateTime.Today;
            dtpData.Font = new Font("Segoe UI", 10);

            // --- Peso ---
            var picPeso = new PictureBox
            {
                Left = 20,
                Top = topInicial + espacamento,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(System.IO.Path.Combine(basePath, "Peso.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblPeso = new Label
            {
                Text = "Peso (kg):",
                Left = labelLeft,
                Top = topInicial + espacamento + 5,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10)
            };
            
            txtPeso.Left = inputLeft;
            txtPeso.Top = topInicial + espacamento;
            txtPeso.Width = inputWidth;
            txtPeso.Font = new Font("Segoe UI", 10);

            // --- Cintura ---
            var picCintura = new PictureBox
            {
                Left = 20,
                Top = topInicial + espacamento * 2,
                Width = iconeSize,
                Height = iconeSize,
                Image = Image.FromFile(System.IO.Path.Combine(basePath, "Cintura.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Label lblCintura = new Label
            {
                Text = "Cintura (cm, opcional):",
                Left = labelLeft,
                Top = topInicial + espacamento * 2 + 5,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10)
            };
            
            txtCintura.Left = inputLeft;
            txtCintura.Top = topInicial + espacamento * 2;
            txtCintura.Width = inputWidth;
            txtCintura.Font = new Font("Segoe UI", 10);

            // --- Botão Salvar ---
            btnSalvar.Text = "💾 Salvar";
            btnSalvar.Left = inputLeft;
            btnSalvar.Top = topInicial + espacamento * 3 + 10;
            btnSalvar.Width = 120;
            btnSalvar.Height = 40;
            btnSalvar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSalvar.Click += BtnSalvar_Click;

            // Adiciona os controles
            Controls.Add(picData);
            Controls.Add(lblData);
            Controls.Add(dtpData);
            Controls.Add(picPeso);
            Controls.Add(lblPeso);
            Controls.Add(txtPeso);
            Controls.Add(picCintura);
            Controls.Add(lblCintura);
            Controls.Add(txtCintura);
            Controls.Add(btnSalvar);
        }

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(txtPeso.Text, out double peso))
                    throw new Exception("Peso inválido.");

                double? cintura = null;
                if (!string.IsNullOrWhiteSpace(txtCintura.Text))
                {
                    if (!double.TryParse(txtCintura.Text, out double c))
                        throw new Exception("Cintura inválida.");
                    cintura = c;
                }

                var registro = new Registro
                {
                    Data = dtpData.Value.Date,
                    Peso = peso,
                    Cintura = cintura
                };

                // ADICIONA O REGISTRO AO PERFIL
                perfil.Registros.Add(registro);
                
                // ATUALIZA O PESO ATUAL DO PERFIL
                perfil.PesoAtual = peso;
                perfil.Cintura = cintura;

                // SALVA O PERFIL ATUALIZADO
                PerfilService.SalvarPerfil(perfil);
                
                // Se este for o perfil atual, atualiza também
                var perfilAtual = PerfilService.CarregarPerfilAtual();
                if (perfilAtual != null && perfilAtual.Nome == perfil.Nome)
                {
                    PerfilService.SalvarPerfilAtual(perfil);
                }

                RegistroCriado = registro;

                MessageBox.Show("Registro salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}