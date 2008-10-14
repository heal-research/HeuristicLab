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

namespace HeuristicLab.GP.Classification {
  public class ClassificationMatrixEvaluator : GPEvaluatorBase {
    private const double EPSILON = 1.0E-6;
    private double[] classesArr;
    private double[] thresholds;
    private IntMatrixData matrix;
    public override string Description {
      get {
        return @"Calculates the classifcation matrix of the model.";
      }
    }

    public ClassificationMatrixEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("ClassificationMatrix", "The resulting classification matrix of the model", typeof(IntMatrixData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TargetClassValues", "The original class values of target variable (for instance negative=0 and positive=1).", typeof(ItemList<DoubleData>), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<DoubleData> classes = GetVariableValue<ItemList<DoubleData>>("TargetClassValues", scope, true);
      classesArr = new double[classes.Count];
      for(int i = 0; i < classesArr.Length; i++) classesArr[i] = classes[i].Data;
      Array.Sort(classesArr);
      thresholds = new double[classes.Count - 1];
      for(int i = 0; i < classesArr.Length - 1; i++) {
        thresholds[i] = (classesArr[i] + classesArr[i + 1]) / 2.0;
      }

      matrix = GetVariableValue<IntMatrixData>("ClassificationMatrix", scope, false, false);
      if(matrix == null) {
        matrix = new IntMatrixData(new int[classesArr.Length, classesArr.Length]);
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("ClassificationMatrix"), matrix));
      }
      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      int nSamples = end - start;
      for(int sample = start; sample < end; sample++) {
        double est = GetEstimatedValue(sample);
        double origClass = GetOriginalValue(sample);
        int estClassIndex = -1;
        // if estimation is lower than the smallest threshold value -> estimated class is the lower class
        if(est < thresholds[0]) estClassIndex = 0;
        // if estimation is larger (or equal) than the largest threshold value -> estimated class is the upper class
        else if(est >= thresholds[thresholds.Length - 1]) estClassIndex = classesArr.Length - 1;
        else {
          // otherwise the estimated class is the class which upper threshold is larger than the estimated value
          for(int k = 0; k < thresholds.Length; k++) {
            if(thresholds[k] > est) {
              estClassIndex = k;
              break;
            }
          }
        }
        SetOriginalValue(sample, classesArr[estClassIndex]);

        int origClassIndex = -1;
        for(int i = 0; i < classesArr.Length; i++) {
          if(IsEqual(origClass, classesArr[i])) origClassIndex = i;
        }
        matrix.Data[origClassIndex, estClassIndex]++;
      }
    }

    private bool IsEqual(double x, double y) {
      return Math.Abs(x - y) < EPSILON;
    }
  }
}
