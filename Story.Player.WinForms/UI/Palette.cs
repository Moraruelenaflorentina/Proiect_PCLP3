using System.Drawing;

namespace Story.Player.WinForms.UI
{
    public static class Palette
    {
        public static readonly Color Background    = Color.FromArgb(15, 10, 26);
        public static readonly Color Surface       = Color.FromArgb(26, 20, 40);
        public static readonly Color SurfaceHigh   = Color.FromArgb(42, 31, 61);
        public static readonly Color SurfaceHover  = Color.FromArgb(60, 42, 90);

        public static readonly Color Primary       = Color.FromArgb(199, 125, 255);
        public static readonly Color PrimaryBright = Color.FromArgb(230, 170, 255);
        public static readonly Color Accent        = Color.FromArgb(255, 110, 199);
        public static readonly Color Mint          = Color.FromArgb(126, 232, 250);

        public static readonly Color Text          = Color.FromArgb(240, 230, 255);
        public static readonly Color TextMuted     = Color.FromArgb(151, 136, 181);
        public static readonly Color TextDim       = Color.FromArgb(110, 100, 140);

        public static readonly Color SanityFrom    = Color.FromArgb(255, 110, 199);
        public static readonly Color SanityTo      = Color.FromArgb(199, 125, 255);

        public static readonly Color HopeFrom      = Color.FromArgb(126, 232, 250);
        public static readonly Color HopeTo        = Color.FromArgb(140, 170, 255);

        public static readonly Color KnowledgeFrom = Color.FromArgb(255, 200, 100);
        public static readonly Color KnowledgeTo   = Color.FromArgb(255, 110, 199);
    }
}
