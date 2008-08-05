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
    public override string Description {
      get {
        return @"TASK";
      }
    }

    public AccuracyEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("ClassSeparation", "The value of separation between negative and positive target classification values (for instance 0.5 if negative=0 and positive=1).", typeof(DoubleData), VariableKind.In));
    }

    private double[] original = new double[1];
    private double[] estimated = new double[1];
    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      int nSamples = trainingEnd-trainingStart;
      double limit = GetVariableValue<DoubleData>("ClassSeparation", scope, true).Data;
      double TP = 0;
      double TN = 0;
      double targetMean = dataset.GetMean(targetVariable, trainingStart, trainingEnd);
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double est = evaluator.Evaluate(sample);
        double orig = dataset.GetValue(sample, targetVariable);
        if(double.IsNaN(est) || double.IsInfinity(est)) {
          est = targetMean + maximumPunishment;
        } else if(est > targetMean + maximumPunishment) {
          est = targetMean + maximumPunishment;
        } else if(est < targetMean - maximumPunishment) {
          est = targetMean - maximumPunishment;
        }
        if(orig >= limit && est>=limit) TP++;
        if(orig < limit && est < limit) TN++;
      }
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * nSamples;
      return (TP+TN) / nSamples;
    }
  }
}
