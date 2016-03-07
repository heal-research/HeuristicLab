using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies {
  [Item("EpsilonGreedy", "Epsilon greedy policy with parameter eps to balance between exploitation and exploration")]
  internal class EpsilonGreedy : PolicyBase {
    private class ActionStatistics : IActionStatistics {
      public double SumQuality { get; set; }
      public double AverageQuality { get { return SumQuality / Tries; } }
      public int Tries { get; set; }
      public bool Done { get; set; }
    }
    private List<int> buf = new List<int>();

    public IFixedValueParameter<DoubleValue> EpsParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["Eps"]; }
    }

    public double Eps {
      get { return EpsParameter.Value.Value; }
      set { EpsParameter.Value.Value = value; }
    }

    private EpsilonGreedy(EpsilonGreedy original, Cloner cloner)
      : base(original, cloner) {
    }
    public EpsilonGreedy()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>("Eps", "Rate of random selection 0 (greedy) <= eps <= 1 (random)", new DoubleValue(0.1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EpsilonGreedy(this, cloner);
    }

    public override int Select(IEnumerable<IActionStatistics> actions, IRandom random) {
      return Select(actions, random, Eps, buf);
    }

    public override void Update(IActionStatistics action, double q) {
      var a = action as ActionStatistics;
      a.SumQuality += q;
      a.Tries++;
    }

    public override IActionStatistics CreateActionStatistics() {
      return new ActionStatistics();
    }

    private static int Select(IEnumerable<IActionStatistics> actions, IRandom rand, double c, IList<int> buf) {
      buf.Clear();
      if (rand.NextDouble() >= c) {
        // select best
        var bestQ = double.NegativeInfinity;
        int aIdx = -1;
        foreach (var a in actions) {
          ++aIdx;
          if (a.Done) continue;
          var actionQ = a.Tries > 0 ? a.AverageQuality : double.PositiveInfinity; // always try unvisited actions first
          if (actionQ > bestQ) {
            buf.Clear();
            buf.Add(aIdx);
            bestQ = actionQ;
          } else if (actionQ >= bestQ) {
            buf.Add(aIdx);
          }
        }
        return buf[rand.Next(buf.Count)];
      } else {
        // random selection
        int aIdx = -1;
        foreach (var a in actions) {
          ++aIdx;
          if (a.Done) continue;
          buf.Add(aIdx);
        }
        return buf[rand.Next(buf.Count)];
      }
    }
  }
}
