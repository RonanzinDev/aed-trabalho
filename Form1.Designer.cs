namespace notepad
{
    public partial class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;
        private RichTextBox richTextBox;
        private TabelaHash tabela;
        private MenuStrip menuStrip;
        private ToolStripMenuItem arquivoMenuItem;
        private ToolStripMenuItem abrirMenuItem;
        private ToolStripMenuItem salvarMenuItem;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        // Função para popular a Tabela hash
        protected void popularTabela()
        {
            string[] palavras = File.ReadAllLines("Palavras.txt");
            tabela = new TabelaHash(1_000_000);
            foreach (string palavra in palavras)
            {
                tabela.Inserir(palavra, palavra);
            }
        }

        private void InitializeComponent()
        {
            // Inicializando componente de texto
            this.richTextBox = new RichTextBox();
            // Inicializando componemte de Menu
            this.menuStrip = new MenuStrip();
            // Inicializando os botoes para salvar e abrir arquivo
            this.arquivoMenuItem = new ToolStripMenuItem();
            this.abrirMenuItem = new ToolStripMenuItem();
            this.salvarMenuItem = new ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // richTextBox
            this.richTextBox.Dock = DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 27); // Ajusta a posição para não cobrir o MenuStrip
            this.richTextBox.Margin = new Padding(6,6,6,6);
            this.richTextBox.Name = "richTextBox";
            // Tamanho do componente do texto
            this.richTextBox.Size = new System.Drawing.Size(1370, 722);
            // Inicializando do inicio do texto 
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            this.richTextBox.TextChanged += new EventHandler(this.ManipularText);
            this.richTextBox.MouseClick += new MouseEventHandler(this.ClicarPalavra);
            // menuStrip
            // Colocando o menu no topo
            menuStrip.Dock = DockStyle.Top;
            this.menuStrip.Items.AddRange(new ToolStripItem[] {
                this.arquivoMenuItem
            });
            menuStrip.Location = new Point(0,0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(11, 4, 0 ,4);
            menuStrip.Size = new System.Drawing.Size(1370, 27);
            menuStrip.TabIndex = 1;
            // Adicionando os botoes de salvar e abrir arquivo no menu
            this.arquivoMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.abrirMenuItem,
                this.salvarMenuItem
            });
            this.arquivoMenuItem.Text = "Arquivo";
            this.abrirMenuItem.Text = "Abrir";
            this.salvarMenuItem.Text = "Salvar";
            this.abrirMenuItem.Click += new EventHandler(this.AbrirArquivo);
            this.salvarMenuItem.Click += new EventHandler(this.SalvarArquivo);
            // Adicionando o atalho para salvar (CTRL + S)
            this.salvarMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Form1";
            this.Text = "Editor de Texto";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void AbrirArquivo(object sender, EventArgs e)
        {
            // Abrindo dialago de arquivo
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Filtrando quais tipos de arquivos que podem
                openFileDialog.Filter = "Text Files|*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Adiciando conteudo na caixa de texto
                    richTextBox.Text = File.ReadAllText(openFileDialog.FileName);
                }
            }
        }

        private void SalvarArquivo(object sender, EventArgs e)
        {
            // Abrindo dialogo de salvar arquivo
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {  
                    // Escrevendo no arquivo o conteudo da caixa de texto
                    File.WriteAllText(saveFileDialog.FileName, richTextBox.Text);
                }
            }
        }
        // Funcao que manipula do texto digitado
        public void ManipularText(object sender, EventArgs e)
        {
            // Armazena a posição atual do cursor
            int currentCursorPosition = richTextBox.SelectionStart;

            // Armazena a seleção de texto original
            int selectionLength = richTextBox.SelectionLength;

            // Remove todas as formatações de cor
            richTextBox.SelectAll();
            richTextBox.SelectionColor = Color.Black;
            richTextBox.DeselectAll();

            // Divide o texto em palavras
            var words = richTextBox.Text.Split(new[] { ' ', '\n', '\r', ',', '.', '!', '?', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);

            // Verifica cada palavra
            foreach (string word in words)
            {
                int index = richTextBox.Text.IndexOf(word, StringComparison.OrdinalIgnoreCase);
                while (index != -1)
                {
                    richTextBox.Select(index, word.Length); // Seleciona a palavra encontrada
                    // Buscando a palavra na Tabela hash e pintando ela de vermelha caso nao esteja, se estiver pinta de preta
                    if (tabela.Buscar(word.ToLower()) == "")
                    {
                        richTextBox.SelectionColor = Color.Red;
                    }
                    else
                    {
                        richTextBox.SelectionColor = Color.Black;
                    }
                    index = richTextBox.Text.IndexOf(word, index + word.Length, StringComparison.OrdinalIgnoreCase);
                }
            }

            // Restaura a posição e seleção do cursor
            richTextBox.SelectionStart = currentCursorPosition;
            richTextBox.SelectionLength = selectionLength;
            richTextBox.SelectionColor = Color.Black;
        }

        //Funcao para clicar na palavra e adicionar no dicionario e no arquivo de palavras
        // Se clicar na palavra em vermelho e dialago sera naquela palavra
        // se clicar em outro lugar, ele ira pegar a ultima palavra marcada em vermelha
        public void ClicarPalavra(object sender, MouseEventArgs e)
        {
            // Posicao do clique
            Point posicaoMouse = richTextBox.PointToClient(MousePosition);
            int charIndex  = richTextBox.GetCharIndexFromPosition(posicaoMouse);
            richTextBox.Select(charIndex, 1);
            // Obter a palavra
            string palavra = palavraNoCursor();
            if (!string.IsNullOrEmpty(palavra) && tabela.Buscar(palavra.ToLower()) == "")
            {
                // Dialago para adicionar palavra
                var resultado = MessageBox.Show($"A palavra '{palavra}' não está no dicionário. Deseja adicioná-la?", "Adicionar palavra", MessageBoxButtons.YesNo);
                if (resultado == DialogResult.Yes)
                {
                    tabela.Inserir(palavra.ToLower(), palavra);
                    File.AppendAllLines("Palavras.txt", new[] { palavra });
                    ManipularText(sender, e);
                }
            }
        }

        private string palavraNoCursor()
        {
            int index = richTextBox.SelectionStart;
            int inicio = index;
            int final = index;
            // enquanto o index de inicio for maior que 0 e nao for um espaco vazio ele diminui o index do inicio
            while (inicio > 0 && !char.IsWhiteSpace(richTextBox.Text[inicio - 1]))
            {
                inicio--;
            }
            // enquanto o index de final for menor que o tamanho da palavra e nao for um espaco vazio ele aumento o index de final
            while (final < richTextBox.Text.Length && !char.IsWhiteSpace(richTextBox.Text[final]))
            {
                final++;
            }
            // retorna a palavra
            return richTextBox.Text.Substring(inicio, final - inicio);
        }
    }
}