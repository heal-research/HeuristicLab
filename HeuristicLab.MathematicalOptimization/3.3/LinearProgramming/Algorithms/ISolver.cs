using HeuristicLab.Core;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  public interface ISolver : IParameterizedNamedItem {
    OptimizationProblemType OptimizationProblemType { get; }
    string LibraryName { get; set; }
    LinearProgrammingType LinearProgrammingType { get; set; }
  }
}
