using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Story.Model
{
    public class StoryDefinition
    {
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("startBlock")] public string StartBlock { get; set; } = "";
        [JsonPropertyName("properties")] public List<StatePropertyDefinition> Properties { get; set; } = new();
        [JsonPropertyName("blocks")] public List<StoryBlock> Blocks { get; set; } = new();
    }

    public class StatePropertyDefinition
    {
        [JsonPropertyName("key")] public string Key { get; set; } = "";
        [JsonPropertyName("hudLabel")] public string HudLabel { get; set; } = "";
        [JsonPropertyName("min")] public double Min { get; set; } = 0;
        [JsonPropertyName("max")] public double Max { get; set; } = 100;
        [JsonPropertyName("initial")] public double Initial { get; set; } = 0;
        [JsonPropertyName("visibleInHud")] public bool VisibleInHud { get; set; } = false;
        [JsonPropertyName("hudOrder")] public int HudOrder { get; set; } = 99;
        [JsonPropertyName("hudType")] public string? HudType { get; set; }
        [JsonPropertyName("onMinBlock")] public string? OnMinBlock { get; set; }
        [JsonPropertyName("onMaxBlock")] public string? OnMaxBlock { get; set; }
    }

    public class StoryBlock
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("text")] public string Text { get; set; } = "";
        [JsonPropertyName("isFinal")] public bool IsFinal { get; set; } = false;
        [JsonPropertyName("backgroundImage")] public string? BackgroundImage { get; set; }
        [JsonPropertyName("decisions")] public List<DecisionDefinition> Decisions { get; set; } = new();
    }

    public class DecisionDefinition
    {
        [JsonPropertyName("text")] public string Text { get; set; } = "";
        [JsonPropertyName("targetBlock")] public string TargetBlock { get; set; } = "";
        [JsonPropertyName("icon")] public string? Icon { get; set; }
        [JsonPropertyName("condition")] public ConditionDefinition? Condition { get; set; }
        [JsonPropertyName("effects")] public List<EffectDefinition> Effects { get; set; } = new();
    }

    public class EffectDefinition
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "ADD";
        [JsonPropertyName("property")] public string Property { get; set; } = "";
        [JsonPropertyName("value")] public double Value { get; set; } = 0;
    }

    public class ConditionDefinition
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "";
        [JsonPropertyName("property")] public string? Property { get; set; }
        [JsonPropertyName("operator")] public string? Operator { get; set; }
        [JsonPropertyName("value")] public double? Value { get; set; }
        [JsonPropertyName("conditions")] public List<ConditionDefinition>? Conditions { get; set; }
    }
}
