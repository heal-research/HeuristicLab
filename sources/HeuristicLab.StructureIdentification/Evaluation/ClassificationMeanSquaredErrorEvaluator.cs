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
  public class ClassificationMeanSquaredErrorEvaluator : GPEvaluatorBase {
    protected double[] backupValues;
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'DataSet' and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.";
      }
    }

    public ClassificationMeanSquaredErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("TargetClassValues", "The original class values of target variable (for instance negative=0 and positive=1).", typeof(ItemList<DoubleData>), VariableKind.In));
    }

    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      ItemList<DoubleData> classes = GetVariableValue<ItemList<DoubleData>>("TargetClassValues", scope, true);
      double[] classesArr = new double[classes.Count];
      for(int i = 0; i < classesArr.Length; i++) classesArr[i] = classes[i].Data;
      Array.Sort(classesArr);

      double errorsSquaredSum = 0;
      double targetMean = dataset.GetMean(targetVariable, trainingStart, trainingEnd);
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double estimated = evaluator.Evaluate(sample);
        double original = dataset.GetValue(sample, targetVariable);
        if(double.IsNaN(estimated) || double.IsInfinity(estimated)) {
          estimated = targetMean + maximumPunishment;
        } else if(estimated > targetMean + maximumPunishment) {
          estimated = targetMean + maximumPunishment;
        } else if(estimated < targetMean - maximumPunishment) {
          estimated = targetMean - maximumPunishment;
        }
        double error = estimated - original;
        if(estimated < classesArr[0] ||
          estimated > classesArr[classesArr.Length - 1]) {
          errorsSquaredSum += Math.Abs(error); // only add linear error below the smallest class or above the largest class
        } else {
          errorsSquaredSum += error * error;
        }
      }

      errorsSquaredSum /= (trainingEnd-trainingStart);
      if(double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * (trainingEnd-trainingStart);
      return errorsSquaredSum;
    }
  }
}
