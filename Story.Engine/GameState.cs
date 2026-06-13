using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Story.Model;

namespace Story.Engine
{
    public class GameState
    {
        public Dictionary<string, double> Properties { get; private set; } = new();
        private Dictionary<string, StatePropertyDefinition> _defs = new();

        public void Initialize(List<StatePropertyDefinition> defs)
        {
            _defs = defs.ToDictionary(d => d.Key);
            Properties.Clear();
            foreach (var d in defs)
                Properties[d.Key] = d.Initial;
        }

        public string? ApplyEffect(EffectDefinition effect)
        {
            if (!_defs.TryGetValue(effect.Property, out var def)) return null;
            double current = Properties.TryGetValue(effect.Property, out double v) ? v : def.Initial;
            double newVal = effect.Type == "SET" ? effect.Value : current + effect.Value;
            newVal = Math.Max(def.Min, Math.Min(def.Max, newVal));
            Properties[effect.Property] = newVal;

            if (Math.Abs(newVal - def.Min) < 1e-9 && !string.IsNullOrEmpty(def.OnMinBlock))
                return def.OnMinBlock;
            if (Math.Abs(newVal - def.Max) < 1e-9 && !string.IsNullOrEmpty(def.OnMaxBlock))
                return def.OnMaxBlock;
            return null;
        }

        public StatePropertyDefinition? GetDef(string key) =>
            _defs.TryGetValue(key, out var d) ? d : null;

        public List<StatePropertyDefinition> GetVisibleHudProps() =>
            _defs.Values.Where(d => d.VisibleInHud).OrderBy(d => d.HudOrder).ToList();

        public string Serialize() => JsonSerializer.Serialize(Properties);

        public void Deserialize(string json)
        {
            var loaded = JsonSerializer.Deserialize<Dictionary<string, double>>(json);
            if (loaded != null) Properties = loaded;
        }
    }
}
