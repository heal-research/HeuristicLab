using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("Glop", "Glop (https://developers.google.com/optimization/lp/glop) can be used out of the box.")]
  [StorableClass]
  public class GlopSolver : Solver {

    public GlopSolver() {
      Parameters.Remove(programmingTypeParam);
      Parameters.Add(programmingTypeParam = new FixedValueParameter<EnumValue<LinearProgrammingType>>(nameof(LinearProgrammingType),
        (EnumValue<LinearProgrammingType>)new EnumValue<LinearProgrammingType>().AsReadOnly()));
    }

    public override OptimizationProblemType OptimizationProblemType => OptimizationProblemType.GLOP_LINEAR_PROGRAMMING;
  }
}