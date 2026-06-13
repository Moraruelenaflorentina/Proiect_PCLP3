using System;
using System.Collections.Generic;
using Story.Model;

namespace Story.Engine
{
    public static class ConditionEvaluator
    {
        public static bool Evaluate(ConditionDefinition? condition, Dictionary<string, double> state)
        {
            if (condition == null) return true;

            switch (condition.Type)
            {
                case "COMPARISON":
                    if (!state.TryGetValue(condition.Property!, out double val)) return false;
                    double cmpVal = condition.Value ?? 0;
                    return condition.Operator switch
                    {
                        "<"  => val < cmpVal,
                        "<=" => val <= cmpVal,
                        ">"  => val > cmpVal,
                        ">=" => val >= cmpVal,
                        "==" => Math.Abs(val - cmpVal) < 1e-9,
                        "!=" => Math.Abs(val - cmpVal) >= 1e-9,
                        _ => false
                    };

                case "AND":
                    foreach (var c in condition.Conditions ?? new())
                        if (!Evaluate(c, state)) return false;
                    return true;

                case "OR":
                    foreach (var c in condition.Conditions ?? new())
                        if (Evaluate(c, state)) return true;
                    return false;

                default:
                    return true;
            }
        }
    }
}
