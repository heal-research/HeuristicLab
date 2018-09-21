using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("GLPK", "GLPK (https://www.gnu.org/software/glpk/) can be used out of the box.")]
  [StorableClass]
  public class GlpkSolver : Solver {

    public GlpkSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<StringValue>(nameof(LibraryName), new StringValue("glpk_4_65.dll")));
    }

    public override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.GLPK_LINEAR_PROGRAMMING
        : OptimizationProblemType.GLPK_MIXED_INTEGER_PROGRAMMING;
  }
}