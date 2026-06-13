namespace Story.Player.WinForms
{
    partial class PlayerForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Menu ──────────────────────────────────────────────────────────────
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuRestart;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuSaveState;
        private System.Windows.Forms.ToolStripMenuItem menuLoadState;
        private System.Windows.Forms.ToolStripSeparator menuSep2;
        private System.Windows.Forms.ToolStripMenuItem menuExit;

        // ── HUD Panel ─────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel panelHUD;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBlockId;

        private System.Windows.Forms.Label lblHealth;
        private System.Windows.Forms.ProgressBar pbHealth;
        private System.Windows.Forms.Label lblStamina;
        private System.Windows.Forms.ProgressBar pbStamina;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.ProgressBar pbLevel;
        private System.Windows.Forms.Panel panelExtraHud;

        // ── Content ───────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.RichTextBox rtbNarrative;
        private System.Windows.Forms.FlowLayoutPanel flowDecisions;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Form ─────────────────────────────────────────────────────────
            this.Text = "Story Engine – Player";
            this.Size = new System.Drawing.Size(1100, 800);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.BackColor = System.Drawing.Color.FromArgb(18, 15, 25);
            this.ForeColor = System.Drawing.Color.FromArgb(230, 220, 245);
            this.Font = new System.Drawing.Font("Segoe UI", 10f);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Resize += new System.EventHandler(this.PlayerForm_Resize);

            // ── MenuStrip ────────────────────────────────────────────────────
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem("📂 File");
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem("Open Story...");
            this.menuRestart = new System.Windows.Forms.ToolStripMenuItem("Restart");
            this.menuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveState = new System.Windows.Forms.ToolStripMenuItem("Save State...");
            this.menuLoadState = new System.Windows.Forms.ToolStripMenuItem("Load State...");
            this.menuSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem("Exit");

            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            this.menuRestart.Click += new System.EventHandler(this.menuRestart_Click);
            this.menuSaveState.Click += new System.EventHandler(this.menuSaveState_Click);
            this.menuLoadState.Click += new System.EventHandler(this.menuLoadState_Click);
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);

            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuOpen, this.menuRestart, this.menuSep1,
                this.menuSaveState, this.menuLoadState, this.menuSep2,
                this.menuExit
            });
            this.menuStrip.Items.Add(this.menuFile);
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(22, 18, 32);
            this.menuStrip.ForeColor = System.Drawing.Color.FromArgb(230, 220, 245);

            // ── HUD Panel ────────────────────────────────────────────────────
            this.panelHUD = new System.Windows.Forms.Panel();
            this.panelHUD.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHUD.Height = 110;
            this.panelHUD.BackColor = System.Drawing.Color.FromArgb(28, 23, 38);
            this.panelHUD.Padding = new System.Windows.Forms.Padding(14, 8, 14, 8);

            // Title
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTitle.Text = "Story Engine";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15f, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 120, 255);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(14, 10);

            // Block ID
            this.lblBlockId = new System.Windows.Forms.Label();
            this.lblBlockId.Text = "";
            this.lblBlockId.Font = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Italic);
            this.lblBlockId.ForeColor = System.Drawing.Color.FromArgb(130, 120, 155);
            this.lblBlockId.AutoSize = true;
            this.lblBlockId.Location = new System.Drawing.Point(14, 42);

            // ── Health bar ───────────────────────────────────────────────────
            int col1 = 270, col2 = 480, col3 = 690, barW = 185, barY1 = 12, barY2 = 34;

            this.lblHealth = new System.Windows.Forms.Label();
            this.lblHealth.Text = "❤ Health";
            this.lblHealth.Font = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);
            this.lblHealth.ForeColor = System.Drawing.Color.FromArgb(220, 210, 240);
            this.lblHealth.AutoSize = true;
            this.lblHealth.Location = new System.Drawing.Point(col1, barY1);

            this.pbHealth = new System.Windows.Forms.ProgressBar();
            this.pbHealth.Location = new System.Drawing.Point(col1, barY2);
            this.pbHealth.Size = new System.Drawing.Size(barW, 22);
            this.pbHealth.Minimum = 0;
            this.pbHealth.Maximum = 100;
            this.pbHealth.Value = 100;
            this.pbHealth.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbHealth.ForeColor = System.Drawing.Color.FromArgb(80, 200, 120);

            // ── Stamina bar ──────────────────────────────────────────────────
            this.lblStamina = new System.Windows.Forms.Label();
            this.lblStamina.Text = "⚡ Stamina";
            this.lblStamina.Font = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);
            this.lblStamina.ForeColor = System.Drawing.Color.FromArgb(220, 210, 240);
            this.lblStamina.AutoSize = true;
            this.lblStamina.Location = new System.Drawing.Point(col2, barY1);

            this.pbStamina = new System.Windows.Forms.ProgressBar();
            this.pbStamina.Location = new System.Drawing.Point(col2, barY2);
            this.pbStamina.Size = new System.Drawing.Size(barW, 22);
            this.pbStamina.Minimum = 0;
            this.pbStamina.Maximum = 100;
            this.pbStamina.Value = 80;
            this.pbStamina.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbStamina.ForeColor = System.Drawing.Color.FromArgb(80, 160, 230);

            // ── Level bar ────────────────────────────────────────────────────
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblLevel.Text = "⭐ Level";
            this.lblLevel.Font = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);
            this.lblLevel.ForeColor = System.Drawing.Color.FromArgb(220, 210, 240);
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(col3, barY1);

            this.pbLevel = new System.Windows.Forms.ProgressBar();
            this.pbLevel.Location = new System.Drawing.Point(col3, barY2);
            this.pbLevel.Size = new System.Drawing.Size(barW, 22);
            this.pbLevel.Minimum = 0;
            this.pbLevel.Maximum = 100;
            this.pbLevel.Value = 10;
            this.pbLevel.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbLevel.ForeColor = System.Drawing.Color.FromArgb(230, 180, 50);

            // ── Extra HUD panel (other visible props) ─────────────────────────
            this.panelExtraHud = new System.Windows.Forms.Panel();
            this.panelExtraHud.Location = new System.Drawing.Point(col3 + barW + 16, 8);
            this.panelExtraHud.Size = new System.Drawing.Size(160, 90);
            this.panelExtraHud.BackColor = System.Drawing.Color.Transparent;

            this.panelHUD.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTitle, this.lblBlockId,
                this.lblHealth, this.pbHealth,
                this.lblStamina, this.pbStamina,
                this.lblLevel, this.pbLevel,
                this.panelExtraHud
            });

            // ── Content Panel ────────────────────────────────────────────────
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(18, 15, 25);
            this.panelContent.Padding = new System.Windows.Forms.Padding(0);

            // Background image
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.picBackground.Dock = System.Windows.Forms.DockStyle.Top;
            this.picBackground.Height = 260;
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBackground.BackColor = System.Drawing.Color.FromArgb(22, 18, 32);
            this.picBackground.Visible = false;

            // Narrative text
            this.rtbNarrative = new System.Windows.Forms.RichTextBox();
            this.rtbNarrative.Dock = System.Windows.Forms.DockStyle.Top;
            this.rtbNarrative.Height = 160;
            this.rtbNarrative.BackColor = System.Drawing.Color.FromArgb(28, 23, 38);
            this.rtbNarrative.ForeColor = System.Drawing.Color.FromArgb(230, 220, 245);
            this.rtbNarrative.Font = new System.Drawing.Font("Georgia", 12f);
            this.rtbNarrative.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbNarrative.ReadOnly = true;
            this.rtbNarrative.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbNarrative.Padding = new System.Windows.Forms.Padding(14, 12, 14, 12);

            // Decisions flow panel
            this.flowDecisions = new System.Windows.Forms.FlowLayoutPanel();
            this.flowDecisions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowDecisions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowDecisions.AutoScroll = true;
            this.flowDecisions.BackColor = System.Drawing.Color.FromArgb(18, 15, 25);
            this.flowDecisions.WrapContents = false;
            this.flowDecisions.Padding = new System.Windows.Forms.Padding(14, 10, 14, 10);

            this.panelContent.Controls.Add(this.flowDecisions);
            this.panelContent.Controls.Add(this.rtbNarrative);
            this.panelContent.Controls.Add(this.picBackground);

            // ── Add to Form ──────────────────────────────────────────────────
            this.MainMenuStrip = this.menuStrip;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHUD);
            this.Controls.Add(this.menuStrip);
        }
    }
}
