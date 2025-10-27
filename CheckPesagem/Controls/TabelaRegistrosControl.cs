using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CheckPesagem.Models;
using CheckPesagem.Services;
using CheckPesagem.Forms;

namespace CheckPesagem.Controls
{
    public class TabelaRegistrosControl : UserControl
    {
        private DataGridView dgvRegistros;
        private Perfil perfilAtual;
        private Panel pnlFiltros;
        private ComboBox cbPeriodo;
        private ComboBox cbAno;
        private ComboBox cbMes;
        private Button btnVerGraficos;
        private Label lblEstatisticas;

        public TabelaRegistrosControl(Perfil perfil)
        {
            perfilAtual = perfil ?? throw new ArgumentNullException(nameof(perfil));
            
            // Inicializar todos os controles
            dgvRegistros = new DataGridView();
            pnlFiltros = new Panel();
            cbPeriodo = new ComboBox();
            cbAno = new ComboBox();
            cbMes = new ComboBox();
            btnVerGraficos = new Button();
            lblEstatisticas = new Label();
            
            InitializeComponent();
            CarregarRegistros();
            AtualizarEstatisticas();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            CriarFiltros();
            CriarTabela();
        }

        private void CriarFiltros()
        {
            pnlFiltros.Height = 80;
            pnlFiltros.Dock = DockStyle.Top;
            pnlFiltros.BackColor = Color.FromArgb(250, 250, 250);
            pnlFiltros.BorderStyle = BorderStyle.None;

            // Label de filtro
            var lblFiltro = new Label
            {
                Text = "Filtrar por:",
                Location = new Point(20, 15),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70)
            };

            // ComboBox de período
            cbPeriodo.Location = new Point(100, 12);
            cbPeriodo.Size = new Size(150, 25);
            cbPeriodo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPeriodo.Font = new Font("Segoe UI", 9);
            cbPeriodo.Items.AddRange(new object[] { "Todos os registros", "Últimos 30 dias", "Últimos 90 dias", "Este ano", "Ano específico", "Mês específico" });
            cbPeriodo.SelectedIndex = 0;
            cbPeriodo.SelectedIndexChanged += FiltroChanged;

            // ComboBox de ano
            cbAno.Location = new Point(260, 12);
            cbAno.Size = new Size(100, 25);
            cbAno.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAno.Font = new Font("Segoe UI", 9);
            cbAno.Visible = false;
            cbAno.SelectedIndexChanged += FiltroChanged;

            // ComboBox de mês
            cbMes.Location = new Point(370, 12);
            cbMes.Size = new Size(120, 25);
            cbMes.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMes.Font = new Font("Segoe UI", 9);
            cbMes.Visible = false;
            cbMes.Items.AddRange(new object[] { 
                "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
            });
            cbMes.SelectedIndexChanged += FiltroChanged;

            // Botão de gráficos
            btnVerGraficos.Text = "📊 Ver Gráficos";
            btnVerGraficos.Location = new Point(500, 10);
            btnVerGraficos.Size = new Size(120, 30);
            btnVerGraficos.BackColor = Color.FromArgb(70, 130, 180);
            btnVerGraficos.ForeColor = Color.White;
            btnVerGraficos.FlatStyle = FlatStyle.Flat;
            btnVerGraficos.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnVerGraficos.Cursor = Cursors.Hand;
            btnVerGraficos.FlatAppearance.BorderSize = 0;
            btnVerGraficos.Click += BtnVerGraficos_Click;

            // Label de estatísticas
            lblEstatisticas.Location = new Point(20, 45);
            lblEstatisticas.Size = new Size(600, 20);
            lblEstatisticas.Font = new Font("Segoe UI", 8);
            lblEstatisticas.ForeColor = Color.FromArgb(100, 100, 100);

            pnlFiltros.Controls.AddRange(new Control[] { 
                lblFiltro, cbPeriodo, cbAno, cbMes, btnVerGraficos, lblEstatisticas 
            });
            Controls.Add(pnlFiltros);
        }

