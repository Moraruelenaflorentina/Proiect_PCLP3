namespace Story.Editor.WinForms
{
    partial class EditorForm
    {
        private System.ComponentModel.IContainer components = null;

        //Menu 
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

        //Layout 
        private System.Windows.Forms.SplitContainer splitMain;       
        private System.Windows.Forms.SplitContainer splitRight;      
        private System.Windows.Forms.SplitContainer splitBottom;     

        //Left pane 
        private System.Windows.Forms.TreeView treeStory;

        //Center pane (edit area) 
        private System.Windows.Forms.Panel panelEdit;

        //preview + log
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.RichTextBox rtbLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            menuItemFile = new ToolStripMenuItem();
            menuNew = new ToolStripMenuItem();
            menuOpen = new ToolStripMenuItem();
            menuSep1 = new ToolStripSeparator();
            menuSave = new ToolStripMenuItem();
            menuSep2 = new ToolStripSeparator();
            menuExit = new ToolStripMenuItem();
            menuItemTools = new ToolStripMenuItem();
            menuValidate = new ToolStripMenuItem();
            treeStory = new TreeView();
            panelEdit = new Panel();
            picPreview = new PictureBox();
            rtbLog = new RichTextBox();
            splitBottom = new SplitContainer();
            splitRight = new SplitContainer();
            splitMain = new SplitContainer();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitBottom).BeginInit();
            splitBottom.Panel1.SuspendLayout();
            splitBottom.Panel2.SuspendLayout();
            splitBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            splitRight.Panel1.SuspendLayout();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.BackColor = Color.FromArgb(22, 18, 32);
            menuStrip.ForeColor = Color.FromArgb(230, 220, 245);
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { menuItemFile, menuItemTools });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1182, 28);
            menuStrip.TabIndex = 1;
            // 
            // menuItemFile
            // 
            menuItemFile.DropDownItems.AddRange(new ToolStripItem[] { menuNew, menuOpen, menuSep1, menuSave, menuSep2, menuExit });
            menuItemFile.Name = "menuItemFile";
            menuItemFile.Size = new Size(71, 24);
            menuItemFile.Text = "📂 File";
            // 
            // menuNew
            // 
            menuNew.Name = "menuNew";
            menuNew.Size = new Size(224, 26);
            menuNew.Text = "New Story";
            menuNew.Click += menuNew_Click;
            // 
            // menuOpen
            // 
            menuOpen.Name = "menuOpen";
            menuOpen.Size = new Size(224, 26);
            menuOpen.Text = "Open Story...";
            menuOpen.Click += menuOpen_Click;
            // 
            // menuSep1
            // 
            menuSep1.Name = "menuSep1";
            menuSep1.Size = new Size(221, 6);
            // 
            // menuSave
            // 
            menuSave.Name = "menuSave";
            menuSave.Size = new Size(224, 26);
            menuSave.Text = "Save Story...";
            menuSave.Click += menuSave_Click;
            // 
            // menuSep2
            // 
            menuSep2.Name = "menuSep2";
            menuSep2.Size = new Size(221, 6);
            // 
            // menuExit
            // 
            menuExit.Name = "menuExit";
            menuExit.Size = new Size(224, 26);
            menuExit.Text = "Exit";
            menuExit.Click += menuExit_Click;
            // 
            // menuItemTools
            // 
            menuItemTools.DropDownItems.AddRange(new ToolStripItem[] { menuValidate });
            menuItemTools.Name = "menuItemTools";
            menuItemTools.Size = new Size(83, 24);
            menuItemTools.Text = "🔧 Tools";
            menuItemTools.Click += menuItemTools_Click;
            // 
            // menuValidate
            // 
            menuValidate.Name = "menuValidate";
            menuValidate.Size = new Size(224, 26);
            menuValidate.Text = "Validate Story";
            menuValidate.Click += menuValidate_Click;
            // 
            // treeStory
            // 
            treeStory.BackColor = Color.FromArgb(26, 21, 38);
            treeStory.BorderStyle = BorderStyle.None;
            treeStory.Dock = DockStyle.Fill;
            treeStory.Font = new Font("Segoe UI", 10F);
            treeStory.ForeColor = Color.FromArgb(220, 210, 240);
            treeStory.ItemHeight = 26;
            treeStory.Location = new Point(0, 0);
            treeStory.Name = "treeStory";
            treeStory.Size = new Size(953, 765);
            treeStory.TabIndex = 0;
            treeStory.AfterSelect += treeStory_AfterSelect;
            // 
            // panelEdit
            // 
            panelEdit.AutoScroll = true;
            panelEdit.BackColor = Color.FromArgb(20, 16, 28);
            panelEdit.Dock = DockStyle.Fill;
            panelEdit.Location = new Point(0, 0);
            panelEdit.Name = "panelEdit";
            panelEdit.Size = new Size(225, 543);
            panelEdit.TabIndex = 0;
            // 
            // picPreview
            // 
            picPreview.BackColor = Color.FromArgb(26, 21, 38);
            picPreview.Dock = DockStyle.Fill;
            picPreview.Location = new Point(0, 0);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(181, 218);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picPreview.TabIndex = 0;
            picPreview.TabStop = false;
            // 
            // rtbLog
            // 
            rtbLog.BackColor = Color.FromArgb(14, 10, 20);
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.Dock = DockStyle.Fill;
            rtbLog.Font = new Font("Consolas", 9F);
            rtbLog.ForeColor = Color.FromArgb(140, 255, 120);
            rtbLog.Location = new Point(0, 0);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(40, 218);
            rtbLog.TabIndex = 0;
            rtbLog.Text = "Ready.";
            // 
            // splitBottom
            // 
            splitBottom.BackColor = Color.FromArgb(20, 16, 28);
            splitBottom.Dock = DockStyle.Fill;
            splitBottom.Location = new Point(0, 0);
            splitBottom.Name = "splitBottom";
            // 
            // splitBottom.Panel1
            // 
            splitBottom.Panel1.Controls.Add(picPreview);
            // 
            // splitBottom.Panel2
            // 
            splitBottom.Panel2.Controls.Add(rtbLog);
            splitBottom.Size = new Size(225, 218);
            splitBottom.SplitterDistance = 181;
            splitBottom.TabIndex = 0;
            // 
            // splitRight
            // 
            splitRight.BackColor = Color.FromArgb(20, 16, 28);
            splitRight.Dock = DockStyle.Fill;
            splitRight.Location = new Point(0, 0);
            splitRight.Name = "splitRight";
            splitRight.Orientation = Orientation.Horizontal;
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(panelEdit);
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(splitBottom);
            splitRight.Size = new Size(225, 765);
            splitRight.SplitterDistance = 543;
            splitRight.TabIndex = 0;
            // 
            // splitMain
            // 
            splitMain.BackColor = Color.FromArgb(20, 16, 28);
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 28);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.BackColor = Color.FromArgb(26, 21, 38);
            splitMain.Panel1.Controls.Add(treeStory);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitRight);
            splitMain.Size = new Size(1182, 765);
            splitMain.SplitterDistance = 953;
            splitMain.TabIndex = 0;
            // 
            // EditorForm
            // 
            BackColor = Color.FromArgb(20, 16, 28);
            ClientSize = new Size(1182, 793);
            Controls.Add(splitMain);
            Controls.Add(menuStrip);
            Font = new Font("Segoe UI", 9.5F);
            ForeColor = Color.FromArgb(230, 220, 245);
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(900, 600);
            Name = "EditorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Story Engine – Editor";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            splitBottom.Panel1.ResumeLayout(false);
            splitBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitBottom).EndInit();
            splitBottom.ResumeLayout(false);
            splitRight.Panel1.ResumeLayout(false);
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
