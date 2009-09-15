#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  /// <summary>
  /// The Variance Accounted For (VAF) function calculates is computed as
  /// VAF(y,y') = ( 1 - var(y-y')/var(y) )
  /// where y' denotes the predicted / modelled values for y and var(x) the variance of a signal x.
  /// </summary>
  public class SimpleVarianceAccountedForEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "VAF";
      }
    }

    public override double Evaluate(double[,] values) {
      try {
        return Calculate(values);
      }
      catch (ArgumentException) {
        return double.NegativeInfinity;
      }
    }

    public static double Calculate(double[,] values) {
      int n = values.GetLength(0);
      double[] errors = new double[n];
      double[] originalTargetVariableValues = new double[n];
      for (int i = 0; i < n; i++) {
        double estimated = values[i, ESTIMATION_INDEX];
        double original = values[i, ORIGINAL_INDEX];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
          !double.IsNaN(original) && !double.IsInfinity(original)) {
          errors[i] = original - estimated;
          originalTargetVariableValues[i] = original;
        } else {
          errors[i] = double.NaN;
          originalTargetVariableValues[i] = double.NaN;
        }
      }
      double errorsVariance = Statistics.Variance(errors);
      double originalsVariance = Statistics.Variance(originalTargetVariableValues);
      if (originalsVariance.IsAlmost(0.0))
        if (errorsVariance.IsAlmost(0.0)) {
          return 1.0;
        } else {
          throw new ArgumentException("Variance of original values is zero");
        } else {
        return 1.0 - errorsVariance / originalsVariance;
      }
    }
  }
}
