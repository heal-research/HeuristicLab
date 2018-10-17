using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("GLPK", "GLPK (https://www.gnu.org/software/glpk/) can be used out of the box.")]
  [StorableClass]
  public class GlpkSolver : ExternalIncrementalSolver {

    public GlpkSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = "glpk465.dll" }));

      programmingTypeParam.Value.ValueChanged += (sender, args) => {
        if (((EnumValue<LinearProgrammingType>)sender).Value == LinearProgrammingType.LinearProgramming) {
          incrementalityParam.Value = new BoolValue(true);
          incrementalityParam.Value.ValueChanged += (s, a) => {
            if (((BoolValue)s).Value) {
              qualityUpdateIntervalParam.Value = new TimeSpanValue(qualityUpdateIntervalParam.Value.Value);
            } else {
              qualityUpdateIntervalParam.Value = (TimeSpanValue)qualityUpdateIntervalParam.Value.AsReadOnly();
            }
          };
        } else {
          incrementalityParam.Value = (BoolValue)new BoolValue().AsReadOnly();
        }
      };
    }

    public override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.GLPK_LINEAR_PROGRAMMING
        : OptimizationProblemType.GLPK_MIXED_INTEGER_PROGRAMMING;
  }
}