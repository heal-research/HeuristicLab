using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("Gurobi", "Gurobi (http://www.gurobi.com/) must be installed and licenced.")]
  [StorableClass]
  public class GurobiSolver : ExternalIncrementalSolver {

    public GurobiSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = "gurobi80.dll" }));
    }

    protected GurobiSolver(GurobiSolver original, Cloner cloner)
          : base(original, cloner) {
      programmingTypeParam = cloner.Clone(original.programmingTypeParam);
    }

    public override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.GUROBI_LINEAR_PROGRAMMING
        : OptimizationProblemType.GUROBI_MIXED_INTEGER_PROGRAMMING;

    public override bool SupportsPause => true;
    public override bool SupportsStop => true;
  }
}