using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Story.Player.WinForms.UI
{
    
    public class DecisionCard : Control
    {
        public int CornerRadius { get; set; } = 20;
        public string DecisionText { get; set; } = "";
        public int Index { get; set; } = 0;
        public int EntryDelayMs { get; set; } = 0;

        private bool _hover;
        private Point _mouse;
        private float _hoverT;
        private float _entryT;
        private float _phase;
        private readonly Stopwatch _clock = new();
        private readonly System.Windows.Forms.Timer _timer;

        private static readonly (Color From, Color To, Color Accent, string Ornament)[] Palettes =
        {
            (Color.FromArgb(255, 110, 199), Color.FromArgb(199, 125, 255), Color.FromArgb(255, 130, 220), "❦"),
            (Color.FromArgb(126, 232, 250), Color.FromArgb(140, 170, 255), Color.FromArgb(140, 220, 255), "✧"),
            (Color.FromArgb(255, 200, 100), Color.FromArgb(255, 110, 199), Color.FromArgb(255, 180, 150), "☙"),
            (Color.FromArgb(200, 200, 230), Color.FromArgb(170, 130, 220), Color.FromArgb(220, 200, 240), "⟡"),
        };

        public DecisionCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Height = 240;

            _timer = new System.Windows.Forms.Timer { Interval = 16 };
            _timer.Tick += OnTick;
        }

        public void StartEntry()
        {
            _entryT = 0f;
            _clock.Restart();
            _timer.Start();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();
            base.OnHandleDestroyed(e);
        }

        private void OnTick(object? s, EventArgs e)
        {
            float hoverTarget = _hover ? 1f : 0f;
            _hoverT += (hoverTarget - _hoverT) * 0.18f;
            if (Math.Abs(hoverTarget - _hoverT) < 0.002f) _hoverT = hoverTarget;

            long ms = _clock.ElapsedMilliseconds - EntryDelayMs;
            _entryT = ms <= 0 ? 0f : Math.Min(1f, ms / 420f);

            _phase = (float)((_clock.ElapsedMilliseconds % 6000) / 6000.0);

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true;  base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; base.OnMouseLeave(e); }
        protected override void OnMouseMove(MouseEventArgs e) { _mouse = e.Location; base.OnMouseMove(e); }

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
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var pal = Palettes[((Index % Palettes.Length) + Palettes.Length) % Palettes.Length];

            float entryEase = OutCubic(_entryT);
            float hoverEase = OutCubic(_hoverT);
            float alpha     = entryEase;
            float yOffset   = (1f - entryEase) * 30f;

            float scale = 1f + hoverEase * 0.03f;
            float w = Width, h = Height;
            float sw = w * scale, sh = h * scale;
            float sx = (w - sw) / 2f;
            float sy = (h - sh) / 2f + yOffset;
            var rect = new RectangleF(sx, sy, sw - 1, sh - 1);

            using var path = MakeRoundPathF(rect, CornerRadius);
            Region = new Region(path);

            if (hoverEase > 0.04f)
            {
                for (int i = 1; i <= 8; i++)
                {
                    var glowR = new RectangleF(rect.X - i, rect.Y - i + 2, rect.Width + i * 2, rect.Height + i * 2);
                    using var glowP = MakeRoundPathF(glowR, CornerRadius + i);
                    int a = Math.Min(60, (int)(40 * hoverEase / i));
                    using var pen = new Pen(Color.FromArgb(a, pal.Accent), 1.6f);
                    g.DrawPath(pen, glowP);
                }
            }

            Color baseTop    = Lerp(Palette.SurfaceHigh, Palette.SurfaceHover, hoverEase * 0.7f);
            Color baseBottom = Lerp(Palette.Surface,     Color.FromArgb(60, 36, 92), hoverEase * 0.7f);
            using (var bgBrush = new LinearGradientBrush(rect,
                Color.FromArgb((int)(255 * alpha), baseTop),
                Color.FromArgb((int)(255 * alpha), baseBottom),
                LinearGradientMode.Vertical))
                g.FillPath(bgBrush, path);

            using (var inner = new GraphicsPath())
            {
                inner.AddEllipse(rect.X - rect.Width * 0.2f, rect.Y + rect.Height * 0.1f,
                                 rect.Width * 1.4f, rect.Height * 1.2f);
                using var pgb = new PathGradientBrush(inner)
                {
                    CenterColor = Color.FromArgb(0, 0, 0, 0),
                    SurroundColors = new[] { Color.FromArgb((int)(90 * alpha), 0, 0, 0) },
                    CenterPoint   = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height * 0.45f)
                };
                var oldClip = g.Clip;
                g.SetClip(path);
                g.FillPath(pgb, inner);
                g.Clip = oldClip;
            }

            if (_hover && hoverEase > 0.08f)
            {
                float spotR = 260f;
                var spotEll = new RectangleF(_mouse.X - spotR, _mouse.Y - spotR, spotR * 2, spotR * 2);
                using var spotPath = new GraphicsPath();
                spotPath.AddEllipse(spotEll);
                using var pgb = new PathGradientBrush(spotPath)
                {
                    CenterColor = Color.FromArgb((int)(90 * hoverEase * alpha), pal.Accent),
                    SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) },
                    CenterPoint = _mouse
                };
                var oldClip = g.Clip;
                g.SetClip(path);
                g.FillPath(pgb, spotPath);
                g.Clip = oldClip;
            }

            var sheenR = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height * 0.32f);
            using (var sheen = new LinearGradientBrush(sheenR,
                Color.FromArgb((int)(36 * alpha), 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                var oldClip = g.Clip;
                g.SetClip(path);
                g.FillRectangle(sheen, sheenR);
                g.Clip = oldClip;
            }

            float angle = _phase * 360f;
            int borderA = (int)((150 + 90 * hoverEase) * alpha);
            using (var bbrush = new LinearGradientBrush(rect,
                Color.FromArgb(borderA, pal.From),
                Color.FromArgb(borderA, pal.To),
                angle))
            using (var bpen = new Pen(bbrush, 1.4f + hoverEase * 1f))
                g.DrawPath(bpen, path);

            string numeral = ToRoman(Index + 1);
            int headerY = (int)rect.Y + 14;
            using (var numFont = new Font("Georgia", 13f, FontStyle.Italic | FontStyle.Bold))
            using (var numBrush = new SolidBrush(Color.FromArgb((int)(230 * alpha), pal.Accent)))
                g.DrawString(numeral, numFont, numBrush, rect.X + 18, headerY);

            using (var ornFont = new Font("Segoe UI Symbol", 18f))
            using (var ornBrush = new SolidBrush(Color.FromArgb((int)(180 * alpha + 70 * hoverEase), pal.From)))
            {
                var os = g.MeasureString(pal.Ornament, ornFont);
                g.DrawString(pal.Ornament, ornFont, ornBrush,
                    rect.X + (rect.Width - os.Width) / 2f, headerY - 4);
            }

            using (var linePen = new Pen(Color.FromArgb((int)(110 * alpha), pal.From), 1.2f))
            {
                float lineY = headerY + 30;
                float midX  = rect.X + rect.Width / 2f;
                float halfW = 50f + hoverEase * 22f;
                g.DrawLine(linePen, midX - halfW, lineY, midX - 12, lineY);
                g.DrawLine(linePen, midX + 12, lineY, midX + halfW, lineY);
                
                using var dotBrush = new SolidBrush(Color.FromArgb((int)(220 * alpha), pal.Accent));
                g.FillEllipse(dotBrush, midX - 2.5f, lineY - 2.5f, 5, 5);
            }

            using (var font = new Font("Georgia", 13f, FontStyle.Italic))
            using (var tBrush = new SolidBrush(Color.FromArgb((int)(248 * alpha), Palette.Text)))
            {
                var textRect = new RectangleF(rect.X + 22, rect.Y + 76,
                                              rect.Width - 44, rect.Height - 130);
                using var fmt = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment     = StringAlignment.Center,
                    Trimming      = StringTrimming.EllipsisWord
                };
                g.DrawString(DecisionText ?? "", font, tBrush, textRect, fmt);
            }

            float footY = rect.Bottom - 30;
            using (var footPen = new Pen(Color.FromArgb((int)(140 * alpha), pal.From), 1.1f))
            {
                float midX  = rect.X + rect.Width / 2f;
                float halfW = 36f + hoverEase * 14f;
                g.DrawLine(footPen, midX - halfW, footY, midX + halfW, footY);
            }

            using (var chevFont = new Font("Segoe UI", 16f, FontStyle.Bold))
            using (var chevBrush = new SolidBrush(Color.FromArgb((int)(230 * alpha),
                Lerp(pal.From, pal.Accent, hoverEase))))
            {
                var cs = g.MeasureString("›", chevFont);
                float cx = rect.Right - cs.Width - 16 + hoverEase * 8f;
                float cy = rect.Bottom - cs.Height - 6;
                g.DrawString("›", chevFont, chevBrush, cx, cy);
            }
        }

        private static float OutCubic(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            float p = 1f - t;
            return 1f - p * p * p;
        }

        private static Color Lerp(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private static GraphicsPath MakeRoundPathF(RectangleF r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X,            r.Y,             d, d, 180, 90);
            p.AddArc(r.Right - d,    r.Y,             d, d, 270, 90);
            p.AddArc(r.Right - d,    r.Bottom - d,    d, d,   0, 90);
            p.AddArc(r.X,            r.Bottom - d,    d, d,  90, 90);
            p.CloseFigure();
            return p;
        }

        private static string ToRoman(int n)
        {
            if (n <= 0) return "";
            string[] r = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X",
                           "XI","XII","XIII","XIV","XV","XVI","XVII","XVIII","XIX","XX" };
            return n <= r.Length ? r[n - 1] : n.ToString();
        }
    }
}
