namespace Story.Editor.WinForms
{
    partial class EditorForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Menu ──────────────────────────────────────────────────────────────
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripSeparator menuSep2;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemTools;
        private System.Windows.Forms.ToolStripMenuItem menuValidate;

        // ── Layout ────────────────────────────────────────────────────────────
        private System.Windows.Forms.SplitContainer splitMain;       // left | right
        private System.Windows.Forms.SplitContainer splitRight;      // edit (top) | preview+log (bottom)
        private System.Windows.Forms.SplitContainer splitBottom;     // preview | log

        // ── Left pane ─────────────────────────────────────────────────────────
        private System.Windows.Forms.TreeView treeStory;

        // ── Center pane (edit area) ───────────────────────────────────────────
        private System.Windows.Forms.Panel panelEdit;

        // ── Right-bottom: preview + log ───────────────────────────────────────
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.RichTextBox rtbLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Form ─────────────────────────────────────────────────────────
            this.Text = "Story Engine – Editor";
            this.Size = new System.Drawing.Size(1200, 840);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.BackColor = System.Drawing.Color.FromArgb(20, 16, 28);
            this.ForeColor = System.Drawing.Color.FromArgb(230, 220, 245);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5f);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // ── MenuStrip ────────────────────────────────────────────────────
            this.menuStrip     = new System.Windows.Forms.MenuStrip();
            this.menuItemFile  = new System.Windows.Forms.ToolStripMenuItem("📂 File");
            this.menuNew       = new System.Windows.Forms.ToolStripMenuItem("New Story");
            this.menuOpen      = new System.Windows.Forms.ToolStripMenuItem("Open Story...");
            this.menuSep1      = new System.Windows.Forms.ToolStripSeparator();
            this.menuSave      = new System.Windows.Forms.ToolStripMenuItem("Save Story...");
            this.menuSep2      = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit      = new System.Windows.Forms.ToolStripMenuItem("Exit");
            this.menuItemTools = new System.Windows.Forms.ToolStripMenuItem("🔧 Tools");
            this.menuValidate  = new System.Windows.Forms.ToolStripMenuItem("Validate Story");

            this.menuNew.Click      += new System.EventHandler(this.menuNew_Click);
            this.menuOpen.Click     += new System.EventHandler(this.menuOpen_Click);
            this.menuSave.Click     += new System.EventHandler(this.menuSave_Click);
            this.menuExit.Click     += new System.EventHandler(this.menuExit_Click);
            this.menuValidate.Click += new System.EventHandler(this.menuValidate_Click);

            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuNew, this.menuOpen, this.menuSep1,
                this.menuSave, this.menuSep2, this.menuExit
            });
            this.menuItemTools.DropDownItems.Add(this.menuValidate);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuItemFile, this.menuItemTools
            });
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(22, 18, 32);
            this.menuStrip.ForeColor = System.Drawing.Color.FromArgb(230, 220, 245);

            // ── TreeView (left pane) ──────────────────────────────────────────
            this.treeStory = new System.Windows.Forms.TreeView();
            this.treeStory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeStory.BackColor = System.Drawing.Color.FromArgb(26, 21, 38);
            this.treeStory.ForeColor = System.Drawing.Color.FromArgb(220, 210, 240);
            this.treeStory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeStory.Font = new System.Drawing.Font("Segoe UI", 10f);
            this.treeStory.ItemHeight = 26;
            this.treeStory.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeStory_AfterSelect);

            // ── Edit panel (center) ───────────────────────────────────────────
            this.panelEdit = new System.Windows.Forms.Panel();
            this.panelEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEdit.BackColor = System.Drawing.Color.FromArgb(20, 16, 28);
            this.panelEdit.AutoScroll = true;

            // ── Preview image ─────────────────────────────────────────────────
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.BackColor = System.Drawing.Color.FromArgb(26, 21, 38);

            // ── Validation log ────────────────────────────────────────────────
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(14, 10, 20);
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(140, 255, 120);
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9f);
            this.rtbLog.ReadOnly = true;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Text = "Ready.";

            // ── SplitContainer: bottom-left (preview) | bottom-right (log) ───
            this.splitBottom = new System.Windows.Forms.SplitContainer();
            this.splitBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBottom.SplitterDistance = 220;
            this.splitBottom.BackColor = System.Drawing.Color.FromArgb(20, 16, 28);
            this.splitBottom.Panel1.Controls.Add(this.picPreview);
            this.splitBottom.Panel2.Controls.Add(this.rtbLog);

            // ── SplitContainer: edit (top) | bottom strip ─────────────────────
            this.splitRight = new System.Windows.Forms.SplitContainer();
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitRight.SplitterDistance = 520;
            this.splitRight.BackColor = System.Drawing.Color.FromArgb(20, 16, 28);
            this.splitRight.Panel1.Controls.Add(this.panelEdit);
            this.splitRight.Panel2.Controls.Add(this.splitBottom);

            // ── SplitContainer: tree (left) | edit+preview (right) ───────────
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.SplitterDistance = 240;
            this.splitMain.BackColor = System.Drawing.Color.FromArgb(20, 16, 28);
            this.splitMain.Panel1.Controls.Add(this.treeStory);
            this.splitMain.Panel1.BackColor = System.Drawing.Color.FromArgb(26, 21, 38);
            this.splitMain.Panel2.Controls.Add(this.splitRight);

            // ── Add to Form ──────────────────────────────────────────────────
            this.MainMenuStrip = this.menuStrip;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.menuStrip);
        }
    }
}
