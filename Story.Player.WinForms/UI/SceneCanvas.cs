using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Story.Model;

namespace Story.Player.WinForms.UI
{
    
    public class SceneCanvas : Control
    {
        
        public string StoryTitle { get; set; } = "StoryTeller";
        public string TempImageDir { get; set; } = "";
        public StoryBlock? CurrentBlock { get; private set; }
        public List<DecisionDefinition> AvailableDecisions { get; private set; } = new();
        public Dictionary<string, double> StatValues { get; private set; } = new();
        public List<StatePropertyDefinition> StatProps { get; private set; } = new();

        public Action<DecisionDefinition>? OnChoiceClicked;
        public Action? OnRestartClicked;

        private Image? _sceneImage;
        private Bitmap? _sceneFinal;
        private Image? _grain;
        private Bitmap? _atmosCache;
        private Size _atmosCacheSize;
        private Bitmap? _grainTileCache;
        private Bitmap? _staticContentCache;
        private Size _staticContentSize;
        private bool _staticContentDirty = true;

        private readonly System.Windows.Forms.Timer _timer;
        private readonly Stopwatch _clock = new();
        private long _entryStartMs = -1;
        private float _entryT;
        private float _driftPhase;
        private float _grainPhase;
        private float _breathePhase;

        private int _hoverChoiceIndex = -1;
        private bool _hoverRestart;
        private int _hoverHudIndex = -1;
        private RectangleF _restartHit;
        private readonly List<RectangleF> _choiceHits = new();
        private readonly List<(RectangleF rect, string label, Color from, Color to)> _hudHits = new();

        private readonly List<string> _textChunks = new();
        private int _currentChunkIndex = 0;
        private RectangleF _continueHit;
        private bool _hoverContinue;

        public SceneCanvas()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            BackColor = Palette.Background;

            _timer = new System.Windows.Forms.Timer { Interval = 33 };
            _timer.Tick += (s, e) =>
            {
                long now = _clock.ElapsedMilliseconds;
                _driftPhase   = (float)((now % 60000) / 60000.0);
                _grainPhase   = (float)((now %  6000) /  6000.0);
                _breathePhase = (float)((now %  4000) /  4000.0);
                if (_entryStartMs >= 0)
                {
                    float dt = (now - _entryStartMs) / 700f;
                    _entryT = OutCubic(Math.Min(1f, Math.Max(0f, dt)));
                }
                else _entryT = 1f;
                Invalidate();
            };
            _clock.Start();
            _timer.Start();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _timer.Stop(); _timer.Dispose();
            _sceneImage?.Dispose();
            _sceneFinal?.Dispose();
            _grain?.Dispose();
            _atmosCache?.Dispose();
            _grainTileCache?.Dispose();
            _staticContentCache?.Dispose();
            base.OnHandleDestroyed(e);
        }

        protected override void OnResize(EventArgs e)
        {
            
            _atmosCache?.Dispose();
            _atmosCache = null;
            _staticContentDirty = true;
            base.OnResize(e);
            Invalidate();
        }

        public void SetScene(StoryBlock block,
                             List<DecisionDefinition> decisions,
                             Dictionary<string, double> stats,
                             List<StatePropertyDefinition> statProps)
        {
            CurrentBlock = block;
            AvailableDecisions = decisions;
            StatValues = stats;
            StatProps = statProps;
            _hoverChoiceIndex = -1;

            _sceneImage?.Dispose();
            _sceneImage = null;
            _sceneFinal?.Dispose();
            _sceneFinal = null;
            if (!string.IsNullOrEmpty(block.BackgroundImage) && !string.IsNullOrEmpty(TempImageDir))
            {
                string path = Path.Combine(TempImageDir,
                    block.BackgroundImage.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(path))
                {
                    try
                    {
                        _sceneImage = Image.FromFile(path);
                        bool greenGrade = string.Equals(block.Grade, "green", StringComparison.OrdinalIgnoreCase);
                        
                        _sceneFinal = BuildSceneFinal(_sceneImage, greenGrade, 0.84f);
                    }
                    catch { }
                }
            }

            _textChunks.Clear();
            _textChunks.AddRange(SplitIntoChunks(block.Text ?? ""));
            _currentChunkIndex = 0;
            _hoverContinue = false;

            _staticContentDirty = true;

            string grainPath = Path.Combine(TempImageDir, "images", "grain.png");
            if (_grain == null && File.Exists(grainPath))
            {
                try { _grain = Image.FromFile(grainPath); } catch { }
            }

            _entryStartMs = _clock.ElapsedMilliseconds;
            _entryT = 0f;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;

            var rect = ClientRectangle;
            g.Clear(Palette.Background);

            if (_sceneImage != null) DrawSceneImage(g, rect);

            EnsureAtmosCache(rect.Size);
            if (_atmosCache != null) g.DrawImageUnscaled(_atmosCache, 0, 0);

            if (_grain != null) DrawGrain(g, rect);

            DrawHeader(g, rect);

            DrawHud(g, rect);

            if (CurrentBlock == null) { DrawHello(g, rect); return; }

            if (_entryT >= 1f)
            {
                EnsureStaticContentCache(rect);
                if (_staticContentCache != null)
                    g.DrawImageUnscaled(_staticContentCache, 0, 0);

                if (HasMoreChunks())
                {
                    
                }
                else
                {
                    DrawChoiceHoverOverlay(g);
                }
            }
            else
            {
                
                DrawContent(g, rect);
                if (!HasMoreChunks())
                    DrawChoices(g, rect);
                DrawDecorativeFrame(g, rect);
            }
        }

