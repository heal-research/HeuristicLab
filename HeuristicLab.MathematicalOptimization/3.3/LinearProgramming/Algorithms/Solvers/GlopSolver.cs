using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("Glop", "Glop (https://developers.google.com/optimization/lp/glop) can be used out of the box.")]
  [StorableClass]
  public class GlopSolver : IncrementalSolver {

    public GlopSolver() {
      programmingTypeParam.Value = (EnumValue<LinearProgrammingType>)programmingTypeParam.Value.AsReadOnly();
    }

    public override OptimizationProblemType OptimizationProblemType => OptimizationProblemType.GLOP_LINEAR_PROGRAMMING;
    public override bool SupportsPause => true;
    public override bool SupportsStop => true;
  }
}