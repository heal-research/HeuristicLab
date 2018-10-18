using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("SCIP", "SCIP (http://scip.zib.de/) must be installed and licenced.")]
  [StorableClass]
  public class ScipSolver : ExternalSolver {

    public ScipSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.ScipLibraryName }));
      programmingTypeParam.Value =
        (EnumValue<LinearProgrammingType>)new EnumValue<LinearProgrammingType>(LinearProgrammingType
          .MixedIntegerProgramming).AsReadOnly();
    }

    public override bool SupportsPause => true;

    public override bool SupportsStop => true;

    protected override OptimizationProblemType OptimizationProblemType =>
              OptimizationProblemType.SCIP_MIXED_INTEGER_PROGRAMMING;
  }
}