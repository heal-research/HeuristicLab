using HeuristicLab.Core;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("BOP", "BOP (https://developers.google.com/optimization/reference/bop/bop_solver/) can be used out of the box.")]
  [StorableClass]
  public class BopSolver : IncrementalSolver {

    public BopSolver() {
      Parameters.Remove(programmingTypeParam);
    }

    public override bool SupportsPause => true;

    public override bool SupportsStop => true;

    protected override OptimizationProblemType OptimizationProblemType =>
              OptimizationProblemType.BOP_INTEGER_PROGRAMMING;
  }
}