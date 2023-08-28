using System;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Dynamic {
  [StorableType("9E7EF12C-6013-4B10-ABCB-3C9F9586B9DE")]
  public interface IDynamicProblemDefinition : IProblemDefinition, IProblem {
    int Seed { get; }
    bool SetSeedRandomly { get; }
    IEpochClock EpochClock { get; }
    ProblemUpdatePolicy ProblemUpdatePolicy { get; }
    event EventHandler<long> EpochChanged;
    void AnalyzeProblem(ResultCollection results, IRandom random);
  }
}
