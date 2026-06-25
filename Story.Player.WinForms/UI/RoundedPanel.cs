using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Story.Player.WinForms.UI
{
    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 18;
        public Color BorderColor { get; set; } = Color.Empty;
        public int BorderThickness { get; set; } = 0;
        public bool DrawShadow { get; set; } = false;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = MakeRoundPath(rect, CornerRadius);

            if (DrawShadow)
            {
                var shadowRect = new Rectangle(2, 4, Width - 5, Height - 5);
                using var sp = MakeRoundPath(shadowRect, CornerRadius);
                using var sb = new SolidBrush(Color.FromArgb(60, 0, 0, 0));
                g.FillPath(sb, sp);
            }

            using (var b = new SolidBrush(BackColor))
                g.FillPath(b, path);

            if (BorderThickness > 0 && BorderColor != Color.Empty)
            {
                using var pen = new Pen(BorderColor, BorderThickness);
                g.DrawPath(pen, path);
            }

            Region = new Region(path);
        }

        public static GraphicsPath MakeRoundPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            if (d <= 0)
            {
                p.AddRectangle(r);
                return p;
            }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
