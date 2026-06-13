using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Story.Engine;
using Story.Model;
using Story.Persistence;

namespace Story.Player.WinForms
{
    public partial class PlayerForm : Form
    {
        private readonly GameEngine _engine = new();
        private string _tempDir = "";
        private string _lastZipPath = "";

        public PlayerForm()
        {
            InitializeComponent();
        }

        // ── Story Loading ─────────────────────────────────────────────────────

        private void menuOpen_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Open Story File",
                Filter = "Story archives (*.zip)|*.zip|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var (story, tempDir) = StoryRepository.LoadFromZip(dlg.FileName);
                _tempDir = tempDir;
                _lastZipPath = dlg.FileName;
                _engine.LoadStory(story);
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading story:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuRestart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_lastZipPath)) return;
            try
            {
                var (story, tempDir) = StoryRepository.LoadFromZip(_lastZipPath);
                _tempDir = tempDir;
                _engine.LoadStory(story);
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restart failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuSaveState_Click(object sender, EventArgs e)
        {
            if (_engine.CurrentBlock == null) return;
            using var dlg = new SaveFileDialog { Filter = "State files (*.json)|*.json", Title = "Save State" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var data = new { Block = _engine.CurrentBlock.Id, State = _engine.State.Serialize() };
            File.WriteAllText(dlg.FileName, System.Text.Json.JsonSerializer.Serialize(data));
            MessageBox.Show("State saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuLoadState_Click(object sender, EventArgs e)
        {
            if (_engine.CurrentBlock == null) return;
            using var dlg = new OpenFileDialog { Filter = "State files (*.json)|*.json", Title = "Load State" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var json = File.ReadAllText(dlg.FileName);
                var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
                string blockId = data.GetProperty("Block").GetString()!;
                string stateJson = data.GetProperty("State").GetString()!;
                _engine.State.Deserialize(stateJson);
                _engine.NavigateTo(blockId);
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading state:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuExit_Click(object sender, EventArgs e) => Close();

        // ── Display ───────────────────────────────────────────────────────────

        private void RefreshDisplay()
        {
            var block = _engine.CurrentBlock;
            if (block == null) return;

            lblTitle.Text = _engine.StoryTitle;
            lblBlockId.Text = "Block: " + block.Id;

            // Background image
            picBackground.Image?.Dispose();
            picBackground.Image = null;
            if (!string.IsNullOrEmpty(block.BackgroundImage) && !string.IsNullOrEmpty(_tempDir))
            {
                string imgPath = Path.Combine(_tempDir, block.BackgroundImage.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(imgPath))
                    try { picBackground.Image = Image.FromFile(imgPath); } catch { }
            }
            picBackground.Visible = picBackground.Image != null;

            // Narrative text
            rtbNarrative.Text = block.Text;

            // HUD
            RefreshHUD();

            // Decisions
            RefreshDecisions();

            if (block.IsFinal)
            {
                flowDecisions.Controls.Clear();
                var lbl = new Label
                {
                    Text = "─── THE END ───",
                    Font = new Font("Segoe UI", 14f, FontStyle.Bold | FontStyle.Italic),
                    ForeColor = Color.FromArgb(180, 120, 255),
                    AutoSize = true,
                    Padding = new Padding(10)
                };
                flowDecisions.Controls.Add(lbl);
            }
        }

        private void RefreshHUD()
        {
            var state = _engine.State;
            var props = state.GetVisibleHudProps();

            bool foundHealth = false, foundStamina = false, foundLevel = false;
            panelExtraHud.Controls.Clear();
            int extraY = 2;

            foreach (var prop in props)
            {
                double val = state.Properties.TryGetValue(prop.Key, out double v) ? v : prop.Initial;
                double pct = prop.Max > prop.Min ? (val - prop.Min) / (prop.Max - prop.Min) * 100 : 0;
                int ipct = Math.Max(0, Math.Min(100, (int)Math.Round(pct)));

                string ht = prop.HudType?.ToLower() ?? "";
                bool isHealth  = ht == "health"  || prop.Key.Contains("life")   || prop.Key.Contains("health");
                bool isStamina = ht == "stamina" || prop.Key.Contains("energy") || prop.Key.Contains("stamina");
                bool isLevel   = ht == "level"   || prop.Key.Contains("level");

                if (isHealth && !foundHealth)
                {
                    lblHealth.Text = $"❤ {prop.HudLabel}  {(int)val} / {(int)prop.Max}";
                    pbHealth.Value = ipct;
                    foundHealth = true;
                }
                else if (isStamina && !foundStamina)
                {
                    lblStamina.Text = $"⚡ {prop.HudLabel}  {(int)val} / {(int)prop.Max}";
                    pbStamina.Value = ipct;
                    foundStamina = true;
                }
                else if (isLevel && !foundLevel)
                {
                    lblLevel.Text = $"⭐ {prop.HudLabel}  {(int)val} / {(int)prop.Max}";
                    pbLevel.Value = ipct;
                    foundLevel = true;
                }
                else
                {
                    // Extra props as labels
                    var lbl = new Label
                    {
                        Text = $"{prop.HudLabel}: {(int)val}",
                        ForeColor = Color.FromArgb(220, 210, 240),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 8.5f),
                        Location = new Point(4, extraY)
                    };
                    panelExtraHud.Controls.Add(lbl);
                    extraY += 20;
                }
            }
        }

        private void RefreshDecisions()
        {
            flowDecisions.Controls.Clear();
            if (_engine.CurrentBlock == null || _engine.IsCurrentBlockFinal()) return;

            var decisions = _engine.GetAvailableDecisions();
            int btnWidth = Math.Max(320, flowDecisions.Width - 20);

            foreach (var dec in decisions)
            {
                var btn = new Button
                {
                    Text = "  " + dec.Text,
                    Width = btnWidth,
                    Height = 50,
                    BackColor = Color.FromArgb(45, 35, 60),
                    ForeColor = Color.FromArgb(230, 220, 245),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11f),
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(2, 4, 2, 4),
                    Tag = dec
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(180, 120, 255);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 55, 95);

                if (!string.IsNullOrEmpty(dec.Icon) && !string.IsNullOrEmpty(_tempDir))
                {
                    string iconPath = Path.Combine(_tempDir, dec.Icon.Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(iconPath))
                        try
                        {
                            btn.Image = new Bitmap(iconPath).GetThumbnailImage(28, 28, null, IntPtr.Zero);
                            btn.ImageAlign = ContentAlignment.MiddleLeft;
                            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                        }
                        catch { }
                }

                btn.Click += (s, ev) =>
                {
                    if (s is Button b && b.Tag is DecisionDefinition d)
                    {
                        string? redirect = _engine.MakeDecision(d);
                        _engine.NavigateTo(redirect ?? d.TargetBlock);
                        RefreshDisplay();
                    }
                };
                flowDecisions.Controls.Add(btn);
            }

            if (decisions.Count == 0)
                flowDecisions.Controls.Add(new Label
                {
                    Text = "(No available choices)",
                    ForeColor = Color.FromArgb(150, 130, 170),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                    Padding = new Padding(8)
                });
        }

        private void PlayerForm_Resize(object sender, EventArgs e)
        {
            RefreshDecisions();
        }
    }
}
