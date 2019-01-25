using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("D3330C77-060A-4F75-A0C5-47AC81C5F2DF")]
  [Item("Logarithmic Transformation", "f(x) = log(x, b) | Represents a logarithmic transformation.")]
  public class LogarithmicTransformation : Transformation<double> {
    protected const string BaseParameterName = "Base";

    #region Parameters
    public IValueParameter<DoubleValue> BaseParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[BaseParameterName]; }
    }
    #endregion

    #region properties
    public override string ShortName {
      get { return "Log"; }
    }
    public double Base {
      get { return BaseParameter.Value.Value; }
    }
    #endregion

    [StorableConstructor]
    protected LogarithmicTransformation(StorableConstructorFlag _) : base(_) { }
    protected LogarithmicTransformation(LogarithmicTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public LogarithmicTransformation(IEnumerable<string> allowedColumns)
      : base(allowedColumns) {
      Parameters.Add(new ValueParameter<DoubleValue>(BaseParameterName, "b | Base of log-function", new DoubleValue(Math.E)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LogarithmicTransformation(this, cloner);
    }

    public override IEnumerable<double> Apply(IEnumerable<double> data) {
      var b = Base;
      return data.Select(d => d > 0.0 ? Math.Log(d, b) : d);
    }

    public override bool Check(IEnumerable<double> data, out string errorMsg) {
      errorMsg = null;
      int errorCounter = data.Count(i => i <= 0.0);
      if (errorCounter > 0) {
        errorMsg = String.Format("{0} values are zero or below zero. Logarithm can not be applied onto these values", errorCounter);
        return false;
      }
      return true;
    }

  }
}
