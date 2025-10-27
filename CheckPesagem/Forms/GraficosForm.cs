using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using CheckPesagem.Models;
using CheckPesagem.Services;

namespace CheckPesagem.Forms
{
    public class GraficosForm : Form
    {
        private Perfil perfil;
        private TabControl tabControl;
        private ComboBox cbPeriodoGrafico;

        public GraficosForm(Perfil perfil)
        {
            this.perfil = perfil ?? throw new ArgumentNullException(nameof(perfil));
            
            // Inicializar controles antes de usar
            tabControl = new TabControl();
            cbPeriodoGrafico = new ComboBox();
            
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "📊 Gráficos - CheckPesagem";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Título
            var lblTitulo = new Label
            {
                Text = "Análise Gráfica do Progresso",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Filtro para gráficos
            var lblFiltro = new Label
            {
                Text = "Período:",
                Location = new Point(20, 70),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 9)
            };

            cbPeriodoGrafico.Location = new Point(90, 67);
            cbPeriodoGrafico.Size = new Size(150, 25);
            cbPeriodoGrafico.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPeriodoGrafico.Font = new Font("Segoe UI", 9);
            cbPeriodoGrafico.Items.AddRange(new object[] { "Todos os registros", "Últimos 30 dias", "Últimos 90 dias", "Este ano", "Último ano" });
            cbPeriodoGrafico.SelectedIndex = 0;
            cbPeriodoGrafico.SelectedIndexChanged += (s, e) => AtualizarGraficos();

            // TabControl para diferentes gráficos
            tabControl.Location = new Point(20, 110);
            tabControl.Size = new Size(this.Width - 60, this.Height - 180);
            tabControl.Font = new Font("Segoe UI", 10);

            // Aba de Peso
            var tabPeso = new TabPage("⚖️ Evolução do Peso");
            tabPeso.BackColor = Color.White;
            tabControl.TabPages.Add(tabPeso);

            // Aba de IMC
            var tabIMC = new TabPage("🔢 Evolução do IMC");
            tabIMC.BackColor = Color.White;
            tabControl.TabPages.Add(tabIMC);

            // Aba de Cintura
            var tabCintura = new TabPage("📏 Evolução da Cintura");
            tabCintura.BackColor = Color.White;
            tabControl.TabPages.Add(tabCintura);

            // Botão fechar
            var btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(this.Width - 120, 20),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.Click += (s, e) => this.Close();

            Controls.Add(lblTitulo);
            Controls.Add(lblFiltro);
            Controls.Add(cbPeriodoGrafico);
            Controls.Add(tabControl);
            Controls.Add(btnFechar);

            // Carregar dados iniciais
            AtualizarGraficos();
        }

        private void AtualizarGraficos()
        {
            var registrosFiltrados = AplicarFiltros(perfil.Registros);

            if (!registrosFiltrados.Any())
            {
                MostrarMensagemSemDados();
                return;
            }

            AtualizarAbaPeso(registrosFiltrados);
            AtualizarAbaIMC(registrosFiltrados);
            AtualizarAbaCintura(registrosFiltrados);
        }

        private List<Registro> AplicarFiltros(List<Registro> registros)
        {
            var hoje = DateTime.Today;
            var filtrados = registros.OrderBy(r => r.Data).ToList();

            switch (cbPeriodoGrafico.SelectedIndex)
            {
                case 1: // Últimos 30 dias
                    return filtrados.Where(r => r.Data >= hoje.AddDays(-30)).ToList();
                case 2: // Últimos 90 dias
                    return filtrados.Where(r => r.Data >= hoje.AddDays(-90)).ToList();
                case 3: // Este ano
                    return filtrados.Where(r => r.Data.Year == hoje.Year).ToList();
                case 4: // Último ano
                    return filtrados.Where(r => r.Data.Year == hoje.Year - 1).ToList();
            }

            return filtrados; // Todos os registros
        }

        private void AtualizarAbaPeso(List<Registro> registros)
        {
            var tabPeso = tabControl.TabPages[0];
            tabPeso.Controls.Clear();

            var dados = registros.Select(r => (r.Data, r.Peso)).ToList();
            CriarVisualizacaoSimples(tabPeso, dados, "Peso (kg)", Color.FromArgb(70, 130, 180));
        }

