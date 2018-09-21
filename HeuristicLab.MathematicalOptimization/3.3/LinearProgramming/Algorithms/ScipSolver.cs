using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("SCIP", "SCIP (http://scip.zib.de/) must be installed and licenced.")]
  [StorableClass]
  public class ScipSolver : Solver {

    public ScipSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<StringValue>(nameof(LibraryName), new StringValue("scip.dll")));
      Parameters.Remove(programmingTypeParam);
      Parameters.Add(programmingTypeParam = new FixedValueParameter<EnumValue<LinearProgrammingType>>(nameof(LinearProgrammingType),
        (EnumValue<LinearProgrammingType>)new EnumValue<LinearProgrammingType>(LinearProgrammingType.MixedIntegerProgramming).AsReadOnly()));
    }

    public override OptimizationProblemType OptimizationProblemType =>
      OptimizationProblemType.SCIP_MIXED_INTEGER_PROGRAMMING;
  }
}