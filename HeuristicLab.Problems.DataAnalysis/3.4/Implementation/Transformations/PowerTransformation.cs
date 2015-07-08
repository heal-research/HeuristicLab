using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Power Transformation", "f(x) = x ^ exp | Represents a power transformation.")]
  public class PowerTransformation : Transformation<double> {
    protected const string ExponentParameterName = "Exponent";

    #region Parameters
    public IValueParameter<DoubleValue> ExponentParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[ExponentParameterName]; }
    }
    #endregion

    #region properties
    public override string ShortName {
      get { return "Pow"; }
    }
    public double Exponent {
      get { return ExponentParameter.Value.Value; }
    }
    #endregion

    [StorableConstructor]
    protected PowerTransformation(bool deserializing) : base(deserializing) { }
    protected PowerTransformation(PowerTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public PowerTransformation(IEnumerable<string> allowedColumns)
      : base(allowedColumns) {
      Parameters.Add(new ValueParameter<DoubleValue>(ExponentParameterName, "exp | Exponent for Exponentation", new DoubleValue(2.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PowerTransformation(this, cloner);
    }

    public override IEnumerable<double> Apply(IEnumerable<double> data) {
      return data.Select(i => Math.Pow(i, Exponent));
    }

    public override bool Check(IEnumerable<double> data, out string errorMsg) {
      errorMsg = "";
      return true;
    }

  }
}
