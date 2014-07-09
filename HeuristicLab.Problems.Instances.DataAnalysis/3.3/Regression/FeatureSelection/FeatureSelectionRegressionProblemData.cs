#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeatureSelectionRegressionProblemData : RegressionProblemData {
    private const string SelectedFeaturesParameterName = "SelectedFeatures";
    private const string WeightsParameterName = "Weights";
    private const string OptimalRSquaredParameterName = "R² (best solution)";

    public IValueParameter<StringArray> SelectedFeaturesParameter {
      get { return (IValueParameter<StringArray>)Parameters[SelectedFeaturesParameterName]; }
    }

    public IValueParameter<DoubleArray> WeightsParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[WeightsParameterName]; }
    }

    public IValueParameter<DoubleValue> OptimalRSquaredParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[OptimalRSquaredParameterName]; }
    }

    [StorableConstructor]
    protected FeatureSelectionRegressionProblemData(bool deserializing)
      : base(deserializing) {
    }
    protected FeatureSelectionRegressionProblemData(FeatureSelectionRegressionProblemData original, Cloner cloner)
      : base(original, cloner) {
    }

    public FeatureSelectionRegressionProblemData(Dataset ds, IEnumerable<string> allowedInputVariables, string targetVariable, string[] selectedFeatures, double[] weights, double optimalRSquared)
      : base(ds, allowedInputVariables, targetVariable) {
      if (selectedFeatures.Length != weights.Length) throw new ArgumentException("Length of selected features vector does not match the length of the weights vector");
      if (optimalRSquared < 0 || optimalRSquared > 1) throw new ArgumentException("Optimal R² is not in range [0..1]");
      Parameters.Add(new FixedValueParameter<StringArray>(
        SelectedFeaturesParameterName,
        "Array of features used to generate the target values.",
        new StringArray(selectedFeatures).AsReadOnly()));
      Parameters.Add(new FixedValueParameter<DoubleArray>(
        WeightsParameterName,
        "Array of weights used to generate the target values.",
        (DoubleArray)(new DoubleArray(weights).AsReadOnly())));
      Parameters.Add(new FixedValueParameter<DoubleValue>(
        OptimalRSquaredParameterName,
        "R² of the optimal solution.",
        (DoubleValue)(new DoubleValue(optimalRSquared).AsReadOnly())));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FeatureSelectionRegressionProblemData(this, cloner);
    }
  }
}
