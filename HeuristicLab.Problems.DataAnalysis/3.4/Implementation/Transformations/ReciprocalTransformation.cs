
using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;
namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("8D242A5A-5EBB-4618-958E-A7EF34151508")]
  [Item("Reciprocal Transformation", "f(x) = 1 / x | Represents a reciprocal transformation.")]
  public class ReciprocalTransformation : Transformation<double> {

    #region properties
    public override string ShortName {
      get { return "Inv"; }
    }
    #endregion

    [StorableConstructor]
    protected ReciprocalTransformation(StorableConstructorFlag _) : base(_) { }
    protected ReciprocalTransformation(ReciprocalTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public ReciprocalTransformation(IEnumerable<string> allowedColumns)
      : base(allowedColumns) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReciprocalTransformation(this, cloner);
    }

    public override IEnumerable<double> Apply(IEnumerable<double> data) {
      return data.Select(d => d > 0 ? 1.0 / d : d);
    }

    public override bool Check(IEnumerable<double> data, out string errorMsg) {
      errorMsg = null;
      int errorCounter = data.Count(i => i <= 0.0);
      if (errorCounter > 0) {
        errorMsg = String.Format("{0} values are zero or below zero. 1/x can not be applied onto these values", errorCounter);
        return false;
      }
      return true;
    }
  }
}
