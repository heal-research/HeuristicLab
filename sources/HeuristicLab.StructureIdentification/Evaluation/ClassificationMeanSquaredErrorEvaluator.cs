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
  public class ClassificationMeanSquaredErrorEvaluator : MeanSquaredErrorEvaluator {
    private const double EPSILON = 1.0E-6;
    private double[] classesArr;
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

    public override IOperation Apply(IScope scope) {
      ItemList<DoubleData> classes = GetVariableValue<ItemList<DoubleData>>("TargetClassValues", scope, true);
      classesArr = new double[classes.Count];
      for(int i = 0; i < classesArr.Length; i++) classesArr[i] = classes[i].Data;
      Array.Sort(classesArr);
      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      double errorsSquaredSum = 0;
      for(int sample = start; sample < end; sample++) {
        double estimated = GetEstimatedValue(sample);
        double original = GetOriginalValue(sample);
        SetOriginalValue(sample, estimated);
        if(!double.IsNaN(original) && !double.IsInfinity(original)) {
          double error = estimated - original;
          // between classes use squared error
          // on the lower end and upper end only add linear error if the absolute error is larger than 1
          // the error>1.0 constraint is needed for balance because in the interval ]-1, 1[ the squared error is smaller than the absolute error
          if((IsEqual(original, classesArr[0]) && error < -1.0) ||
            (IsEqual(original, classesArr[classesArr.Length - 1]) && error > 1.0)) {
            errorsSquaredSum += Math.Abs(error); // only add linear error below the smallest class or above the largest class
          } else {
            errorsSquaredSum += error * error;
          }
        }
      }

      errorsSquaredSum /= (end - start);
      if(double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }
      mse.Data = errorsSquaredSum;
    }

    private bool IsEqual(double x, double y) {
      return Math.Abs(x - y) < EPSILON;
    }
  }
}
