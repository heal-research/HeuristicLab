using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("Clp/Cbc", "Clp (https://projects.coin-or.org/Clp) and Cbc (https://projects.coin-or.org/Cbc) can be used out of the box.")]
  [StorableClass]
  public class CoinOrSolver : IncrementalSolver {

    public CoinOrSolver() {
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
        ? OptimizationProblemType.CLP_LINEAR_PROGRAMMING
        : OptimizationProblemType.CBC_MIXED_INTEGER_PROGRAMMING;
  }
}