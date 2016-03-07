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
  [Item("Ucb Policy", "Ucb with parameter c to balance between exploitation and exploration")]
  public class Ucb : PolicyBase {
    private class ActionStatistics : IActionStatistics {
      public double SumQuality { get; set; }
      public double AverageQuality { get { return SumQuality / Tries; } }
      public int Tries { get; set; }
      public bool Done { get; set; }
    }
    private List<int> buf = new List<int>();

    public IFixedValueParameter<DoubleValue> CParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["C"]; }
    }

    public double C {
      get { return CParameter.Value.Value; }
      set { CParameter.Value.Value = value; }
    }

    protected Ucb(Ucb original, Cloner cloner)
      : base(original, cloner) {
    }
    public Ucb()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>("C", "Parameter to balance between exploration and exploitation 0 <= c < 100", new DoubleValue(Math.Sqrt(2))));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Ucb(this, cloner);
    }

    public override int Select(IEnumerable<IActionStatistics> actions, IRandom random) {
      return Select(actions, random, C, buf);
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
      // determine total tries of still active actions
      int totalTries = 0;
      buf.Clear();
      int aIdx = -1;
      foreach (var a in actions) {
        ++aIdx;
        if (a.Done) continue;
        if (a.Tries == 0) buf.Add(aIdx);
        else totalTries += a.Tries;
      }
      // if there are unvisited actions select a random action
      if (buf.Any()) {
        return buf[rand.Next(buf.Count)];
      }
      Contract.Assert(totalTries > 0);
      double logTotalTries = Math.Log(totalTries);
      var bestQ = double.NegativeInfinity;
      aIdx = -1;
      foreach (var a in actions) {
        ++aIdx;
        if (a.Done) continue;
        var actionQ = a.AverageQuality + c * Math.Sqrt(logTotalTries / a.Tries);
        if (actionQ > bestQ) {
          buf.Clear();
          buf.Add(aIdx);
          bestQ = actionQ;
        } else if (actionQ >= bestQ) {
          buf.Add(aIdx);
        }
      }
      return buf[rand.Next(buf.Count)];
    }

  }
}
