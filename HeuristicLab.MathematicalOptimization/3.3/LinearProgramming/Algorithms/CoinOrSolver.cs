using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("Clp/Cbc", "Clp (https://projects.coin-or.org/Clp) and Cbc (https://projects.coin-or.org/Cbc) can be used out of the box.")]
  [StorableClass]
  public class CoinOrSolver : Solver {

    public override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.CLP_LINEAR_PROGRAMMING
        : OptimizationProblemType.CBC_MIXED_INTEGER_PROGRAMMING;
  }
}