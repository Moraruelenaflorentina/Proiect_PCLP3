using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Story.Model;
using Story.Persistence;

namespace Story.Editor.WinForms
{
    public partial class EditorForm : Form
    {
        private StoryDefinition _story = new();
        private string _workDir = "";
        private bool _isDirty = false;

        public EditorForm()
        {
            InitializeComponent();
            NewStory();
        }

        //Menu handlers

        private void menuNew_Click(object sender, EventArgs e) => NewStory();

        private void menuOpen_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "Story archives (*.zip)|*.zip|All files (*.*)|*.*" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var (story, tempDir) = StoryRepository.LoadFromZip(dlg.FileName);
                _story = story;
                _workDir = tempDir;
                _isDirty = false;
                RefreshTree();
                rtbLog.Text = "Opened: " + dlg.FileName;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            var errs = StoryRepository.ValidateStory(_story);
            if (errs.Count > 0)
            {
                var msg = "Validation errors:\n" + string.Join("\n", errs) + "\n\nSave anyway?";
                if (MessageBox.Show(msg, "Validation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            using var dlg = new SaveFileDialog { Filter = "Story archives (*.zip)|*.zip", Title = "Save Story" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            StoryRepository.SaveToZipWithImages(_story, dlg.FileName, _workDir);
            _isDirty = false;
            MessageBox.Show("Story saved!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuValidate_Click(object sender, EventArgs e)
        {
            var errs = StoryRepository.ValidateStory(_story);
            rtbLog.Clear();
            if (errs.Count == 0) rtbLog.AppendText("✓ No errors found.\n");
            else foreach (var err in errs) rtbLog.AppendText("✗ " + err + "\n");
        }

        private void menuExit_Click(object sender, EventArgs e) => Close();

        //Story operations

        private void NewStory()
        {
            _story = new StoryDefinition
            {
                Title = "New Story",
                StartBlock = "intro.start",
                Properties = new List<StatePropertyDefinition>
                {
                    new() { Key = "player.life",   HudLabel = "Health",  Min = 0, Max = 100, Initial = 100, VisibleInHud = true, HudOrder = 1, HudType = "health",  OnMinBlock = "" },
                    new() { Key = "player.energy", HudLabel = "Stamina", Min = 0, Max = 100, Initial = 80,  VisibleInHud = true, HudOrder = 2, HudType = "stamina" },
                    new() { Key = "player.level",  HudLabel = "Level",   Min = 0, Max = 10,  Initial = 1,   VisibleInHud = true, HudOrder = 3, HudType = "level"   }
                },
                Blocks = new List<StoryBlock>
                {
                    new() { Id = "intro.start", Text = "Your adventure begins...", Decisions = new() }
                }
            };
            _workDir = Path.Combine(Path.GetTempPath(), "StoryEditor_" + Guid.NewGuid());
            Directory.CreateDirectory(_workDir);
            Directory.CreateDirectory(Path.Combine(_workDir, "images"));
            _isDirty = false;
            RefreshTree();
            ClearEditPanel();
            rtbLog.Text = "New story created.";
        }

        // Tree
        private void RefreshTree()
        {
            treeStory.Nodes.Clear();

            var root = new TreeNode("📖 " + _story.Title) { Tag = "story", Name = "root" };

            var propsNode = new TreeNode("⚙ Properties") { Tag = "props" };
            foreach (var p in _story.Properties)
                propsNode.Nodes.Add(new TreeNode(p.Key) { Tag = p });
            propsNode.Nodes.Add(new TreeNode("＋ Add Property") { Tag = "addprop", ForeColor = Color.FromArgb(100, 200, 100) });

            var blocksNode = new TreeNode("📦 Blocks") { Tag = "blocks" };
            foreach (var b in _story.Blocks)
                blocksNode.Nodes.Add(new TreeNode((b.IsFinal ? "🏁 " : "▶ ") + b.Id) { Tag = b });
            blocksNode.Nodes.Add(new TreeNode("＋ Add Block") { Tag = "addblock", ForeColor = Color.FromArgb(100, 200, 100) });

            root.Nodes.Add(propsNode);
            root.Nodes.Add(blocksNode);
            treeStory.Nodes.Add(root);
            root.ExpandAll();
        }

        private void treeStory_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = e.Node?.Tag;
            ClearEditPanel();

            if (tag is string s)
            {
                if (s == "story") ShowStoryMeta();
                if (s == "addprop") { AddProperty(); return; }
                if (s == "addblock") { AddBlock(); return; }
            }
            else if (tag is StatePropertyDefinition prop) ShowPropertyEditor(prop);
            else if (tag is StoryBlock block) ShowBlockEditor(block);
        }

        private void ClearEditPanel()
        {
            panelEdit.Controls.Clear();
            picPreview.Image = null;
        }

        //Story meta
        private void ShowStoryMeta()
        {
            var pnl = MakeScrollPanel();

            AddHeader(pnl, "Story Metadata");

            var (lTitle, tbTitle) = AddLabeledField(pnl, "Title:", _story.Title, 400);
            var (lStart, tbStart) = AddLabeledField(pnl, "Start Block ID:", _story.StartBlock, 300);

            var btnApply = MakeButton("💾 Apply");
            btnApply.Click += (s, e) =>
            {
                _story.Title = tbTitle.Text;
                _story.StartBlock = tbStart.Text;
                _isDirty = true;
                RefreshTree();
            };
            pnl.Controls.Add(btnApply);

            panelEdit.Controls.Add(pnl);
        }

        //Property editor
        private void AddProperty()
        {
            var p = new StatePropertyDefinition { Key = "new.property", Min = 0, Max = 100, Initial = 50, VisibleInHud = true };
            _story.Properties.Add(p);
            _isDirty = true;
            RefreshTree();
            ShowPropertyEditor(p);
        }

        private void ShowPropertyEditor(StatePropertyDefinition prop)
        {
            var pnl = MakeScrollPanel();
            AddHeader(pnl, "Property: " + prop.Key);

            var (_, tbKey) = AddLabeledField(pnl, "Key:", prop.Key, 300);
            var (_, tbLabel) = AddLabeledField(pnl, "HUD Label:", prop.HudLabel, 200);
            var (_, tbMin) = AddLabeledField(pnl, "Min:", prop.Min.ToString(), 80);
            var (_, tbMax) = AddLabeledField(pnl, "Max:", prop.Max.ToString(), 80);
            var (_, tbInitial) = AddLabeledField(pnl, "Initial:", prop.Initial.ToString(), 80);
            var (_, tbHudType) = AddLabeledField(pnl, "HUD Type (health / stamina / level / blank):", prop.HudType ?? "", 120);
            var cbVisible = AddCheckField(pnl, "Visible in HUD", prop.VisibleInHud);
            var (_, tbOrder) = AddLabeledField(pnl, "HUD Order:", prop.HudOrder.ToString(), 60);
            var (_, tbOnMin) = AddLabeledField(pnl, "On Min → Block:", prop.OnMinBlock ?? "", 200);
            var (_, tbOnMax) = AddLabeledField(pnl, "On Max → Block:", prop.OnMaxBlock ?? "", 200);

            AddSpacer(pnl);

            var btnApply = MakeButton("💾 Apply");
            btnApply.Click += (s, e) =>
            {
                prop.Key = tbKey.Text;
                prop.HudLabel = tbLabel.Text;
                prop.Min = TryDouble(tbMin.Text, prop.Min);
                prop.Max = TryDouble(tbMax.Text, prop.Max);
                prop.Initial = TryDouble(tbInitial.Text, prop.Initial);
                prop.HudType = string.IsNullOrWhiteSpace(tbHudType.Text) ? null : tbHudType.Text.Trim();
                prop.VisibleInHud = cbVisible.Checked;
                prop.HudOrder = TryInt(tbOrder.Text, prop.HudOrder);
                prop.OnMinBlock = NullIfEmpty(tbOnMin.Text);
                prop.OnMaxBlock = NullIfEmpty(tbOnMax.Text);
                _isDirty = true;
                RefreshTree();
                rtbLog.Text = "Property saved: " + prop.Key;
            };

            var btnDelete = MakeButton("🗑 Delete", Color.FromArgb(110, 35, 35));
            btnDelete.Click += (s, e) =>
            {
                if (MessageBox.Show("Delete property '" + prop.Key + "'?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _story.Properties.Remove(prop);
                    _isDirty = true;
                    ClearEditPanel();
                    RefreshTree();
                }
            };

            var row = MakeRow(btnApply, btnDelete);
            pnl.Controls.Add(row);
            panelEdit.Controls.Add(pnl);
        }

        //Block editor
        private void AddBlock()
        {
            var b = new StoryBlock { Id = "new.block", Text = "Block text here.", Decisions = new() };
            _story.Blocks.Add(b);
            _isDirty = true;
            RefreshTree();
            ShowBlockEditor(b);
        }

        private void ShowBlockEditor(StoryBlock block)
        {
            var pnl = MakeScrollPanel();
            AddHeader(pnl, "Block: " + block.Id);

            var (_, tbId) = AddLabeledField(pnl, "Block ID:", block.Id, 300);

            AddSmallLabel(pnl, "Narrative Text:");
            var rtbText = new RichTextBox
            {
                Text = block.Text,
                Width = 540,
                Height = 110,
                BackColor = Color.FromArgb(32, 26, 46),
                ForeColor = Color.FromArgb(230, 220, 245),
                Font = new Font("Georgia", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 6)
            };
            pnl.Controls.Add(rtbText);

            var cbFinal = AddCheckField(pnl, "Is Final Block (end of story)", block.IsFinal);

            //Background image
            AddHeader(pnl, "Background Image");
            var lblImg = new Label
            {
                Text = string.IsNullOrEmpty(block.BackgroundImage) ? "(none selected)" : block.BackgroundImage,
                ForeColor = Color.FromArgb(150, 140, 190),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 4)
            };
            pnl.Controls.Add(lblImg);

            var btnImg = MakeButton("📷 Select Image...");
            btnImg.Click += (s, e) =>
            {
                using var fd = new OpenFileDialog { Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp" };
                if (fd.ShowDialog() != DialogResult.OK) return;
                string dest = Path.Combine(_workDir, "images", Path.GetFileName(fd.FileName));
                Directory.CreateDirectory(Path.Combine(_workDir, "images"));
                File.Copy(fd.FileName, dest, true);
                block.BackgroundImage = "images/" + Path.GetFileName(fd.FileName);
                lblImg.Text = block.BackgroundImage;
                _isDirty = true;
                try { picPreview.Image?.Dispose(); picPreview.Image = Image.FromFile(dest); } catch { }
            };
            pnl.Controls.Add(btnImg);

            if (!string.IsNullOrEmpty(block.BackgroundImage))
            {
                string ip = Path.Combine(_workDir, block.BackgroundImage.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(ip)) try { picPreview.Image = Image.FromFile(ip); } catch { }
            }

            //block button
            AddSpacer(pnl);
            var btnApply = MakeButton("💾 Apply Block");
            btnApply.Click += (s, e) =>
            {
                block.Id = tbId.Text;
                block.Text = rtbText.Text;
                block.IsFinal = cbFinal.Checked;
                _isDirty = true;
                RefreshTree();
            };

            //decisions
            AddHeader(pnl, "Decisions");
            var grid = BuildDecisionsGrid(block, pnl);
            pnl.Controls.Add(grid);

            var btnAddDec = MakeButton("＋ Add Decision", Color.FromArgb(35, 70, 35));
            btnAddDec.Click += (s, e) =>
            {
                var dec = new DecisionDefinition { Text = "New choice", TargetBlock = "" };
                block.Decisions.Add(dec);
                _isDirty = true;
                ShowDecisionDialog(dec);
                ClearEditPanel();
                ShowBlockEditor(block);
            };

            var btnDelete = MakeButton("🗑 Delete Block", Color.FromArgb(110, 35, 35));
            btnDelete.Click += (s, e) =>
            {
                if (MessageBox.Show("Delete block '" + block.Id + "'?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _story.Blocks.Remove(block);
                    _isDirty = true;
                    ClearEditPanel();
                    RefreshTree();
                }
            };

            var row1 = MakeRow(btnApply);
            var row2 = MakeRow(btnAddDec, btnDelete);
            pnl.Controls.Add(row1);
            pnl.Controls.Add(row2);
            panelEdit.Controls.Add(pnl);
        }

        private DataGridView BuildDecisionsGrid(StoryBlock block, FlowLayoutPanel pnl)
        {
            var grid = new DataGridView
            {
                Width = 560,
                Height = 160,
                BackgroundColor = Color.FromArgb(26, 20, 38),
                GridColor = Color.FromArgb(55, 45, 75),
                ForeColor = Color.FromArgb(220, 210, 240),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                { BackColor = Color.FromArgb(38, 30, 54), ForeColor = Color.FromArgb(220, 210, 240) },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(26, 20, 38),
                    ForeColor = Color.FromArgb(220, 210, 240),
                    SelectionBackColor = Color.FromArgb(65, 50, 90)
                },
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 4, 0, 8)
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Text", Width = 210 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Target Block", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Condition", Width = 75 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Effects", Width = 75 });

            foreach (var dec in block.Decisions)
            {
                int r = grid.Rows.Add();
                grid.Rows[r].Cells[0].Value = dec.Text;
                grid.Rows[r].Cells[1].Value = dec.TargetBlock;
                grid.Rows[r].Cells[2].Value = dec.Condition != null ? "Yes" : "—";
                grid.Rows[r].Cells[3].Value = dec.Effects.Count > 0 ? dec.Effects.Count + "x" : "—";
                grid.Rows[r].Tag = dec;
            }

            grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (grid.Rows[e.RowIndex].Tag is DecisionDefinition dec)
                {
                    ShowDecisionDialog(dec);
                    ClearEditPanel();
                    ShowBlockEditor(block);
                }
            };

            return grid;
        }

        //Decision dialog
        private void ShowDecisionDialog(DecisionDefinition dec)
        {
            using var dlg = new Form
            {
                Text = "Edit Decision",
                Size = new Size(680, 640),
                BackColor = Color.FromArgb(22, 18, 32),
                ForeColor = Color.FromArgb(230, 220, 245),
                Font = new Font("Segoe UI", 9.5f),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(16),
                BackColor = Color.FromArgb(22, 18, 32),
                WrapContents = false
            };

            //decision text
            flow.Controls.Add(SectionLabel("Decision Text"));
            var tbText = new TextBox { Text = dec.Text, Width = 620, BackColor = Color.FromArgb(32, 26, 46), ForeColor = Color.FromArgb(230, 220, 245), BorderStyle = BorderStyle.FixedSingle };
            flow.Controls.Add(tbText);

            //target block
            flow.Controls.Add(SectionLabel("Target Block ID"));
            var tbTarget = new TextBox { Text = dec.TargetBlock, Width = 320, BackColor = Color.FromArgb(32, 26, 46), ForeColor = Color.FromArgb(230, 220, 245), BorderStyle = BorderStyle.FixedSingle };
            flow.Controls.Add(tbTarget);

            //icon
            flow.Controls.Add(SectionLabel("Icon Image"));
            var lblIcon = new Label { Text = dec.Icon ?? "(none)", ForeColor = Color.FromArgb(140, 130, 180), AutoSize = true };
            flow.Controls.Add(lblIcon);
            var btnIcon = new Button { Text = "🖼 Select Icon...", Width = 160, Height = 30, BackColor = Color.FromArgb(45, 35, 60), ForeColor = Color.FromArgb(230, 220, 245), FlatStyle = FlatStyle.Flat };
            btnIcon.FlatAppearance.BorderColor = Color.FromArgb(180, 120, 255);
            btnIcon.Click += (s, e) =>
            {
                using var fd = new OpenFileDialog { Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp" };
                if (fd.ShowDialog() != DialogResult.OK) return;
                string dest = Path.Combine(_workDir, "images", Path.GetFileName(fd.FileName));
                Directory.CreateDirectory(Path.Combine(_workDir, "images"));
                File.Copy(fd.FileName, dest, true);
                dec.Icon = "images/" + Path.GetFileName(fd.FileName);
                lblIcon.Text = dec.Icon;
            };
            flow.Controls.Add(btnIcon);

            //effects grid
            flow.Controls.Add(SectionLabel("Effects  (double-click row to edit; last row = new)"));
            var propKeys = _story.Properties.Select(p => p.Key).ToArray();
            var effectsGrid = new DataGridView
            {
                Width = 620,
                Height = 120,
                BackgroundColor = Color.FromArgb(20, 16, 30),
                DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(26, 20, 38), ForeColor = Color.FromArgb(220, 210, 240) },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(38, 30, 54), ForeColor = Color.FromArgb(220, 210, 240) },
                AllowUserToAddRows = true,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                Margin = new Padding(0, 0, 0, 6)
            };
            var colProp = new DataGridViewComboBoxColumn { HeaderText = "Property", Width = 240, DataSource = propKeys.ToList() };
            var colType = new DataGridViewComboBoxColumn { HeaderText = "Type", Width = 80, DataSource = new List<string> { "ADD", "SET" } };
            var colVal = new DataGridViewTextBoxColumn { HeaderText = "Value", Width = 100 };
            effectsGrid.Columns.AddRange(colProp, colType, colVal);
            foreach (var ef in dec.Effects)
            {
                int r = effectsGrid.Rows.Add();
                effectsGrid.Rows[r].Cells[0].Value = ef.Property;
                effectsGrid.Rows[r].Cells[1].Value = ef.Type;
                effectsGrid.Rows[r].Cells[2].Value = ef.Value.ToString();
            }
            flow.Controls.Add(effectsGrid);

            //condition JSON
            flow.Controls.Add(SectionLabel("Condition (JSON AST - leave blank for none)"));
            flow.Controls.Add(new Label
            {
                Text = "Example: {\"type\":\"COMPARISON\",\"property\":\"inventory.money\",\"operator\":\">=\",\"value\":5}",
                ForeColor = Color.FromArgb(110, 100, 150),
                AutoSize = true,
                Font = new Font("Consolas", 7.5f),
                Margin = new Padding(0, 0, 0, 2)
            });
            string condJson = dec.Condition == null ? "" :
                System.Text.Json.JsonSerializer.Serialize(dec.Condition,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            var rtbCond = new RichTextBox
            {
                Text = condJson,
                Width = 620,
                Height = 100,
                BackColor = Color.FromArgb(16, 12, 24),
                ForeColor = Color.FromArgb(160, 255, 140),
                Font = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 8)
            };
            flow.Controls.Add(rtbCond);

            //Save button
            var btnSave = new Button
            {
                Text = "Save Decision",
                Width = 180,
                Height = 36,
                BackColor = Color.FromArgb(35, 80, 35),
                ForeColor = Color.FromArgb(230, 220, 245),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(80, 200, 80);
            btnSave.Click += (s, e) =>
            {
                dec.Text = tbText.Text;
                dec.TargetBlock = tbTarget.Text;

                dec.Effects.Clear();
                foreach (DataGridViewRow row in effectsGrid.Rows)
                {
                    if (row.IsNewRow) continue;
                    string? prop = row.Cells[0].Value?.ToString();
                    string? type = row.Cells[1].Value?.ToString();
                    if (string.IsNullOrEmpty(prop)) continue;
                    dec.Effects.Add(new EffectDefinition
                    {
                        Property = prop,
                        Type = type ?? "ADD",
                        Value = TryDouble(row.Cells[2].Value?.ToString() ?? "0", 0)
                    });
                }

                if (!string.IsNullOrWhiteSpace(rtbCond.Text))
                {
                    try { dec.Condition = System.Text.Json.JsonSerializer.Deserialize<ConditionDefinition>(rtbCond.Text); }
                    catch { MessageBox.Show("Invalid condition JSON.", "Error"); return; }
                }
                else dec.Condition = null;

                _isDirty = true;
                dlg.Close();
            };
            flow.Controls.Add(btnSave);

            dlg.Controls.Add(flow);
            dlg.ShowDialog(this);
        }

        //UI helpers
        private FlowLayoutPanel MakeScrollPanel() => new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false,
            BackColor = Color.FromArgb(22, 18, 32),
            Padding = new Padding(18, 14, 18, 18)
        };

