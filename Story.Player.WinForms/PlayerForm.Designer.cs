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
        private System.Windows.Forms.ToolStripMenuItem menuSaveState;
        private System.Windows.Forms.ToolStripMenuItem menuLoadState;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerForm));
            menuStrip = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuRestart = new ToolStripMenuItem();
            menuOpen = new ToolStripMenuItem();
            menuSaveState = new ToolStripMenuItem();
            menuLoadState = new ToolStripMenuItem();
            menuExit = new ToolStripMenuItem();
            createStoryToolStripMenuItem = new ToolStripMenuItem();
            panelHUD = new Panel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            lblTitle = new Label();
            lblBlockId = new Label();
            lblHealth = new Label();
            pbHealth = new ProgressBar();
            lblStamina = new Label();
            pbStamina = new ProgressBar();
            lblLevel = new Label();
            pbLevel = new ProgressBar();
            panelExtraHud = new Panel();
            pictureBox3 = new PictureBox();
            panelContent = new Panel();
            flowDecisions = new FlowLayoutPanel();
            rtbNarrative = new RichTextBox();
            picBackground = new PictureBox();
            menuStrip.SuspendLayout();
            panelHUD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelExtraHud.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            panelContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBackground).BeginInit();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.BackColor = Color.FromArgb(22, 18, 32);
            menuStrip.ForeColor = Color.FromArgb(230, 220, 245);
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { menuFile });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1082, 28);
            menuStrip.TabIndex = 2;
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuRestart, menuOpen, menuSaveState, menuLoadState, menuExit, createStoryToolStripMenuItem });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(71, 24);
            menuFile.Text = "📂 File";
            // 
            // menuRestart
            // 
            menuRestart.BackColor = Color.FromArgb(22, 18, 32);
            menuRestart.ForeColor = SystemColors.ButtonFace;
            menuRestart.Name = "menuRestart";
            menuRestart.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
            menuRestart.Size = new Size(260, 26);
            menuRestart.Text = "Restart";
            menuRestart.Click += menuRestart_Click;
            // 
            // menuOpen
            // 
            menuOpen.BackColor = Color.FromArgb(22, 18, 32);
            menuOpen.ForeColor = SystemColors.ButtonFace;
            menuOpen.Name = "menuOpen";
            menuOpen.ShortcutKeys = Keys.Control | Keys.O;
            menuOpen.Size = new Size(260, 26);
            menuOpen.Text = "Open Story...";
            menuOpen.Click += menuOpen_Click;
            // 
            // menuSaveState
            // 
            menuSaveState.BackColor = Color.FromArgb(22, 18, 32);
            menuSaveState.ForeColor = SystemColors.ButtonFace;
            menuSaveState.Name = "menuSaveState";
            menuSaveState.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            menuSaveState.Size = new Size(260, 26);
            menuSaveState.Text = "Save State...";
            menuSaveState.Click += menuSaveState_Click;
            // 
            // menuLoadState
            // 
            menuLoadState.BackColor = Color.FromArgb(22, 18, 32);
            menuLoadState.ForeColor = SystemColors.ButtonFace;
            menuLoadState.Name = "menuLoadState";
            menuLoadState.ShortcutKeys = Keys.Control | Keys.L;
            menuLoadState.Size = new Size(260, 26);
            menuLoadState.Text = "Load State...";
            menuLoadState.Click += menuLoadState_Click;
            // 
            // menuExit
            // 
            menuExit.BackColor = Color.FromArgb(22, 18, 32);
            menuExit.ForeColor = SystemColors.ButtonFace;
            menuExit.Name = "menuExit";
            menuExit.ShortcutKeys = Keys.Control | Keys.E;
            menuExit.Size = new Size(260, 26);
            menuExit.Text = "Exit";
            menuExit.Click += menuExit_Click;
            // 
            // createStoryToolStripMenuItem
            // 
            createStoryToolStripMenuItem.BackColor = Color.FromArgb(22, 18, 32);
            createStoryToolStripMenuItem.ForeColor = SystemColors.ButtonFace;
            createStoryToolStripMenuItem.Name = "createStoryToolStripMenuItem";
            createStoryToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            createStoryToolStripMenuItem.Size = new Size(260, 26);
            createStoryToolStripMenuItem.Text = "Create Story";
            createStoryToolStripMenuItem.Click += createStoryToolStripMenuItem_Click;
            // 
            // panelHUD
            // 
            panelHUD.BackColor = Color.FromArgb(28, 23, 38);
            panelHUD.Controls.Add(pictureBox2);
            panelHUD.Controls.Add(pictureBox1);
            panelHUD.Controls.Add(lblTitle);
            panelHUD.Controls.Add(lblBlockId);
            panelHUD.Controls.Add(lblHealth);
            panelHUD.Controls.Add(pbHealth);
            panelHUD.Controls.Add(lblStamina);
            panelHUD.Controls.Add(pbStamina);
            panelHUD.Controls.Add(lblLevel);
            panelHUD.Controls.Add(pbLevel);
            panelHUD.Controls.Add(panelExtraHud);
            panelHUD.Dock = DockStyle.Top;
            panelHUD.Location = new Point(0, 28);
            panelHUD.Name = "panelHUD";
            panelHUD.Padding = new Padding(14, 8, 14, 8);
            panelHUD.Size = new Size(1082, 110);
            panelHUD.TabIndex = 1;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.icons8_flash_331;
            pictureBox2.Location = new Point(478, 2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(33, 30);
            pictureBox2.TabIndex = 10;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_health_332;
            pictureBox1.Location = new Point(270, 1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(33, 33);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(180, 120, 255);
            lblTitle.Location = new Point(14, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(163, 35);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Story Engine";
            // 
            // lblBlockId
            // 
            lblBlockId.AutoSize = true;
            lblBlockId.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            lblBlockId.ForeColor = Color.FromArgb(130, 120, 155);
            lblBlockId.Location = new Point(14, 42);
            lblBlockId.Name = "lblBlockId";
            lblBlockId.Size = new Size(0, 20);
            lblBlockId.TabIndex = 1;
            // 
            // lblHealth
            // 
            lblHealth.AutoSize = true;
            lblHealth.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblHealth.ForeColor = Color.FromArgb(220, 210, 240);
            lblHealth.Location = new Point(309, 10);
            lblHealth.Name = "lblHealth";
            lblHealth.Size = new Size(59, 20);
            lblHealth.TabIndex = 2;
            lblHealth.Text = " Health";
            // 
            // pbHealth
            // 
            pbHealth.ForeColor = Color.FromArgb(80, 200, 120);
            pbHealth.Location = new Point(270, 36);
            pbHealth.Name = "pbHealth";
            pbHealth.Size = new Size(185, 22);
            pbHealth.Style = ProgressBarStyle.Continuous;
            pbHealth.TabIndex = 3;
            pbHealth.Value = 100;
            // 
            // lblStamina
            // 
            lblStamina.AutoSize = true;
            lblStamina.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblStamina.ForeColor = Color.FromArgb(220, 210, 240);
            lblStamina.Location = new Point(509, 11);
            lblStamina.Name = "lblStamina";
            lblStamina.Size = new Size(70, 20);
            lblStamina.TabIndex = 4;
            lblStamina.Text = " Stamina";
            // 
            // pbStamina
            // 
            pbStamina.ForeColor = Color.FromArgb(80, 160, 230);
            pbStamina.Location = new Point(480, 36);
            pbStamina.Name = "pbStamina";
            pbStamina.Size = new Size(185, 22);
            pbStamina.Style = ProgressBarStyle.Continuous;
            pbStamina.TabIndex = 5;
            pbStamina.Value = 80;
            pbStamina.Click += pbStamina_Click;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblLevel.ForeColor = Color.FromArgb(220, 210, 240);
            lblLevel.Location = new Point(732, 11);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(49, 20);
            lblLevel.TabIndex = 6;
            lblLevel.Text = " Level";
            // 
            // pbLevel
            // 
            pbLevel.ForeColor = Color.FromArgb(230, 180, 50);
            pbLevel.Location = new Point(690, 36);
            pbLevel.Name = "pbLevel";
            pbLevel.Size = new Size(185, 22);
            pbLevel.Style = ProgressBarStyle.Continuous;
            pbLevel.TabIndex = 7;
            pbLevel.Value = 10;
            // 
            // panelExtraHud
            // 
            panelExtraHud.BackColor = Color.Transparent;
            panelExtraHud.Controls.Add(pictureBox3);
            panelExtraHud.Location = new Point(690, 8);
            panelExtraHud.Name = "panelExtraHud";
            panelExtraHud.Size = new Size(160, 90);
            panelExtraHud.TabIndex = 8;
            panelExtraHud.Paint += panelExtraHud_Paint;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.icons8_level_33;
            pictureBox3.Location = new Point(3, -8);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(38, 31);
            pictureBox3.TabIndex = 11;
            pictureBox3.TabStop = false;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.FromArgb(18, 15, 25);
            panelContent.Controls.Add(flowDecisions);
            panelContent.Controls.Add(rtbNarrative);
            panelContent.Controls.Add(picBackground);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 138);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1082, 615);
            panelContent.TabIndex = 0;
            // 
            // flowDecisions
            // 
            flowDecisions.AutoScroll = true;
            flowDecisions.BackColor = Color.FromArgb(18, 15, 25);
            flowDecisions.Dock = DockStyle.Fill;
            flowDecisions.FlowDirection = FlowDirection.TopDown;
            flowDecisions.Location = new Point(0, 554);
            flowDecisions.Name = "flowDecisions";
            flowDecisions.Padding = new Padding(14, 10, 14, 10);
            flowDecisions.Size = new Size(1082, 61);
            flowDecisions.TabIndex = 0;
            flowDecisions.WrapContents = false;
            // 
            // rtbNarrative
            // 
            rtbNarrative.BackColor = Color.FromArgb(28, 23, 38);
            rtbNarrative.BorderStyle = BorderStyle.None;
            rtbNarrative.Dock = DockStyle.Top;
            rtbNarrative.Font = new Font("Georgia", 12F);
            rtbNarrative.ForeColor = Color.FromArgb(230, 220, 245);
            rtbNarrative.Location = new Point(0, 377);
            rtbNarrative.Name = "rtbNarrative";
            rtbNarrative.ReadOnly = true;
            rtbNarrative.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbNarrative.Size = new Size(1082, 177);
            rtbNarrative.TabIndex = 1;
            rtbNarrative.Text = "";
            // 
            // picBackground
            // 
            picBackground.BackColor = Color.FromArgb(22, 18, 32);
            picBackground.Dock = DockStyle.Top;
            picBackground.Location = new Point(0, 0);
            picBackground.Name = "picBackground";
            picBackground.Size = new Size(1082, 377);
            picBackground.SizeMode = PictureBoxSizeMode.Zoom;
            picBackground.TabIndex = 2;
            picBackground.TabStop = false;
            picBackground.Visible = false;
            picBackground.Click += picBackground_Click;
            // 
            // PlayerForm
            // 
            BackColor = Color.FromArgb(18, 15, 25);
            ClientSize = new Size(1082, 753);
            Controls.Add(panelContent);
            Controls.Add(panelHUD);
            Controls.Add(menuStrip);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(230, 220, 245);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(800, 600);
            Name = "PlayerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Story Engine – Player";
            Resize += PlayerForm_Resize;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            panelHUD.ResumeLayout(false);
            panelHUD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelExtraHud.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            panelContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picBackground).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private ToolStripMenuItem createStoryToolStripMenuItem;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
    }
}
