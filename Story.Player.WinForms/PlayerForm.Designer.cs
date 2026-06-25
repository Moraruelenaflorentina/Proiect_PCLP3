using System.Drawing;
using System.Windows.Forms;
using Story.Player.WinForms.UI;

namespace Story.Player.WinForms
{
    partial class PlayerForm
    {
        private System.ComponentModel.IContainer components = null;

        private MenuStrip menuStrip = null!;
        private ToolStripMenuItem menuFile = null!;
        private ToolStripMenuItem menuOpen = null!;
        private ToolStripMenuItem menuRestart = null!;
        private ToolStripMenuItem menuSaveState = null!;
        private ToolStripMenuItem menuLoadState = null!;
        private ToolStripMenuItem menuCreateStory = null!;
        private ToolStripMenuItem menuExit = null!;

        private SceneCanvas canvas = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Text          = "StoryTeller";
            BackColor     = Palette.Background;
            ForeColor     = Palette.Text;
            Font          = new Font("Segoe UI", 10f);
            ClientSize    = new Size(1280, 800);
            MinimumSize   = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(12, 6, 18),
                ForeColor = Palette.Text,
                Renderer  = new ToolStripProfessionalRenderer(new DarkColors()),
                Padding   = new Padding(4, 2, 4, 2)
            };

            menuFile        = new ToolStripMenuItem("Poveste") { ForeColor = Palette.Text };
            menuOpen        = MakeMenuItem("Deschide ZIP...",  Keys.Control | Keys.O);
            menuRestart     = MakeMenuItem("Reia povestea",    Keys.Control | Keys.Shift | Keys.R);
            menuSaveState   = MakeMenuItem("Salvează stare...", Keys.Control | Keys.S);
            menuLoadState   = MakeMenuItem("Încarcă stare...",  Keys.Control | Keys.L);
            menuCreateStory = MakeMenuItem("Editor poveste...", Keys.Control | Keys.N);
            menuExit        = MakeMenuItem("Ieșire",           Keys.Control | Keys.Q);

            menuFile.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuOpen, menuRestart,
                new ToolStripSeparator(),
                menuSaveState, menuLoadState,
                new ToolStripSeparator(),
                menuCreateStory,
                new ToolStripSeparator(),
                menuExit
            });
            menuStrip.Items.Add(menuFile);

            menuOpen.Click        += menuOpen_Click;
            menuRestart.Click     += menuRestart_Click;
            menuSaveState.Click   += menuSaveState_Click;
            menuLoadState.Click   += menuLoadState_Click;
            menuCreateStory.Click += createStoryToolStripMenuItem_Click;
            menuExit.Click        += menuExit_Click;

            canvas = new SceneCanvas
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(canvas);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;

            ResumeLayout(false);
            PerformLayout();
        }

        private ToolStripMenuItem MakeMenuItem(string text, Keys shortcut)
        {
            return new ToolStripMenuItem(text)
            {
                ShortcutKeys = shortcut,
                BackColor    = Color.FromArgb(12, 6, 18),
                ForeColor    = Palette.Text
            };
        }

        private class DarkColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected              => Palette.SurfaceHover;
            public override Color MenuItemSelectedGradientBegin => Palette.SurfaceHover;
            public override Color MenuItemSelectedGradientEnd   => Palette.SurfaceHover;
            public override Color MenuItemBorder                => Palette.Primary;
            public override Color MenuItemPressedGradientBegin  => Palette.Surface;
            public override Color MenuItemPressedGradientEnd    => Palette.Surface;
            public override Color ToolStripDropDownBackground   => Color.FromArgb(20, 12, 30);
            public override Color ImageMarginGradientBegin      => Color.FromArgb(20, 12, 30);
            public override Color ImageMarginGradientMiddle     => Color.FromArgb(20, 12, 30);
            public override Color ImageMarginGradientEnd        => Color.FromArgb(20, 12, 30);
            public override Color SeparatorDark                 => Palette.SurfaceHigh;
            public override Color SeparatorLight                => Palette.SurfaceHigh;
            public override Color MenuBorder                    => Palette.Primary;
            public override Color ToolStripBorder               => Color.FromArgb(12, 6, 18);
        }
    }
}
