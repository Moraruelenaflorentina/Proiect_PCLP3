using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Story.Editor.WinForms;
using Story.Engine;
using Story.Model;
using Story.Persistence;
using Story.Player.WinForms.UI;

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

            canvas.OnChoiceClicked = HandleChoice;
            canvas.OnRestartClicked = () => menuRestart_Click(this, EventArgs.Empty);
            canvas.StoryTitle = "StoryTeller";
        }

        private void menuOpen_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title  = "Deschide povestea (.zip)",
                Filter = "Arhive poveste (*.zip)|*.zip|Toate fișierele (*.*)|*.*"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var (story, tempDir) = StoryRepository.LoadFromZip(dlg.FileName);
                _tempDir = tempDir;
                _lastZipPath = dlg.FileName;
                canvas.TempImageDir = tempDir;
                canvas.StoryTitle = story.Title;
                _engine.LoadStory(story);
                RefreshScene();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la încărcare:\n" + ex.Message, "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuRestart_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_lastZipPath)) return;
            try
            {
                var (story, tempDir) = StoryRepository.LoadFromZip(_lastZipPath);
                _tempDir = tempDir;
                canvas.TempImageDir = tempDir;
                canvas.StoryTitle = story.Title;
                _engine.LoadStory(story);
                RefreshScene();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reluarea a eșuat: " + ex.Message, "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuSaveState_Click(object? sender, EventArgs e)
        {
            if (_engine.CurrentBlock == null) return;
            using var dlg = new SaveFileDialog
            {
                Filter = "Stare poveste (*.json)|*.json",
                Title  = "Salvează starea"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var data = new
            {
                Block = _engine.CurrentBlock.Id,
                State = _engine.State.Serialize()
            };
            File.WriteAllText(dlg.FileName, System.Text.Json.JsonSerializer.Serialize(data));
            MessageBox.Show("Starea a fost salvată.", "Salvat",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuLoadState_Click(object? sender, EventArgs e)
        {
            if (_engine.CurrentBlock == null) return;
            using var dlg = new OpenFileDialog
            {
                Filter = "Stare poveste (*.json)|*.json",
                Title  = "Încarcă starea"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var json = File.ReadAllText(dlg.FileName);
                var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
                string blockId   = data.GetProperty("Block").GetString()!;
                string stateJson = data.GetProperty("State").GetString()!;
                _engine.State.Deserialize(stateJson);
                _engine.NavigateTo(blockId);
                RefreshScene();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la încărcarea stării:\n" + ex.Message, "Eroare",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuExit_Click(object? sender, EventArgs e) => Close();

        private void createStoryToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var editorForm = new EditorForm();
            editorForm.Show();
        }

        private void HandleChoice(DecisionDefinition d)
        {
            string? redirect = _engine.MakeDecision(d);
            _engine.NavigateTo(redirect ?? d.TargetBlock);
            RefreshScene();
        }

        private void RefreshScene()
        {
            var block = _engine.CurrentBlock;
            if (block == null) return;

            var decisions = block.IsFinal ? new System.Collections.Generic.List<DecisionDefinition>()
                                          : _engine.GetAvailableDecisions();

            canvas.SetScene(
                block,
                decisions,
                _engine.State.Properties,
                _engine.State.GetVisibleHudProps());
        }
    }
}
