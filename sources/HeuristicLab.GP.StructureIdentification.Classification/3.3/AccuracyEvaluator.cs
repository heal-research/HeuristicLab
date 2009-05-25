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
using HeuristicLab.Data;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class AccuracyEvaluator : GPClassificationEvaluatorBase {
    private const double EPSILON = 1.0E-6;
    public override string Description {
      get {
        return @"Calculates the total accuracy of the model (ratio of correctly classified instances to total number of instances) given a model and the list of possible target class values.";
      }
    }

    public AccuracyEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Accuracy", "The total accuracy of the model (ratio of correctly classified instances to total number of instances)", typeof(DoubleData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, double[] classes, double[] thresholds, int start, int end) {
      DoubleData accuracy = GetVariableValue<DoubleData>("Accuracy", scope, false, false);
      if (accuracy == null) {
        accuracy = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Accuracy"), accuracy));
      }

      int nSamples = end - start;
      int nCorrect = 0;
      for (int sample = start; sample < end; sample++) {
        double est = evaluator.Evaluate(sample);
        double origClass = dataset.GetValue(sample, targetVariable);
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
        if (Math.Abs(estClass - origClass) < EPSILON) nCorrect++;
      }
      accuracy.Data = nCorrect / (double)nSamples;
    }
  }
}
