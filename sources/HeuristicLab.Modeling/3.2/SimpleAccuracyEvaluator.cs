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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Modeling {
  public class SimpleAccuracyEvaluator : SimpleEvaluatorBase {
    public override string OutputVariableName {
      get {
        return "Accuracy";
      }
    }
    public override string Description {
      get {
        return @"Calculates the total accuracy of the model (ratio of correctly classified instances to total number of instances) given a model and the list of possible target class values.";
      }
    }

    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      int nSamples = values.GetLength(0);
      int nCorrect = 0;
      double[] classes = CalculateTargetClasses(values);
      double[] thresholds = CalculateThresholds(classes);

      for (int sample = 0; sample < nSamples; sample++) {
        double est = values[sample, ESTIMATION_INDEX];
        double origClass = values[sample, ORIGINAL_INDEX];
        double estClass = double.NaN;
        // if estimation is lower than the smallest threshold value -> estimated class is the lower class
        if (est < thresholds[0]) estClass = classes[0];
        // if estimation is larger (or equal) than the largest threshold value -> estimated class is the upper class
        else if (est >= thresholds[thresholds.Length - 1]) estClass = classes[classes.Length - 1];
        else {
          // otherwise the estimated class is the class which upper threshold is larger than the estimated value
          for (int k = 0; k < thresholds.Length; k++) {
            if (thresholds[k] > est) {
              estClass = classes[k];
              break;
            }
          }
        }
        if (estClass.IsAlmost(origClass)) nCorrect++;
      }
      return nCorrect / (double)nSamples;
    }

    public static double[] CalculateTargetClasses(double[,] values) {
      int n = values.GetLength(0);
      double[] original = new double[n];
      for (int i = 0; i < n; i++) original[i] = values[i, ORIGINAL_INDEX];
      return original.OrderBy(x => x).Distinct().ToArray();
    }

    public static double[] CalculateThresholds(double[] targetClasses) {
      double[] thresholds = new double[targetClasses.Length - 1];
      for (int i = 1; i < targetClasses.Length; i++) {
        thresholds[i - 1] = (targetClasses[i - 1] + targetClasses[i]) / 2.0;
      }
      return thresholds;
    }
  }
}
