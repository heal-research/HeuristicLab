using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("4A91E704-927C-4278-AA11-79C16BD8E4F2")]
  [Item("Shift to Range Transformation", "f(x) = k * x + d, start <= f(x) <= end | Represents a linear Transformation using Parameters defining a target range")]
  public class ShiftToRangeTransformation : LinearTransformation {
    protected const string RangeParameterName = "Range";

    #region Parameters
    public IValueParameter<DoubleRange> RangeParameter {
      get { return (IValueParameter<DoubleRange>)Parameters[RangeParameterName]; }
    }
    #endregion

    #region properties
    public override string ShortName {
      get { return "Sft"; }
    }
    public DoubleRange Range {
      get { return RangeParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected ShiftToRangeTransformation(StorableConstructorFlag _) : base(_) { }
    protected ShiftToRangeTransformation(ShiftToRangeTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public ShiftToRangeTransformation(IEnumerable<string> allowedColumns)
      : base(allowedColumns) {
      MultiplierParameter.Hidden = true;
      AddendParameter.Hidden = true;
      Parameters.Add(new ValueParameter<DoubleRange>(RangeParameterName, "start, end | Range for the target window of the linear transformation", new DoubleRange(0.0, 1.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShiftToRangeTransformation(this, cloner);
    }

    public override bool Check(IEnumerable<double> data, out string errorMsg) {
      ConfigureParameters(data);
      return base.Check(data, out errorMsg);
    }

    public override void ConfigureParameters(IEnumerable<double> data) {
      double originalRangeStart = data.Min();
      double originalRangeEnd = data.Max();

      double originalRangeWidth = originalRangeEnd - originalRangeStart;
      double targetRangeWidth = Range.End - Range.Start;

      Multiplier = targetRangeWidth / originalRangeWidth;
      Addend = Range.Start - originalRangeStart * Multiplier;
    }
  }
}