        private void EnsureStaticContentCache(Rectangle rect)
        {
            if (_staticContentCache != null && !_staticContentDirty && _staticContentSize == rect.Size)
                return;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            _staticContentCache?.Dispose();
            _staticContentCache = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppPArgb);
            _staticContentSize = rect.Size;
            _staticContentDirty = false;

            using var g = Graphics.FromImage(_staticContentCache);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            float savedEntry = _entryT;
            int savedHover = _hoverChoiceIndex;
            _entryT = 1f;
            _hoverChoiceIndex = -1;

            DrawContent(g, rect);
            if (!HasMoreChunks())
                DrawChoices(g, rect);
            else
                _choiceHits.Clear();
            DrawDecorativeFrame(g, rect);

            _entryT = savedEntry;
            _hoverChoiceIndex = savedHover;
        }

        private void DrawChoiceHoverOverlay(Graphics g)
        {
            if (_hoverChoiceIndex < 0 || _hoverChoiceIndex >= _choiceHits.Count) return;
            var itemRect = _choiceHits[_hoverChoiceIndex];
            using var hovBrush = new LinearGradientBrush(itemRect,
                Color.FromArgb(40, 255, 140, 225),
                Color.FromArgb(0, 255, 140, 225),
                LinearGradientMode.Horizontal);
            g.FillRectangle(hovBrush, itemRect);
        }

        private void DrawSceneImage(Graphics g, Rectangle rect)
        {
            if (_sceneFinal == null) return;

            float t = _driftPhase;
            float sineT = (float)((Math.Sin(t * Math.PI * 2) + 1) / 2);
            float scale = 1.06f + sineT * 0.10f;
            float tx    = -sineT * rect.Width  * 0.025f;
            float ty    = -sineT * rect.Height * 0.020f;

            int dw = (int)(rect.Width  * scale);
            int dh = (int)(rect.Height * scale);
            int dx = (rect.Width  - dw) / 2 + (int)tx;
            int dy = (rect.Height - dh) / 2 + (int)ty;
            var dest = new Rectangle(dx, dy, dw, dh);

            if (_entryT >= 1f)
            {
                g.DrawImage(_sceneFinal, dest);
                return;
            }

            using var attrs = new ImageAttributes();
            var cm = new ColorMatrix { Matrix33 = _entryT };
            attrs.SetColorMatrix(cm);
            g.DrawImage(_sceneFinal, dest, 0, 0, _sceneFinal.Width, _sceneFinal.Height,
                        GraphicsUnit.Pixel, attrs);
        }

        private static Bitmap BuildSceneFinal(Image source, bool greenGrade, float bright)
        {
            var bmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppPArgb);
            using var g = Graphics.FromImage(bmp);
            using var attrs = new ImageAttributes();
            ColorMatrix m;
            if (greenGrade)
            {
                m = new ColorMatrix(new float[][]
                {
                    new float[] {0.30f * bright, 0.20f * bright, 0.05f * bright, 0, 0},
                    new float[] {0.40f * bright, 0.65f * bright, 0.30f * bright, 0, 0},
                    new float[] {0.10f * bright, 0.25f * bright, 0.30f * bright, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {-0.10f, -0.05f, -0.18f, 0, 1}
                });
            }
            else
            {
                m = new ColorMatrix();
                m.Matrix00 = bright;
                m.Matrix11 = bright;
                m.Matrix22 = bright;
            }
            attrs.SetColorMatrix(m);
            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                        0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attrs);
            return bmp;
        }

        private void EnsureAtmosCache(Size size)
        {
            if (_atmosCache != null && _atmosCacheSize == size) return;
            if (size.Width <= 0 || size.Height <= 0) return;

            _atmosCache?.Dispose();
            _atmosCache = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
            _atmosCacheSize = size;

            using var g = Graphics.FromImage(_atmosCache);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.Clear(Color.Transparent);
            var rect = new Rectangle(0, 0, size.Width, size.Height);

            using (var inner = new GraphicsPath())
            {
                inner.AddEllipse(rect.X - rect.Width * 0.25f, rect.Y - rect.Height * 0.08f,
                                 rect.Width * 1.5f, rect.Height * 1.3f);
                using var pgb = new PathGradientBrush(inner)
                {
                    CenterPoint    = new PointF(rect.X + rect.Width * 0.5f, rect.Y + rect.Height * 0.42f),
                    CenterColor    = Color.FromArgb(0,   8, 3, 16),
                    SurroundColors = new[] { Color.FromArgb(118, 8, 3, 16) }
                };
                g.FillPath(pgb, inner);
            }

            using (var glowEll = new GraphicsPath())
            {
                float gw = rect.Width  * 0.85f;
                float gh = rect.Height * 0.65f;
                float gx = rect.Width  * 0.78f - gw / 2f;
                float gy = rect.Height * 0.12f - gh / 2f;
                glowEll.AddEllipse(gx, gy, gw, gh);
                using var pgb = new PathGradientBrush(glowEll)
                {
                    CenterPoint    = new PointF(gx + gw / 2f, gy + gh / 2f),
                    CenterColor    = Color.FromArgb(6, 196, 90, 255),
                    SurroundColors = new[] { Color.FromArgb(0, 196, 90, 255) }
                };
                g.FillPath(pgb, glowEll);
            }

            int topH = (int)(rect.Height * 0.24f);
            using (var topBrush = new LinearGradientBrush(
                new Rectangle(0, 0, rect.Width, topH),
                Color.FromArgb(180, 6, 4, 11),
                Color.FromArgb(0,   6, 4, 11),
                LinearGradientMode.Vertical))
                g.FillRectangle(topBrush, 0, 0, rect.Width, topH);

            int botH = (int)(rect.Height * 0.74f);
            int botY = rect.Height - botH;
            using (var botBrush = new LinearGradientBrush(
                new Rectangle(0, botY, rect.Width, botH),
                Color.FromArgb(0,   6, 4, 11),
                Color.FromArgb(238, 6, 4, 11),
                LinearGradientMode.Vertical))
                g.FillRectangle(botBrush, 0, botY, rect.Width, botH);
        }

        private void EnsureGrainCache()
        {
            if (_grainTileCache != null || _grain == null) return;
            _grainTileCache = new Bitmap(_grain.Width, _grain.Height, PixelFormat.Format32bppPArgb);
            using var g = Graphics.FromImage(_grainTileCache);
            using var attrs = new ImageAttributes();
            var cm = new ColorMatrix { Matrix33 = 0.07f };
            attrs.SetColorMatrix(cm);
            g.DrawImage(_grain, new Rectangle(0, 0, _grain.Width, _grain.Height),
                        0, 0, _grain.Width, _grain.Height, GraphicsUnit.Pixel, attrs);
        }

        private void DrawGrain(Graphics g, Rectangle rect)
        {
            if (_grain == null) return;
            EnsureGrainCache();
            if (_grainTileCache == null) return;

            float t = _grainPhase;
            float dx = (float)Math.Sin(t * Math.PI * 2) * rect.Width  * 0.025f;
            float dy = (float)Math.Cos(t * Math.PI * 2) * rect.Height * 0.018f;

            int gw = _grainTileCache.Width;
            int gh = _grainTileCache.Height;
            int startX = (int)(-gw + dx);
            int startY = (int)(-gh + dy);

            for (int y = startY; y < rect.Height + gh; y += gh)
                for (int x = startX; x < rect.Width + gw; x += gw)
                    g.DrawImageUnscaled(_grainTileCache, x, y);
        }

        private void DrawHeader(Graphics g, Rectangle rect)
        {
            int padX = 30, padY = 22;

            float breatheAlpha = 0.55f + 0.30f * (float)Math.Abs(Math.Sin(_breathePhase * Math.PI));
            using (var starFont = new Font("Segoe UI Symbol", 11f))
            using (var starBrush = new SolidBrush(Color.FromArgb((int)(255 * breatheAlpha), Palette.Primary)))
                g.DrawString("✦", starFont, starBrush, padX, padY);

            using var titleFont = new Font("Georgia", 13f, FontStyle.Italic | FontStyle.Bold);
            float titleX = padX + 22;
            float titleY = padY - 3;
            DrawGradientText(g, StoryTitle, titleFont,
                new RectangleF(titleX, titleY, 600, 32),
                Color.FromArgb(255, 143, 224), Color.FromArgb(199, 123, 255), 0f);

            var ts = g.MeasureString(StoryTitle, titleFont);
            float codeX = titleX + ts.Width + 12;

            using (var codeFont = new Font("Consolas", 8.5f))
            using (var codeBrush = new SolidBrush(Color.FromArgb(127, 190, 160, 250)))
                g.DrawString("· " + (CurrentBlock?.Id ?? ""), codeFont, codeBrush, codeX, titleY + 6);

            using var restFont = new Font("Consolas", 9.5f, FontStyle.Regular);
            string restartTxt = "⟲ REIA";
            var rs = g.MeasureString(restartTxt, restFont);
            float rx = rect.Width - padX - rs.Width;
            float ry = padY;
            _restartHit = new RectangleF(rx - 4, ry - 2, rs.Width + 8, rs.Height + 4);
            using var restBrush = new SolidBrush(_hoverRestart
                ? Color.FromArgb(255, 255, 150, 220)
                : Color.FromArgb(199, 255, 150, 220));
            g.DrawString(restartTxt, restFont, restBrush, rx, ry);
        }

        private void DrawHud(Graphics g, Rectangle rect)
        {
            _hudHits.Clear();
            int x = 30, y = 70;
            int meterW = 138, meterH = 36, gap = 10;

            int idx = 0;
            int hoveredX = 0, hoveredY = 0, hoveredW = 0;
            string? hoveredLabel = null;
            Color hoveredFrom = Palette.Primary, hoveredTo = Palette.Accent;

            foreach (var prop in StatProps)
            {
                if (!prop.VisibleInHud) continue;
                if (idx >= 3) break;
                int val = (int)Math.Round(StatValues.TryGetValue(prop.Key, out double v) ? v : prop.Initial);
                int max = (int)Math.Round(prop.Max);
                string ht = prop.HudType?.ToLowerInvariant() ?? "";

                (string glyph, Color from, Color to) = ht switch
                {
                    "health"  => ("☉", Palette.SanityFrom,    Palette.SanityTo),
                    "stamina" => ("♥", Palette.HopeFrom,      Palette.HopeTo),
                    "level"   => ("☁", Palette.KnowledgeFrom, Palette.KnowledgeTo),
                    _         => ("◆", Palette.Primary,       Palette.Accent)
                };

                int mx = x + idx * (meterW + gap);
                DrawHudMeter(g, mx, y, meterW, meterH, glyph, val, max, from, to);

                var hitRect = new RectangleF(mx, y - 4, meterW, meterH + 8);
                _hudHits.Add((hitRect, prop.HudLabel, from, to));

                if (idx == _hoverHudIndex)
                {
                    hoveredX = mx; hoveredY = y + meterH; hoveredW = meterW;
                    hoveredLabel = prop.HudLabel; hoveredFrom = from; hoveredTo = to;
                }
                idx++;
            }

            if (hoveredLabel != null)
                DrawHudLabelChip(g, hoveredX, hoveredY + 8, hoveredW, hoveredLabel, hoveredFrom, hoveredTo);
        }

        private static void DrawHudLabelChip(Graphics g, int x, int y, int meterW, string label, Color from, Color to)
        {
            using var font = new Font("Georgia", 10.5f, FontStyle.Italic);
            var size = g.MeasureString(label, font);
            int padX = 14, padY = 5;
            int w = (int)size.Width + padX * 2;
            int h = (int)size.Height + padY * 2;
            
            int chipX = x + (meterW - w) / 2;
            var rect = new Rectangle(chipX, y, w, h);

            using (var bgPath = RoundedPanel.MakeRoundPath(rect, h / 2))
            using (var bg = new SolidBrush(Color.FromArgb(228, 18, 12, 30)))
                g.FillPath(bg, bgPath);

            using (var borderPath = RoundedPanel.MakeRoundPath(rect, h / 2))
            using (var borderBrush = new LinearGradientBrush(rect, from, to, 0f))
            using (var borderPen = new Pen(borderBrush, 1.3f))
                g.DrawPath(borderPen, borderPath);

            using (var arrow = new GraphicsPath())
            {
                int ax = chipX + w / 2;
                arrow.AddPolygon(new[]
                {
                    new Point(ax,     y - 5),
                    new Point(ax - 5, y + 1),
                    new Point(ax + 5, y + 1)
                });
                using var arrBrush = new SolidBrush(Color.FromArgb(228, 18, 12, 30));
                g.FillPath(arrBrush, arrow);
                using var arrPen = new Pen(Color.FromArgb(220, to), 1.2f);
                
                g.DrawLine(arrPen, ax, y - 5, ax - 5, y + 1);
                g.DrawLine(arrPen, ax, y - 5, ax + 5, y + 1);
            }

            using var textBrush = new SolidBrush(Palette.Text);
            g.DrawString(label, font, textBrush, chipX + padX, y + padY - 1);
        }

        private static void DrawHudMeter(Graphics g, int x, int y, int w, int h,
            string glyph, int val, int max, Color from, Color to)
        {
            
            int circle = 28;
            var circleRect = new Rectangle(x + 2, y + (h - circle) / 2, circle, circle);
            using (var grad = new LinearGradientBrush(circleRect, from, to, LinearGradientMode.Vertical))
                g.FillEllipse(grad, circleRect);
            using (var ringPen = new Pen(Color.FromArgb(170, Color.White), 1f))
                g.DrawEllipse(ringPen, circleRect);
            using (var glyphFont = new Font("Segoe UI Symbol", 13f, FontStyle.Bold))
            using (var glyphBrush = new SolidBrush(Color.White))
            {
                var gs = g.MeasureString(glyph, glyphFont);
                g.DrawString(glyph, glyphFont, glyphBrush,
                    circleRect.X + (circle - gs.Width) / 2f,
                    circleRect.Y + (circle - gs.Height) / 2f + 0.5f);
            }

            using var valFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var valBrush = new SolidBrush(Color.FromArgb(240, Palette.Text));
            string valStr = val.ToString();
            var vs = g.MeasureString(valStr, valFont);
            float valX = x + w - vs.Width - 2;
            float valY = y + (h - vs.Height) / 2f;
            g.DrawString(valStr, valFont, valBrush, valX, valY);

            int barLeft  = circleRect.Right + 8;
            int barRight = (int)valX - 8;
            int barW = Math.Max(20, barRight - barLeft);
            int barH = 6;
            var trackRect = new Rectangle(barLeft, y + (h - barH) / 2, barW, barH);
            using (var trackPath = RoundedPanel.MakeRoundPath(trackRect, barH / 2))
            using (var trackBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0)))
                g.FillPath(trackBrush, trackPath);

            float pct = max > 0 ? (float)val / max : 0;
            pct = Math.Max(0, Math.Min(1, pct));
            int fillW = (int)(trackRect.Width * pct);
            if (fillW > 2)
            {
                var fillRect = new Rectangle(trackRect.X, trackRect.Y, fillW, barH);
                using var fillPath = RoundedPanel.MakeRoundPath(fillRect, barH / 2);
                using var grad = new LinearGradientBrush(fillRect, from, to, LinearGradientMode.Horizontal);
                g.FillPath(grad, fillPath);
            }
        }

        private void DrawContent(Graphics g, Rectangle rect)
        {
            if (CurrentBlock == null) return;
            float entryAlpha = _entryT;
            float yLift = (1f - _entryT) * 22f;

            int padL = 132;
            int padR = 56;
            int padB = 56;

            int maxW = (int)((rect.Width - padL - padR) * 0.6f);
            if (maxW < 360) maxW = 360;
            if (maxW > 660) maxW = 660;

            string narrative;
            if (_textChunks.Count > 0)
            {
                int idx = Math.Min(_currentChunkIndex, _textChunks.Count - 1);
                narrative = _textChunks[idx];
            }
            else narrative = CurrentBlock.Text ?? "";
            using var narFont = new Font("Georgia", 13.5f, FontStyle.Regular);
            var narSize = g.MeasureString(narrative, narFont, maxW - 8);

            string title = !string.IsNullOrEmpty(CurrentBlock.Title) ? CurrentBlock.Title : Cap(CurrentBlock.Id);
            using var titleFont = new Font("Georgia", 38f, FontStyle.Italic | FontStyle.Bold);
            var titleSize = g.MeasureString(title, titleFont, maxW);

            string act = CurrentBlock.Act ?? "";
            using var actFont = new Font("Consolas", 8.5f, FontStyle.Bold);
            var actSize = g.MeasureString(act, actFont);

            float totalH = (act.Length > 0 ? actSize.Height + 14 : 0)
                         + titleSize.Height + 22
                         + 1 + 22
                         + narSize.Height;
            float startY = rect.Height - padB - totalH + yLift;
            float startX = padL;

            if (act.Length > 0)
            {
                using var actBrush = new SolidBrush(Color.FromArgb((int)(184 * entryAlpha), 255, 165, 225));
                g.DrawString(act.ToUpperInvariant(), actFont, actBrush, startX, startY);
                startY += actSize.Height + 14;
            }

            DrawGradientText(g, title, titleFont,
                new RectangleF(startX, startY, maxW, titleSize.Height + 4),
                Color.FromArgb((int)(255 * entryAlpha), 255, 240, 251),
                Color.FromArgb((int)(255 * entryAlpha), 199, 123, 255),
                10f);
            startY += titleSize.Height + 22;

            using (var linePen = new Pen(Color.FromArgb((int)(204 * entryAlpha), 255, 150, 225), 1.2f))
                g.DrawLine(linePen, startX, startY, startX + 56, startY);
            startY += 22;

            using var narBrush = new SolidBrush(Color.FromArgb((int)(240 * entryAlpha), 236, 228, 252));
            g.DrawString(narrative, narFont, narBrush, new RectangleF(startX, startY, maxW, narSize.Height + 4));

            if (HasMoreChunks())
            {
                float indY = startY + narSize.Height + 18;
                DrawContinueIndicator(g, startX, indY, maxW);
            }
        }

        private static List<string> SplitIntoChunks(string text)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(text)) return result;
            var paragraphs = text.Split(new[] { "\n\n" }, StringSplitOptions.None);
            const int target = 360;
            var current = new System.Text.StringBuilder();
            foreach (var p in paragraphs)
            {
                if (current.Length > 0 && current.Length + p.Length > target)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                if (current.Length > 0) current.Append("\n\n");
                current.Append(p);
            }
            if (current.Length > 0) result.Add(current.ToString());
            return result;
        }

        private bool HasMoreChunks() =>
            _textChunks.Count > 1 && _currentChunkIndex < _textChunks.Count - 1;

        private void DrawContinueIndicator(Graphics g, float startX, float y, int maxW)
        {
            
            float pulse = 0.55f + 0.35f * (float)Math.Abs(Math.Sin(_breathePhase * Math.PI));
            int alpha = (int)(255 * pulse);

            using var font = new Font("Consolas", 9f, FontStyle.Bold);
            string label = (_hoverContinue ? "▾" : "▼") + "  CONTINUĂ  " + (_hoverContinue ? "▾" : "▼");
            var sz = g.MeasureString(label, font);
            float x = startX + (maxW - sz.Width) / 2f;

            using var brush = new SolidBrush(
                _hoverContinue
                    ? Color.FromArgb(255, 255, 200, 235)
                    : Color.FromArgb(alpha, 255, 150, 220));
            g.DrawString(label, font, brush, x, y);

            _continueHit = new RectangleF(startX, y - 8, maxW, sz.Height + 16);
        }

        private void DrawChoices(Graphics g, Rectangle rect)
        {
            _choiceHits.Clear();
            if (CurrentBlock == null) return;
            float entryAlpha = _entryT;
            float yLift = (1f - _entryT) * 28f;

            int padR = 56;
            int padB = 56;
            int colW = 410;
            int colX = rect.Width - padR - colW;
            int x    = colX;

            if (CurrentBlock.IsFinal)
            {
                using var finalFont = new Font("Georgia", 22f, FontStyle.Italic | FontStyle.Bold);
                string fl = CurrentBlock.FinalLine ?? "";
                var fs = g.MeasureString(fl, finalFont, colW);
                float fy = rect.Height - padB - fs.Height - 80 + yLift;

                using (var accentPen = new Pen(Color.FromArgb((int)(150 * entryAlpha), 255, 150, 225), 1f))
                    g.DrawLine(accentPen, x - 24, fy, x - 24, fy + fs.Height + 70);

                using var flBrush = new SolidBrush(Color.FromArgb((int)(252 * entryAlpha), 251, 238, 255));
                g.DrawString(fl, finalFont, flBrush, new RectangleF(x, fy, colW, fs.Height + 4));

                using var endFont = new Font("Consolas", 8.5f, FontStyle.Bold);
                using var endBrush = new SolidBrush(Color.FromArgb((int)(180 * entryAlpha), 190, 160, 250));
                g.DrawString("— SFÂRȘIT —", endFont, endBrush, x, fy + fs.Height + 22);

                return;
            }

            string header = "CE ALEGI";
            using var headerFont = new Font("Consolas", 8.5f, FontStyle.Bold);
            using var headerBrush = new SolidBrush(Color.FromArgb((int)(140 * entryAlpha), 190, 160, 250));
            var hSize = g.MeasureString(header, headerFont);

            string[] numerals = { "I", "II", "III", "IV", "V", "VI", "VII" };
            using var labelFont = new Font("Georgia", 13.5f, FontStyle.Italic);
            using var numFont   = new Font("Georgia", 18f, FontStyle.Italic | FontStyle.Bold);
            using var noteFont  = new Font("Consolas", 7.5f, FontStyle.Bold);

            int gap = 0;
            int sep = 1;
            int padV = 14;
            int n = AvailableDecisions.Count;
            float[] heights = new float[n];
            for (int i = 0; i < n; i++)
            {
                var labelSize = g.MeasureString(AvailableDecisions[i].Text, labelFont, colW - 70);
                heights[i] = Math.Max(28f, labelSize.Height) + padV * 2;
            }
            float totalChoiceH = 0;
            for (int i = 0; i < n; i++) totalChoiceH += heights[i] + sep;
            float totalH = hSize.Height + 8 + totalChoiceH;
            float startY = rect.Height - padB - totalH + yLift;

            g.DrawString(header, headerFont, headerBrush, x + 18, startY);
            startY += hSize.Height + 8;

            using (var sepPen = new Pen(Color.FromArgb((int)(40 * entryAlpha), 180, 130, 255), 1f))
                g.DrawLine(sepPen, x, startY, x + colW, startY);

            float curY = startY;
            for (int i = 0; i < n; i++)
            {
                bool hov = i == _hoverChoiceIndex;
                float h = heights[i];
                float hoverShift = hov ? 12f : 0f;
                var itemRect = new RectangleF(x, curY, colW, h);
                _choiceHits.Add(itemRect);

                if (hov)
                {
                    using var hovBrush = new LinearGradientBrush(itemRect,
                        Color.FromArgb((int)(40 * entryAlpha), 255, 140, 225),
                        Color.FromArgb(0, 255, 140, 225),
                        LinearGradientMode.Horizontal);
                    g.FillRectangle(hovBrush, itemRect);
                }

                string num = i < numerals.Length ? numerals[i] : (i + 1).ToString();
                var numRect = new RectangleF(x + 18 + hoverShift, curY + padV - 2, 80, 30);
                DrawGradientText(g, num, numFont, numRect,
                    Color.FromArgb((int)(255 * entryAlpha), 255, 143, 224),
                    Color.FromArgb((int)(255 * entryAlpha), 199, 123, 255), 0f);

                using var labelBrush = new SolidBrush(Color.FromArgb((int)(244 * entryAlpha), 236, 228, 252));
                var labelRect = new RectangleF(x + 60 + hoverShift, curY + padV - 2, colW - 90 - hoverShift, h - padV * 2 + 10);
                g.DrawString(AvailableDecisions[i].Text, labelFont, labelBrush, labelRect);

                string? note = AvailableDecisions[i].Note;
                if (!string.IsNullOrEmpty(note))
                {
                    var noteSize = g.MeasureString(note, noteFont);
                    float pillW = noteSize.Width + 18;
                    float pillH = noteSize.Height + 6;
                    var pillRect = new RectangleF(x + colW - pillW - 8, curY + (h - pillH) / 2f, pillW, pillH);
                    using (var pillPath = RoundedPanel.MakeRoundPath(Rectangle.Round(pillRect), (int)(pillH / 2)))
                    using (var pillPen  = new Pen(Color.FromArgb((int)(70 * entryAlpha), 255, 150, 225), 1f))
                        g.DrawPath(pillPen, pillPath);
                    using var noteBrush = new SolidBrush(Color.FromArgb((int)(180 * entryAlpha), 255, 165, 225));
                    g.DrawString(note, noteFont, noteBrush,
                        pillRect.X + 9, pillRect.Y + 3);
                }

                curY += h;
                using (var sepPen = new Pen(Color.FromArgb((int)(40 * entryAlpha), 180, 130, 255), 1f))
                    g.DrawLine(sepPen, x, curY, x + colW, curY);
                curY += sep;
            }
        }

        private void DrawDecorativeFrame(Graphics g, Rectangle rect)
        {
            int inset = 15;
            var frame = new Rectangle(inset, inset, rect.Width - inset * 2 - 1, rect.Height - inset * 2 - 1);
            using (var pen = new Pen(Color.FromArgb(46, 190, 140, 255), 1f))
                g.DrawRectangle(pen, frame);

            using var starFont = new Font("Segoe UI Symbol", 9f);
            using var starBrush = new SolidBrush(Color.FromArgb(127, 255, 150, 225));
            g.DrawString("✦", starFont, starBrush, frame.Left - 7, frame.Top  - 11);
            g.DrawString("✦", starFont, starBrush, frame.Right - 7, frame.Top - 11);
            g.DrawString("✦", starFont, starBrush, frame.Left - 7, frame.Bottom - 8);
            g.DrawString("✦", starFont, starBrush, frame.Right - 7, frame.Bottom - 8);
        }

        private void DrawHello(Graphics g, Rectangle rect)
        {
            string msg = "Deschide o poveste din meniul «Poveste → Deschide ZIP…»";
            using var f = new Font("Georgia", 13f, FontStyle.Italic);
            using var b = new SolidBrush(Color.FromArgb(200, Palette.TextMuted));
            var s = g.MeasureString(msg, f);
            g.DrawString(msg, f, b, (rect.Width - s.Width) / 2, (rect.Height - s.Height) / 2);
            DrawDecorativeFrame(g, rect);
        }

        private static void DrawGradientText(Graphics g, string text, Font font, RectangleF rect,
                                             Color cFrom, Color cTo, float angle)
        {
            if (string.IsNullOrEmpty(text)) return;
            using var path = new GraphicsPath();
            using var fmt = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
            float emSize = g.DpiY * font.SizeInPoints / 72f;
            path.AddString(text, font.FontFamily, (int)font.Style, emSize, rect, fmt);
            using var brush = new LinearGradientBrush(rect, cFrom, cTo, angle);
            g.FillPath(brush, path);
        }

        private static string Cap(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var parts = s.Split('.');
            var output = new List<string>();
            foreach (var p in parts)
                output.Add(p.Length == 0 ? "" : char.ToUpperInvariant(p[0]) + p.Substring(1));
            return string.Join(" · ", output);
        }

        private static float OutCubic(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            float p = 1f - t;
            return 1f - p * p * p;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int newHover = -1;
            if (!HasMoreChunks())
            {
                for (int i = 0; i < _choiceHits.Count; i++)
                    if (_choiceHits[i].Contains(e.Location)) { newHover = i; break; }
            }
            bool newRestart = _restartHit.Contains(e.Location);

            int newHudHover = -1;
            for (int i = 0; i < _hudHits.Count; i++)
                if (_hudHits[i].rect.Contains(e.Location)) { newHudHover = i; break; }

            bool newContinue = HasMoreChunks() && _continueHit.Contains(e.Location);

            if (newHover != _hoverChoiceIndex || newRestart != _hoverRestart
                || newHudHover != _hoverHudIndex || newContinue != _hoverContinue)
            {
                _hoverChoiceIndex = newHover;
                _hoverRestart = newRestart;
                _hoverHudIndex = newHudHover;
                _hoverContinue = newContinue;
                Cursor = (newHover >= 0 || newRestart || newContinue) ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_hoverRestart) { OnRestartClicked?.Invoke(); return; }
            if (_hoverContinue && HasMoreChunks())
            {
                _currentChunkIndex++;
                _staticContentDirty = true;
                Invalidate();
                return;
            }
            
            if (HasMoreChunks() && !_restartHit.Contains(e.Location)
                && _hoverHudIndex < 0)
            {
                _currentChunkIndex++;
                _staticContentDirty = true;
                Invalidate();
                return;
            }
            if (_hoverChoiceIndex >= 0 && _hoverChoiceIndex < AvailableDecisions.Count)
            {
                OnChoiceClicked?.Invoke(AvailableDecisions[_hoverChoiceIndex]);
                return;
            }
            base.OnMouseDown(e);
        }
    }
}
