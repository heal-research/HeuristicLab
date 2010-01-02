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
using HeuristicLab.GP.Interfaces;
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class AvergePercentageChangeEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"TASK";
      }
    }

    public AvergePercentageChangeEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Differential", "Wether to transform the target variable to percentage change first or not.", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("APC", "The average percentage change of the model", typeof(DoubleData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, IFunctionTree tree, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end) {
      bool differential = GetVariableValue<BoolData>("Differential", scope, true).Data;
      DoubleData apc = GetVariableValue<DoubleData>("APC", scope, false, false);
      if (apc == null) {
        apc = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("APC"), apc));
      }

      double percentageSum = 0;
      double[] estimatedValues = evaluator.Evaluate(dataset, tree, Enumerable.Range(start, end - start)).ToArray();
      double[] originalValues = dataset.GetVariableValues(targetVariable, start - 1, end);
      for (int i = 0; i < estimatedValues.Length; i++) {
        double prevOriginal;
        double originalPercentageChange;
        double estimatedPercentageChange;
        if (differential) {
          prevOriginal = originalValues[i];
          originalPercentageChange = (originalValues[i + 1] - prevOriginal) / prevOriginal;
          estimatedPercentageChange = (estimatedValues[i] - prevOriginal) / prevOriginal;

        } else {
          originalPercentageChange = originalValues[i + 1];
          estimatedPercentageChange = estimatedValues[i];

        }
        if (!double.IsNaN(originalPercentageChange) && !double.IsInfinity(originalPercentageChange)) {
          if ((estimatedPercentageChange > 0 && originalPercentageChange > 0) ||
            (estimatedPercentageChange < 0 && originalPercentageChange < 0)) {
            percentageSum += Math.Abs(originalPercentageChange);
          } else if ((estimatedPercentageChange > 0 && originalPercentageChange < 0) ||
            (estimatedPercentageChange < 0 && originalPercentageChange > 0)) {
            percentageSum -= Math.Abs(originalPercentageChange);
          }
        }
      }

      percentageSum /= (end - start);
      if (double.IsNaN(percentageSum) || double.IsInfinity(percentageSum)) {
        percentageSum = double.MinValue;
      }
      apc.Data = percentageSum;
    }
  }
}