        private void CriarTabela()
        {
            dgvRegistros.Dock = DockStyle.Fill;
            dgvRegistros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRegistros.AllowUserToAddRows = false;
            dgvRegistros.ReadOnly = true;
            dgvRegistros.BackgroundColor = Color.White;
            dgvRegistros.RowHeadersVisible = false;
            dgvRegistros.Font = new Font("Segoe UI", 9);
            dgvRegistros.BorderStyle = BorderStyle.None;
            dgvRegistros.GridColor = Color.FromArgb(240, 240, 240);
            dgvRegistros.DefaultCellStyle = new DataGridViewCellStyle
            {
                Padding = new Padding(5),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9)
            };
            dgvRegistros.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvRegistros.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 245, 245)
            };
            dgvRegistros.RowTemplate.Height = 35;

            Controls.Add(dgvRegistros);
            dgvRegistros.BringToFront();
        }

        private void FiltroChanged(object? sender, EventArgs e)
        {
            // Mostrar/ocultar controles baseado no período selecionado
            switch (cbPeriodo.SelectedIndex)
            {
                case 4: // Ano específico
                    cbAno.Visible = true;
                    cbMes.Visible = false;
                    CarregarAnos();
                    break;
                case 5: // Mês específico
                    cbAno.Visible = true;
                    cbMes.Visible = true;
                    CarregarAnos();
                    break;
                default:
                    cbAno.Visible = false;
                    cbMes.Visible = false;
                    break;
            }

            CarregarRegistros();
            AtualizarEstatisticas();
        }

        private void CarregarAnos()
        {
            if (perfilAtual?.Registros == null) return;

            var anos = perfilAtual.Registros
                .Select(r => r.Data.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            cbAno.Items.Clear();
            foreach (var ano in anos)
            {
                cbAno.Items.Add(ano);
            }

            if (cbAno.Items.Count > 0)
                cbAno.SelectedIndex = 0;
        }

        private void CarregarRegistros()
        {
            dgvRegistros.Columns.Clear();
            dgvRegistros.Rows.Clear();

            if (perfilAtual?.Registros == null || !perfilAtual.Registros.Any())
            {
                dgvRegistros.Columns.Add("Mensagem", "Mensagem");
                dgvRegistros.Rows.Add("Nenhum registro encontrado");
                return;
            }

            // Configurar colunas com estilo
            dgvRegistros.Columns.Add("Data", "📅 Data");
            dgvRegistros.Columns.Add("Peso", "⚖️ Peso (kg)");
            dgvRegistros.Columns.Add("Cintura", "📏 Cintura (cm)");
            dgvRegistros.Columns.Add("IMC", "🔢 IMC");
            dgvRegistros.Columns.Add("Classificacao", "🎯 Classificação");

            // Aplicar filtros
            var registrosFiltrados = AplicarFiltros(perfilAtual.Registros);

            foreach (var registro in registrosFiltrados)
            {
                double imc = perfilAtual.CalcularIMC(registro.Peso);
                string classificacao = perfilAtual.ClassificarIMC(imc);
                Color corClassificacao = ObterCorClassificacao(classificacao);

                int rowIndex = dgvRegistros.Rows.Add(
                    registro.Data.ToString("dd/MM/yyyy"),
                    registro.Peso.ToString("F1"),
                    registro.Cintura?.ToString("F1") ?? "N/A",
                    imc.ToString("F2"),
                    classificacao
                );

                // Colorir a célula de classificação
                dgvRegistros.Rows[rowIndex].Cells["Classificacao"].Style.BackColor = corClassificacao;
                dgvRegistros.Rows[rowIndex].Cells["Classificacao"].Style.ForeColor = Color.White;
            }

            // Ajustar estilo das colunas
            foreach (DataGridViewColumn column in dgvRegistros.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private List<Registro> AplicarFiltros(List<Registro> registros)
        {
            var hoje = DateTime.Today;
            var filtrados = registros.OrderByDescending(r => r.Data).ToList();

            switch (cbPeriodo.SelectedIndex)
            {
                case 1: // Últimos 30 dias
                    return filtrados.Where(r => r.Data >= hoje.AddDays(-30)).ToList();
                case 2: // Últimos 90 dias
                    return filtrados.Where(r => r.Data >= hoje.AddDays(-90)).ToList();
                case 3: // Este ano
                    return filtrados.Where(r => r.Data.Year == hoje.Year).ToList();
                case 4: // Ano específico
                    if (cbAno.SelectedItem != null)
                        return filtrados.Where(r => r.Data.Year == (int)cbAno.SelectedItem).ToList();
                    break;
                case 5: // Mês específico
                    if (cbAno.SelectedItem != null && cbMes.SelectedIndex >= 0)
                        return filtrados.Where(r => r.Data.Year == (int)cbAno.SelectedItem && 
                                                   r.Data.Month == cbMes.SelectedIndex + 1).ToList();
                    break;
            }

            return filtrados; // Todos os registros
        }

        private void AtualizarEstatisticas()
        {
            if (perfilAtual?.Registros == null || !perfilAtual.Registros.Any())
            {
                lblEstatisticas.Text = "Nenhum registro para calcular estatísticas";
                return;
            }

            var registros = AplicarFiltros(perfilAtual.Registros);
            if (!registros.Any())
            {
                lblEstatisticas.Text = "Nenhum registro no período selecionado";
                return;
            }

            var pesos = registros.Select(r => r.Peso).ToList();
            double mediaPeso = pesos.Average();
            double variacao = ((pesos.Last() - pesos.First()) / pesos.First()) * 100;

            lblEstatisticas.Text = $"📈 Estatísticas: {registros.Count} registros | " +
                                 $"Média de peso: {mediaPeso:F1}kg | " +
                                 $"Variação: {variacao:+#.##;-#.##;0}%";
        }

        private Color ObterCorClassificacao(string classificacao)
        {
            return classificacao switch
            {
                "Abaixo do peso" => Color.SkyBlue,
                "Peso normal" => Color.LimeGreen,
                "Sobrepeso" => Color.Gold,
                "Obesidade Grau I" => Color.Orange,
                "Obesidade Grau II" => Color.OrangeRed,
                "Obesidade Grau III" => Color.Red,
                _ => Color.Gray
            };
        }

        private void BtnVerGraficos_Click(object? sender, EventArgs e)
        {
            if (perfilAtual == null)
            {
                MessageBox.Show("Nenhum perfil carregado para mostrar gráficos.", "Aviso", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var graficosForm = new GraficosForm(perfilAtual);
            graficosForm.ShowDialog();
        }

        public void AtualizarDados(Perfil perfil)
        {
            perfilAtual = perfil;
            CarregarAnos();
            CarregarRegistros();
            AtualizarEstatisticas();
        }
    }
}