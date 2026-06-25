using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Story.Player.WinForms.UI
{
    public class SceneCard : Control
    {
        public int CornerRadius { get; set; } = 22;
        public Image? SceneImage { get; set; }
        public string SceneTitle { get; set; } = "";

        public SceneCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            BackColor = Palette.Surface;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedPanel.MakeRoundPath(rect, CornerRadius);
            Region = new Region(path);

            using (var bg = new SolidBrush(Palette.Surface))
                g.FillPath(bg, path);

            if (SceneImage != null)
            {
                var imgRect = ComputeImageRect(rect);
                var oldClip = g.Clip;
                g.SetClip(path);
                g.DrawImage(SceneImage, imgRect);
                g.Clip = oldClip;

                int gradH = (int)(Height * 0.45);
                var gradRect = new Rectangle(0, Height - gradH, Width, gradH);
                using var gradBrush = new LinearGradientBrush(
                    gradRect,
                    Color.FromArgb(0, Palette.Background),
                    Color.FromArgb(230, Palette.Background),
                    LinearGradientMode.Vertical);
                var oldClip2 = g.Clip;
                g.SetClip(path);
                g.FillRectangle(gradBrush, gradRect);
                g.Clip = oldClip2;
            }
            else
            {
                using var placeholder = new SolidBrush(Palette.SurfaceHigh);
                g.FillPath(placeholder, path);
                using var f = new Font("Segoe UI", 11f, FontStyle.Italic);
                var sz = g.MeasureString("(fără imagine)", f);
                using var tb = new SolidBrush(Palette.TextDim);
                g.DrawString("(fără imagine)", f, tb,
                    (Width - sz.Width) / 2, (Height - sz.Height) / 2);
            }

            if (!string.IsNullOrEmpty(SceneTitle))
            {
                using var titleFont = new Font("Georgia", 16f, FontStyle.Italic);
                var tsize = g.MeasureString(SceneTitle, titleFont);
                int px = 24;
                int py = Height - (int)tsize.Height - 20;

                using var glow = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
                g.FillRectangle(glow, px - 4, py - 2, (int)tsize.Width + 12, (int)tsize.Height + 4);

                using var tbrush = new SolidBrush(Palette.PrimaryBright);
                g.DrawString(SceneTitle, titleFont, tbrush, px, py);

                using var lineP = new Pen(Palette.Primary, 2);
                g.DrawLine(lineP, px, py + (int)tsize.Height + 1,
                                  px + 40, py + (int)tsize.Height + 1);
            }

            using var borderPen = new Pen(Color.FromArgb(80, Palette.Primary), 1.5f);
            g.DrawPath(borderPen, path);
        }

        private Rectangle ComputeImageRect(Rectangle bounds)
        {
            if (SceneImage == null) return bounds;
            float imgAspect = (float)SceneImage.Width / SceneImage.Height;
            float boxAspect = (float)bounds.Width / bounds.Height;
            if (imgAspect > boxAspect)
            {
                int h = bounds.Height;
                int w = (int)(h * imgAspect);
                int x = bounds.X - (w - bounds.Width) / 2;
                return new Rectangle(x, bounds.Y, w, h);
            }
            else
            {
                int w = bounds.Width;
                int h = (int)(w / imgAspect);
                int y = bounds.Y - (h - bounds.Height) / 2;
                return new Rectangle(bounds.X, y, w, h);
            }
        }
    }
}