        private void AddHeader(FlowLayoutPanel pnl, string text) =>
            pnl.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 120, 255),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 4)
            });

        private void AddSmallLabel(FlowLayoutPanel pnl, string text) =>
            pnl.Controls.Add(new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = Color.FromArgb(170, 160, 200),
                Margin = new Padding(0, 6, 0, 2)
            });

        private void AddSpacer(FlowLayoutPanel pnl) =>
            pnl.Controls.Add(new Label { Height = 6, Width = 10, AutoSize = false });

        private Label SectionLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(180, 120, 255),
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 3)
        };

        private (Label lbl, TextBox tb) AddLabeledField(FlowLayoutPanel pnl, string label, string value, int width)
        {
            var lbl = new Label { Text = label, AutoSize = true, ForeColor = Color.FromArgb(170, 160, 200), Margin = new Padding(0, 6, 0, 1) };
            var tb = new TextBox { Text = value, Width = width, BackColor = Color.FromArgb(32, 26, 46), ForeColor = Color.FromArgb(230, 220, 245), BorderStyle = BorderStyle.FixedSingle };
            pnl.Controls.Add(lbl);
            pnl.Controls.Add(tb);
            return (lbl, tb);
        }

        private CheckBox AddCheckField(FlowLayoutPanel pnl, string text, bool value)
        {
            var cb = new CheckBox { Text = text, Checked = value, AutoSize = true, ForeColor = Color.FromArgb(220, 210, 240), Margin = new Padding(0, 6, 0, 4) };
            pnl.Controls.Add(cb);
            return cb;
        }

        private Button MakeButton(string text, Color? bg = null)
        {
            var btn = new Button
            {
                Text = text,
                Width = 180,
                Height = 34,
                BackColor = bg ?? Color.FromArgb(48, 38, 65),
                ForeColor = Color.FromArgb(230, 220, 245),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 4, 8, 4)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(180, 120, 255);
            return btn;
        }

        private Panel MakeRow(params Button[] buttons)
        {
            var row = new Panel { Height = 44, Width = 560, BackColor = Color.Transparent, Margin = new Padding(0, 4, 0, 0) };
            int x = 0;
            foreach (var b in buttons) { b.Location = new Point(x, 4); row.Controls.Add(b); x += b.Width + 8; }
            return row;
        }

        //Utility
        private static double TryDouble(string s, double fallback) =>
            double.TryParse(s, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : fallback;

        private static int TryInt(string s, int fallback) =>
            int.TryParse(s, out int v) ? v : fallback;

        private static string? NullIfEmpty(string s) =>
            string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private void menuItemTools_Click(object sender, EventArgs e)
        {

        }

        private void menuItemFile_Click(object sender, EventArgs e)
        {

        }
    }
}
