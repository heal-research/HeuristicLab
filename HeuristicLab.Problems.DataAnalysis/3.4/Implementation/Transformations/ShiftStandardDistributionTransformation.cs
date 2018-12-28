using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("F0DC4951-64DF-4058-8E87-748D0FA1AA21")]
  [Item("Shift Standard Distribution Transformation", "f(x) = ((x - m_org) / s_org ) * s_tar + m_tar | Represents Transformation to unit standard deviation and additional linear transformation to a target Mean and Standard deviation")]
  public class ShiftStandardDistributionTransformation : Transformation<double> {
    protected const string OriginalMeanParameterName = "Original Mean";
    protected const string OriginalStandardDeviationParameterName = "Original Standard Deviation";
    protected const string MeanParameterName = "Mean";
    protected const string StandardDeviationParameterName = "Standard Deviation";

    #region Parameters
    public IValueParameter<DoubleValue> OriginalMeanParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[OriginalMeanParameterName]; }
    }
    public IValueParameter<DoubleValue> OriginalStandardDeviationParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[OriginalStandardDeviationParameterName]; }
    }
    public IValueParameter<DoubleValue> MeanParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[MeanParameterName]; }
    }
    public IValueParameter<DoubleValue> StandardDeviationParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[StandardDeviationParameterName]; }
    }
    #endregion

    #region properties
    public override string ShortName {
      get { return "Std"; }
    }
    public double OriginalMean {
      get { return OriginalMeanParameter.Value.Value; }
      set { OriginalMeanParameter.Value.Value = value; }
    }
    public double OriginalStandardDeviation {
      get { return OriginalStandardDeviationParameter.Value.Value; }
      set { OriginalStandardDeviationParameter.Value.Value = value; }
    }
    public double Mean {
      get { return MeanParameter.Value.Value; }
    }
    public double StandardDeviation {
      get { return StandardDeviationParameter.Value.Value; }
    }
    #endregion

    [StorableConstructor]
    protected ShiftStandardDistributionTransformation(StorableConstructorFlag _) : base(_) { }
    protected ShiftStandardDistributionTransformation(ShiftStandardDistributionTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public ShiftStandardDistributionTransformation(IEnumerable<string> allowedColumns)
      : base(allowedColumns) {
      Parameters.Add(new ValueParameter<DoubleValue>(OriginalMeanParameterName, "m_org | Mean value of the original data's deviation.", new DoubleValue()));
      Parameters.Add(new ValueParameter<DoubleValue>(OriginalStandardDeviationParameterName, "s_org | Standard deviation of the original data.", new DoubleValue()));
      OriginalMeanParameter.Hidden = true;
      OriginalStandardDeviationParameter.Hidden = true;
      Parameters.Add(new ValueParameter<DoubleValue>(MeanParameterName, "m_tar | Mean value for the target deviation.", new DoubleValue(0.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(StandardDeviationParameterName, "s_tar | Standard deviation for the target data.", new DoubleValue(1.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShiftStandardDistributionTransformation(this, cloner);
    }

    public override IEnumerable<double> Apply(IEnumerable<double> data) {
      if (OriginalStandardDeviation.IsAlmost(0.0)) {
        return data;
      }
      var old_m = OriginalMean;
      var old_s = OriginalStandardDeviation;
      var m = Mean;
      var s = StandardDeviation;
      return data
        .Select(d => (d - old_m) / old_s) // standardized
        .Select(d => d * s + m);
    }

    public override bool Check(IEnumerable<double> data, out string errorMsg) {
      ConfigureParameters(data);
      errorMsg = "";
      if (OriginalStandardDeviation.IsAlmost(0.0)) {
        errorMsg = "Standard deviaton for the original data is 0.0, Transformation cannot be applied onto these values.";
        return false;
      }
      return true;
    }

    public override void ConfigureParameters(IEnumerable<double> data) {
      OriginalStandardDeviation = data.StandardDeviation();
      OriginalMean = data.Average();
    }
  }
}
