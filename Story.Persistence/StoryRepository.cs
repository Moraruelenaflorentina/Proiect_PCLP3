using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Story.Model;

namespace Story.Persistence
{
    public static class StoryRepository
    {
        private static readonly JsonSerializerOptions _opts = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = null
        };

        public static (StoryDefinition story, string tempDir) LoadFromZip(string zipPath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "StoryEngine_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            ZipFile.ExtractToDirectory(zipPath, tempDir, true);
            string jsonPath = Path.Combine(tempDir, "story.json");
            string json = File.ReadAllText(jsonPath);
            var story = JsonSerializer.Deserialize<StoryDefinition>(json, _opts)
                ?? throw new InvalidDataException("Invalid story.json");
            return (story, tempDir);
        }

        public static void SaveToZip(StoryDefinition story, string zipPath, string? tempDir = null)
        {
            string workDir = tempDir ?? Path.Combine(Path.GetTempPath(), "StoryEngine_Save_" + Guid.NewGuid());
            Directory.CreateDirectory(workDir);
            Directory.CreateDirectory(Path.Combine(workDir, "images"));

            string json = JsonSerializer.Serialize(story, _opts);
            File.WriteAllText(Path.Combine(workDir, "story.json"), json);

            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(workDir, zipPath);
        }

        public static void SaveToZipWithImages(StoryDefinition story, string zipPath, string sourceDir)
        {
            string workDir = Path.Combine(Path.GetTempPath(), "StoryEngine_Save_" + Guid.NewGuid());
            Directory.CreateDirectory(workDir);
            Directory.CreateDirectory(Path.Combine(workDir, "images"));

            string imgSrc = Path.Combine(sourceDir, "images");
            if (Directory.Exists(imgSrc))
            {
                foreach (var f in Directory.GetFiles(imgSrc))
                    File.Copy(f, Path.Combine(workDir, "images", Path.GetFileName(f)), true);
            }

            string json = JsonSerializer.Serialize(story, _opts);
            File.WriteAllText(Path.Combine(workDir, "story.json"), json);

            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(workDir, zipPath);

            Directory.Delete(workDir, true);
        }

        public static List<string> ValidateStory(StoryDefinition story)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(story.Title)) errors.Add("Title is missing.");
            if (string.IsNullOrWhiteSpace(story.StartBlock)) errors.Add("StartBlock is missing.");

            var blockIds = new HashSet<string>();
            foreach (var b in story.Blocks)
            {
                if (!blockIds.Add(b.Id))
                    errors.Add($"Duplicate block id: {b.Id}");
            }

            if (!blockIds.Contains(story.StartBlock))
                errors.Add($"StartBlock '{story.StartBlock}' not found in blocks.");

            var propKeys = new HashSet<string>();
            foreach (var p in story.Properties)
            {
                if (!propKeys.Add(p.Key))
                    errors.Add($"Duplicate property: {p.Key}");
                if (p.Min > p.Max) errors.Add($"Property {p.Key}: min > max");
                if (p.Initial < p.Min || p.Initial > p.Max)
                    errors.Add($"Property {p.Key}: initial out of [min,max]");
                if (!string.IsNullOrEmpty(p.OnMinBlock) && !blockIds.Contains(p.OnMinBlock))
                    errors.Add($"Property {p.Key} onMinBlock '{p.OnMinBlock}' not found.");
                if (!string.IsNullOrEmpty(p.OnMaxBlock) && !blockIds.Contains(p.OnMaxBlock))
                    errors.Add($"Property {p.Key} onMaxBlock '{p.OnMaxBlock}' not found.");
            }

            foreach (var block in story.Blocks)
            {
                foreach (var d in block.Decisions)
                {
                    if (!blockIds.Contains(d.TargetBlock))
                        errors.Add($"Block '{block.Id}' decision '{d.Text}': targetBlock '{d.TargetBlock}' not found.");
                    foreach (var e in d.Effects)
                    {
                        if (!propKeys.Contains(e.Property))
                            errors.Add($"Block '{block.Id}' decision '{d.Text}': effect references unknown property '{e.Property}'.");
                    }
                }
            }

            return errors;
        }
    }
}