        private void AtualizarAbaIMC(List<Registro> registros)
        {
            var tabIMC = tabControl.TabPages[1];
            tabIMC.Controls.Clear();

            var dados = registros.Select(r => (r.Data, perfil.CalcularIMC(r.Peso))).ToList();
            CriarVisualizacaoSimples(tabIMC, dados, "IMC", Color.FromArgb(220, 120, 60));
        }

        private void AtualizarAbaCintura(List<Registro> registros)
        {
            var tabCintura = tabControl.TabPages[2];
            tabCintura.Controls.Clear();

            var dados = registros
                .Where(r => r.Cintura.HasValue)
                .Select(r => (r.Data, r.Cintura.Value))
                .ToList();

            if (dados.Any())
            {
                CriarVisualizacaoSimples(tabCintura, dados, "Cintura (cm)", Color.FromArgb(90, 180, 90));
            }
            else
            {
                var lblSemDados = new Label
                {
                    Text = "📊 Nenhum dado de cintura disponível\n\nAdicione medidas de cintura nos registros para ver o gráfico.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabCintura.Controls.Add(lblSemDados);
            }
        }

        private void CriarVisualizacaoSimples(TabPage tab, List<(DateTime data, double valor)> dados, string titulo, Color cor)
        {
            // Painel principal
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Título
            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = cor,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // DataGridView para mostrar os dados em formato de tabela
            var dgv = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(tab.Width - 60, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            dgv.Columns.Add("Data", "📅 Data");
            dgv.Columns.Add("Valor", $"📊 {titulo}");

            foreach (var (data, valor) in dados)
            {
                dgv.Rows.Add(data.ToString("dd/MM/yyyy"), valor.ToString("F2"));
            }

            // Estatísticas
            var estatisticas = CalcularEstatisticas(dados);
            var lblEstatisticas = new Label
            {
                Text = $"📈 Estatísticas:\n" +
                      $"• Média: {estatisticas.media:F2}\n" +
                      $"• Mínimo: {estatisticas.minimo:F2}\n" +
                      $"• Máximo: {estatisticas.maximo:F2}\n" +
                      $"• Variação: {estatisticas.variacao:+#.##;-#.##;0}%",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(20, 280),
                AutoSize = true
            };

            // Representação visual simples com texto
            var lblVisualizacao = new Label
            {
                Text = CriarRepresentacaoVisual(dados, titulo),
                Font = new Font("Consolas", 9),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(20, 380),
                Size = new Size(tab.Width - 60, 200),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            panel.Controls.AddRange(new Control[] { lblTitulo, dgv, lblEstatisticas, lblVisualizacao });
            tab.Controls.Add(panel);
        }

        private (double media, double minimo, double maximo, double variacao) CalcularEstatisticas(List<(DateTime data, double valor)> dados)
        {
            if (!dados.Any()) return (0, 0, 0, 0);

            var valores = dados.Select(d => d.valor).ToList();
            var media = valores.Average();
            var minimo = valores.Min();
            var maximo = valores.Max();
            var variacao = ((valores.Last() - valores.First()) / valores.First()) * 100;

            return (media, minimo, maximo, variacao);
        }

        private string CriarRepresentacaoVisual(List<(DateTime data, double valor)> dados, string titulo)
        {
            if (!dados.Any()) return "Nenhum dado disponível";

            var result = $"{titulo} - Evolução Temporal:\n\n";
            
            // Encontrar escala
            var minValor = dados.Min(d => d.valor);
            var maxValor = dados.Max(d => d.valor);
            var range = maxValor - minValor;

            foreach (var (data, valor) in dados.Take(20)) // Limitar a 20 pontos para não ficar muito grande
            {
                var normalized = range > 0 ? (valor - minValor) / range : 0.5;
                var barLength = (int)(normalized * 50);
                var bar = new string('█', barLength);
                
                result += $"{data:dd/MM}: {valor,6:F1} {bar}\n";
            }

            if (dados.Count > 20)
            {
                result += $"\n... e mais {dados.Count - 20} registros";
            }

            return result;
        }

        private void MostrarMensagemSemDados()
        {
            foreach (TabPage tab in tabControl.TabPages)
            {
                tab.Controls.Clear();
                
                var lblMensagem = new Label
                {
                    Text = "📊 Nenhum dado disponível para o período selecionado\n\nAltere o filtro ou adicione mais registros.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tab.Controls.Add(lblMensagem);
            }
        }
    }
}