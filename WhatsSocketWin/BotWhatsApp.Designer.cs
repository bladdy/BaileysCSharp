namespace WhatsSocketWin
    {
    partial class BotWhatsApp
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            components = new System.ComponentModel.Container();
            contextMenuStrip1 = new ContextMenuStrip(components);
            conectToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            abrirToolStripMenuItem = new ToolStripMenuItem();
            conetarToolStripMenuItem = new ToolStripMenuItem();
            desconectarToolStripMenuItem = new ToolStripMenuItem();
            salirToolStripMenuItem = new ToolStripMenuItem();
            verToolStripMenuItem = new ToolStripMenuItem();
            verToolStripMenuItem1 = new ToolStripMenuItem();
            ordenesToolStripMenuItem = new ToolStripMenuItem();
            configurarToolStripMenuItem = new ToolStripMenuItem();
            menuToolStripMenuItem = new ToolStripMenuItem();
            ofertasToolStripMenuItem = new ToolStripMenuItem();
            notificacionesToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            DgvMensage = new DataGridView();
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvMensage).BeginInit();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { conectToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(113, 26);
            // 
            // conectToolStripMenuItem
            // 
            conectToolStripMenuItem.Name = "conectToolStripMenuItem";
            conectToolStripMenuItem.Size = new Size(112, 22);
            conectToolStripMenuItem.Text = "Conect";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { abrirToolStripMenuItem, verToolStripMenuItem, verToolStripMenuItem1, configurarToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(851, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // abrirToolStripMenuItem
            // 
            abrirToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { conetarToolStripMenuItem, desconectarToolStripMenuItem, salirToolStripMenuItem });
            abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            abrirToolStripMenuItem.Size = new Size(45, 20);
            abrirToolStripMenuItem.Text = "Abrir";
            // 
            // conetarToolStripMenuItem
            // 
            conetarToolStripMenuItem.Name = "conetarToolStripMenuItem";
            conetarToolStripMenuItem.Size = new Size(139, 22);
            conetarToolStripMenuItem.Text = "Conectar";
            conetarToolStripMenuItem.Click += conetarToolStripMenuItem_Click;
            // 
            // desconectarToolStripMenuItem
            // 
            desconectarToolStripMenuItem.Name = "desconectarToolStripMenuItem";
            desconectarToolStripMenuItem.Size = new Size(139, 22);
            desconectarToolStripMenuItem.Text = "Desconectar";
            // 
            // salirToolStripMenuItem
            // 
            salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            salirToolStripMenuItem.Size = new Size(139, 22);
            salirToolStripMenuItem.Text = "Salir";
            // 
            // verToolStripMenuItem
            // 
            verToolStripMenuItem.Name = "verToolStripMenuItem";
            verToolStripMenuItem.Size = new Size(49, 20);
            verToolStripMenuItem.Text = "Editar";
            // 
            // verToolStripMenuItem1
            // 
            verToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { ordenesToolStripMenuItem });
            verToolStripMenuItem1.Name = "verToolStripMenuItem1";
            verToolStripMenuItem1.Size = new Size(35, 20);
            verToolStripMenuItem1.Text = "Ver";
            // 
            // ordenesToolStripMenuItem
            // 
            ordenesToolStripMenuItem.Name = "ordenesToolStripMenuItem";
            ordenesToolStripMenuItem.Size = new Size(118, 22);
            ordenesToolStripMenuItem.Text = "Ordenes";
            // 
            // configurarToolStripMenuItem
            // 
            configurarToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuToolStripMenuItem, ofertasToolStripMenuItem, notificacionesToolStripMenuItem });
            configurarToolStripMenuItem.Name = "configurarToolStripMenuItem";
            configurarToolStripMenuItem.Size = new Size(76, 20);
            configurarToolStripMenuItem.Text = "Configurar";
            // 
            // menuToolStripMenuItem
            // 
            menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            menuToolStripMenuItem.Size = new Size(150, 22);
            menuToolStripMenuItem.Text = "Menu";
            // 
            // ofertasToolStripMenuItem
            // 
            ofertasToolStripMenuItem.Name = "ofertasToolStripMenuItem";
            ofertasToolStripMenuItem.Size = new Size(150, 22);
            ofertasToolStripMenuItem.Text = "Ofertas";
            // 
            // notificacionesToolStripMenuItem
            // 
            notificacionesToolStripMenuItem.Name = "notificacionesToolStripMenuItem";
            notificacionesToolStripMenuItem.Size = new Size(150, 22);
            notificacionesToolStripMenuItem.Text = "Notificaciones";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 589);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(851, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            toolStripStatusLabel1.Click += toolStripStatusLabel1_Click;
            // 
            // DgvMensage
            // 
            DgvMensage.AllowUserToAddRows = false;
            DgvMensage.AllowUserToDeleteRows = false;
            DgvMensage.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            DgvMensage.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvMensage.Dock = DockStyle.Fill;
            DgvMensage.Location = new Point(0, 24);
            DgvMensage.Name = "DgvMensage";
            DgvMensage.ReadOnly = true;
            DgvMensage.Size = new Size(851, 565);
            DgvMensage.TabIndex = 3;
            // 
            // BotWhatsApp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(851, 611);
            Controls.Add(DgvMensage);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "BotWhatsApp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BotWhatsApp";
            Load += BotWhatsApp_Load;
            contextMenuStrip1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DgvMensage).EndInit();
            ResumeLayout(false);
            PerformLayout();
            }

        #endregion

        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem conectToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem abrirToolStripMenuItem;
        private ToolStripMenuItem conetarToolStripMenuItem;
        private ToolStripMenuItem desconectarToolStripMenuItem;
        private ToolStripMenuItem salirToolStripMenuItem;
        private ToolStripMenuItem verToolStripMenuItem;
        private ToolStripMenuItem verToolStripMenuItem1;
        private ToolStripMenuItem ordenesToolStripMenuItem;
        private ToolStripMenuItem configurarToolStripMenuItem;
        private ToolStripMenuItem menuToolStripMenuItem;
        private ToolStripMenuItem ofertasToolStripMenuItem;
        private ToolStripMenuItem notificacionesToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DataGridView DgvMensage;
        }
    }