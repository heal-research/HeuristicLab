using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {

  [Item("CPLEX", "CPLEX (https://www.ibm.com/analytics/cplex-optimizer) must be installed and licenced.")]
  [StorableClass]
  public class CplexSolver : Solver {

    public CplexSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<StringValue>(nameof(LibraryName), new StringValue("cplex1280.dll")));
    }

    public override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.CPLEX_LINEAR_PROGRAMMING
        : OptimizationProblemType.CPLEX_MIXED_INTEGER_PROGRAMMING;
  }
}