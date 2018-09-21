using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("Gurobi", "Gurobi (http://www.gurobi.com/) must be installed and licenced.")]
  [StorableClass]
  public class GurobiSolver : Solver {

    public GurobiSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<StringValue>(nameof(LibraryName), new StringValue("gurobi80.dll")));
    }

    public override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.GUROBI_LINEAR_PROGRAMMING
        : OptimizationProblemType.GUROBI_MIXED_INTEGER_PROGRAMMING;
  }
}