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
using HeuristicLab.Operators;
using HeuristicLab.Functions;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public class AccuracyEvaluator : GPEvaluatorBase {
    private const double EPSILON = 1.0E-6;
    public override string Description {
      get {
        return @"TASK";
      }
    }

    public AccuracyEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("TargetClassValues", "The original class values of target variable (for instance negative=0 and positive=1).", typeof(ItemList<DoubleData>), VariableKind.In));
    }

    private double[] original = new double[1];
    private double[] estimated = new double[1];
    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      int nSamples = trainingEnd-trainingStart;
      ItemList<DoubleData> classes = GetVariableValue<ItemList<DoubleData>>("TargetClassValues", scope, true);
      double[] classesArr = new double[classes.Count];
      for(int i=0;i<classesArr.Length;i++) classesArr[i] = classes[i].Data;
      Array.Sort(classesArr);
      double[] thresholds = new double[classes.Count - 1];
      for(int i=0;i<classesArr.Length-1;i++) {
        thresholds[i] = (classesArr[i]+classesArr[i+1]) / 2.0;
      }

      int nCorrect = 0;
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double est = evaluator.Evaluate(sample);
        double origClass = dataset.GetValue(sample, targetVariable);
        double estClass = double.NaN;
        // if estimation is lower than the smallest threshold value -> estimated class is the lower class
        if(est < thresholds[0]) estClass = classesArr[0];
        // if estimation is larger (or equal) than the largest threshold value -> estimated class is the upper class
        else if(est >= thresholds[thresholds.Length - 1]) estClass = classesArr[classesArr.Length - 1];  
        else {
          // otherwise the estimated class is the class which upper threshold is larger than the estimated value
          for(int k = 0; k < thresholds.Length; k++) {
            if(thresholds[k] > est) {
              estClass = classesArr[k];
              break;
            }
          }
        }
        if(Math.Abs(estClass - origClass) < EPSILON) nCorrect++;
      }
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * nSamples;
      return  nCorrect / (double)nSamples;
    }
  }
}
