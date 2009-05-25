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

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class ConfusionMatrixEvaluator : GPClassificationEvaluatorBase {
    public override string Description {
      get {
        return @"Calculates the classifcation matrix of the model.";
      }
    }

    public ConfusionMatrixEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("ConfusionMatrix", "The confusion matrix of the model", typeof(IntMatrixData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, HeuristicLab.DataAnalysis.Dataset dataset, int targetVariable, double[] classes, double[] thresholds, int start, int end) {
      IntMatrixData matrix = GetVariableValue<IntMatrixData>("ConfusionMatrix", scope, false, false);
      if (matrix == null) {
        matrix = new IntMatrixData(new int[classes.Length, classes.Length]);
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("ConfusionMatrix"), matrix));
      }

      int nSamples = end - start;
      for (int sample = start; sample < end; sample++) {
        double est = evaluator.Evaluate(sample);
        double origClass = dataset.GetValue(sample, targetVariable);
        int estClassIndex = -1;
        // if estimation is lower than the smallest threshold value -> estimated class is the lower class
        if (est < thresholds[0]) estClassIndex = 0;
        // if estimation is larger (or equal) than the largest threshold value -> estimated class is the upper class
        else if (est >= thresholds[thresholds.Length - 1]) estClassIndex = classes.Length - 1;
        else {
          // otherwise the estimated class is the class which upper threshold is larger than the estimated value
          for (int k = 0; k < thresholds.Length; k++) {
            if (thresholds[k] > est) {
              estClassIndex = k;
              break;
            }
          }
        }

        // find the first threshold index that is larger to the original value
        int origClassIndex = classes.Length - 1;
        for (int i = 0; i < thresholds.Length; i++) {
          if (origClass < thresholds[i]) {
            origClassIndex = i;
            break;
          }
        }
        matrix.Data[origClassIndex, estClassIndex]++;
      }
    }
  }
}
