using System;
using System.Collections.Generic;
using System.Linq;
using Story.Model;

namespace Story.Engine
{
    public class GameEngine
    {
        private StoryDefinition _story = new();
        private Dictionary<string, StoryBlock> _blockMap = new();
        public GameState State { get; } = new();
        public StoryBlock? CurrentBlock { get; private set; }

        public void LoadStory(StoryDefinition story)
        {
            _story = story;
            _blockMap = story.Blocks.ToDictionary(b => b.Id);
            State.Initialize(story.Properties);
            CurrentBlock = _blockMap.TryGetValue(story.StartBlock, out var sb) ? sb : null;
        }

        public string StoryTitle => _story.Title;

        public List<DecisionDefinition> GetAvailableDecisions()
        {
            if (CurrentBlock == null) return new();
            return CurrentBlock.Decisions
                .Where(d => ConditionEvaluator.Evaluate(d.Condition, State.Properties))
                .ToList();
        }

        // Returns redirect block id if triggered, else null
        public string? MakeDecision(DecisionDefinition decision)
        {
            string? redirect = null;
            foreach (var effect in decision.Effects)
            {
                var r = State.ApplyEffect(effect);
                if (r != null) redirect = r;
            }
            return redirect;
        }

        public bool NavigateTo(string blockId)
        {
            if (_blockMap.TryGetValue(blockId, out var block))
            {
                CurrentBlock = block;
                return true;
            }
            return false;
        }

        public bool IsCurrentBlockFinal() => CurrentBlock?.IsFinal ?? false;
    }
}
