using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Story.Player.WinForms.UI
{
    
    public class HudMeter : Control
    {
        public string Label { get; set; } = "";
        public string Glyph { get; set; } = "♥";
        public int Value { get; set; } = 0;
        public int MaxValue { get; set; } = 100;
        public Color BarFrom { get; set; } = Palette.SanityFrom;
        public Color BarTo { get; set; } = Palette.SanityTo;

        private readonly ToolTip _tip = new();

        public HudMeter()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Width = 138;
            Height = 38;
        }

        public void Set(string label, string glyph, int value, int max, Color from, Color to)
        {
            Label = label; Glyph = glyph; Value = value; MaxValue = max;
            BarFrom = from; BarTo = to;
            _tip.SetToolTip(this, $"{label}: {value}/{max}");
            Invalidate();
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
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int circle = 28;
            var circleRect = new Rectangle(2, (Height - circle) / 2, circle, circle);

            using (var glowPen = new Pen(Color.FromArgb(50, BarTo), 1f))
                g.DrawEllipse(glowPen,
                    circleRect.X - 1, circleRect.Y - 1,
                    circle + 2, circle + 2);

            using (var grad = new LinearGradientBrush(circleRect, BarFrom, BarTo, LinearGradientMode.Vertical))
                g.FillEllipse(grad, circleRect);
            using (var ringPen = new Pen(Color.FromArgb(170, Color.White), 1f))
                g.DrawEllipse(ringPen, circleRect);

            using (var glyphFont = new Font("Segoe UI Symbol", 13f, FontStyle.Bold))
            using (var glyphBrush = new SolidBrush(Color.White))
            {
                var gs = g.MeasureString(Glyph, glyphFont);
                g.DrawString(Glyph, glyphFont, glyphBrush,
                    circleRect.X + (circle - gs.Width) / 2f,
                    circleRect.Y + (circle - gs.Height) / 2f + 0.5f);
            }

            string valStr = Value.ToString();
            using var valFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var valBrush = new SolidBrush(Palette.Text);
            var vs = g.MeasureString(valStr, valFont);
            float valX = Width - vs.Width - 2;
            float valY = (Height - vs.Height) / 2f;
            g.DrawString(valStr, valFont, valBrush, valX, valY);

            int barLeft  = circleRect.Right + 8;
            int barRight = (int)valX - 8;
            int barW = System.Math.Max(20, barRight - barLeft);
            int barH = 6;
            var trackRect = new Rectangle(barLeft, (Height - barH) / 2, barW, barH);

            using (var trackPath = RoundedPanel.MakeRoundPath(trackRect, barH / 2))
            using (var trackBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                g.FillPath(trackBrush, trackPath);

            float pct = MaxValue > 0 ? (float)Value / MaxValue : 0;
            pct = System.Math.Max(0, System.Math.Min(1, pct));
            int fillW = (int)(trackRect.Width * pct);
            if (fillW > 2)
            {
                var fillRect = new Rectangle(trackRect.X, trackRect.Y, fillW, barH);
                using var fillPath = RoundedPanel.MakeRoundPath(fillRect, barH / 2);
                using var grad = new LinearGradientBrush(fillRect, BarFrom, BarTo, LinearGradientMode.Horizontal);
                g.FillPath(grad, fillPath);

                var sheenRect = new RectangleF(fillRect.X, fillRect.Y, fillRect.Width, fillRect.Height * 0.45f);
                using var sheen = new LinearGradientBrush(sheenRect,
                    Color.FromArgb(80, Color.White),
                    Color.FromArgb(0, Color.White),
                    LinearGradientMode.Vertical);
                var oldClip = g.Clip;
                g.SetClip(fillPath);
                g.FillRectangle(sheen, sheenRect);
                g.Clip = oldClip;
            }
        }
    }
}
