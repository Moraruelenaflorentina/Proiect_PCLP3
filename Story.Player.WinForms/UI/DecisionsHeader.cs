using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Story.Player.WinForms.UI
{
    public class DecisionsHeader : Control
    {
        public string Title { get; set; } = "Ce alegi?";

        public DecisionsHeader()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Height = 28;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using var b = new SolidBrush(Parent.BackColor);
                pevent.Graphics.FillRectangle(b, ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using var font   = new Font("Georgia", 12.5f, FontStyle.Italic);
            using var brush  = new SolidBrush(Palette.PrimaryBright);

            var size = g.MeasureString(Title, font);
            float midX = Width / 2f;
            float midY = Height / 2f;
            float titleX = midX - size.Width / 2f;
            g.DrawString(Title, font, brush, titleX, midY - size.Height / 2f);

            float pad = 18f;
            float lineEndL = titleX - pad;
            float lineStartR = titleX + size.Width + pad;
            float lineLen = 110f;

            using var pen = new Pen(Color.FromArgb(120, Palette.Primary), 1.2f);
            g.DrawLine(pen, lineEndL - lineLen, midY, lineEndL, midY);
            g.DrawLine(pen, lineStartR, midY, lineStartR + lineLen, midY);

            using var dotBrush = new SolidBrush(Color.FromArgb(220, Palette.Accent));
            DrawDiamond(g, dotBrush, lineEndL - lineLen, midY, 3.5f);
            DrawDiamond(g, dotBrush, lineEndL,           midY, 3.5f);
            DrawDiamond(g, dotBrush, lineStartR,         midY, 3.5f);
            DrawDiamond(g, dotBrush, lineStartR + lineLen, midY, 3.5f);
        }

        private static void DrawDiamond(Graphics g, Brush b, float cx, float cy, float r)
        {
            var pts = new[]
            {
                new PointF(cx,     cy - r),
                new PointF(cx + r, cy),
                new PointF(cx,     cy + r),
                new PointF(cx - r, cy)
            };
            g.FillPolygon(b, pts);
        }
    }
}
