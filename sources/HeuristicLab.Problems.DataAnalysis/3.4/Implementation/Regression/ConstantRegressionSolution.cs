using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item(Name = "Constant Regression Solution", Description = "Represents a constant regression solution (model + data).")]
  public class ConstantRegressionSolution : RegressionSolution {
    public new ConstantRegressionModel Model {
      get { return (ConstantRegressionModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    protected ConstantRegressionSolution(bool deserializing) : base(deserializing) { }
    protected ConstantRegressionSolution(ConstantRegressionSolution original, Cloner cloner) : base(original, cloner) { }
    public ConstantRegressionSolution(ConstantRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      RecalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConstantRegressionSolution(this, cloner);
    }
  }
}
