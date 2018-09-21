using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("BOP", "BOP (https://developers.google.com/optimization/reference/bop/bop_solver/) can be used out of the box.")]
  [StorableClass]
  public class BopSolver : Solver {

    public BopSolver() {
      Parameters.Remove(programmingTypeParam);
    }

    public override OptimizationProblemType OptimizationProblemType => OptimizationProblemType.BOP_INTEGER_PROGRAMMING;
  }
}