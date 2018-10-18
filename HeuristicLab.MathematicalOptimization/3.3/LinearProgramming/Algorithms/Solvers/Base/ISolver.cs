using System.Threading;
using HeuristicLab.Core;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base {

  public interface ISolver : IParameterizedNamedItem {
    LinearProgrammingType LinearProgrammingType { get; set; }
    bool SupportsPause { get; }
    bool SupportsStop { get; }

    void Interrupt();

    void Reset();

    void Solve(LinearProgrammingAlgorithm algorithm);

    void Solve(LinearProgrammingAlgorithm algorithm, CancellationToken cancellationToken);
  }
}