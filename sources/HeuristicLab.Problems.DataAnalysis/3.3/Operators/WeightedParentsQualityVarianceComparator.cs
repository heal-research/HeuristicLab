#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using alglib;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Operators {
  [Item("WeightedParentsQualityVarianceComparator", "Compares the quality and variance of the quality against that of its parents (assumes the parents are subscopes to the child scope). This operator works with any number of subscopes > 0.")]
  [StorableClass]
  public class WeightedParentsQualityVarianceComparator : SingleSuccessorOperator, ISubScopesQualityComparator {
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<BoolValue> ResultParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Result"]; }
    }
    public IValueLookupParameter<DoubleValue> ConfidenceIntervalParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["ConfidenceInterval"]; }
    }
    public ILookupParameter<DoubleValue> LeftSideParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["LeftSide"]; }
    }
    public ILookupParameter<DoubleValue> LeftSideVarianceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["LeftSideVariance"]; }
    }
    public ILookupParameter<IntValue> LeftSideSamplesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["LeftSideSamples"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> RightSideParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["RightSide"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> RightSideVariancesParameters {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["RightSideVariances"]; }
    }
    public ILookupParameter<ItemArray<IntValue>> RightSideSamplesParameters {
      get { return (ILookupParameter<ItemArray<IntValue>>)Parameters["RightSideSamples"]; }
    }

    public WeightedParentsQualityVarianceComparator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, false otherwise"));
      Parameters.Add(new LookupParameter<BoolValue>("Result", "The result of the comparison: True means Quality is better, False means it is worse than parents."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ConfidenceInterval", "The confidence interval used for the test.", new DoubleValue(0.05)));

      Parameters.Add(new LookupParameter<DoubleValue>("LeftSide", "The quality of the child."));
      Parameters.Add(new LookupParameter<DoubleValue>("LeftSideVariance", "The variances of the quality of the new child."));
      Parameters.Add(new LookupParameter<IntValue>("LeftSideSamples", "The number of samples used to calculate the quality of the new child."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("RightSide", "The qualities of the parents."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("RightSideVariances", "The variances of the parents."));
      Parameters.Add(new LookupParameter<IntValue>("RightSideSamples", "The number of samples used to calculate the quality of the parent."));
    }

    public override IOperation Apply() {
      double leftQuality = LeftSideParameter.ActualValue.Value;
      double leftVariance = LeftSideVarianceParameter.ActualValue.Value;
      int leftSamples = LeftSideSamplesParameter.ActualValue.Value;

      ItemArray<DoubleValue> rightQualities = RightSideParameter.ActualValue;
      ItemArray<DoubleValue> rightVariances = RightSideVariancesParameters.ActualValue;
      ItemArray<IntValue> rightSamples = RightSideSamplesParameters.ActualValue;

      if (rightQualities.Length < 1) throw new InvalidOperationException(Name + ": No subscopes found.");
      bool maximization = MaximizationParameter.ActualValue.Value;

      int bestParentIndex;
      double bestParentQuality;
      double bestParentVariance;
      int bestParentSamples;

      if (maximization)
        bestParentQuality = rightQualities.Max(x => x.Value);
      else
        bestParentQuality = rightQualities.Min(x => x.Value);
      bestParentIndex = rightQualities.FindIndex(x => x.Value == bestParentQuality);
      bestParentVariance = rightVariances[bestParentIndex].Value;
      bestParentSamples = rightSamples[bestParentIndex].Value;

      double xmean = leftQuality;
      double xvar = leftVariance;
      int n = leftSamples;
      double ymean = bestParentQuality;
      double yvar = bestParentVariance;
      double m = bestParentSamples;


      //following code taken from ALGLIB studentttest line 351
      // Two-sample unpooled test
      double p = 0;
      double stat = (xmean - ymean) / Math.Sqrt(xvar / n + yvar / m);
      double c = xvar / n / (xvar / n + yvar / m);
      double df = (n - 1) * (m - 1) / ((m - 1) * AP.Math.Sqr(c) + (n - 1) * (1 - AP.Math.Sqr(c)));
      if ((double)(stat) > (double)(0))
        p = 1 - 0.5 * ibetaf.incompletebeta(df / 2, 0.5, df / (df + AP.Math.Sqr(stat)));
      else
        p = 0.5 * ibetaf.incompletebeta(df / 2, 0.5, df / (df + AP.Math.Sqr(stat)));
      double bothtails = 2 * Math.Min(p, 1 - p);
      double lefttail = p;
      double righttail = 1 - p;

      bool result = false;
      if (maximization)
        result = righttail < ConfidenceIntervalParameter.ActualValue.Value;
      else
        result = lefttail < ConfidenceIntervalParameter.ActualValue.Value;

      BoolValue resultValue = ResultParameter.ActualValue;
      if (resultValue == null) {
        ResultParameter.ActualValue = new BoolValue(result);
      } else {
        resultValue.Value = result;
      }



      return base.Apply();
    }
  }
}
