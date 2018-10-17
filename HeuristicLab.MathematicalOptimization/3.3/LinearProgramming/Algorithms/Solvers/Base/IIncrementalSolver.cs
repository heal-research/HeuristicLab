using System;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base {

  public interface IIncrementalSolver : ISolver {
    bool Incrementality { get; set; }
    TimeSpan QualityUpdateInterval { get; set; }
  }
}