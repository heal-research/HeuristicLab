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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Logging {
  public class BestAverageWorstQualityCalculator : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public BestAverageWorstQualityCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value of a solution", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BestQuality", "Quality value of best solution", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("AverageQuality", "Average quality value", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("WorstQuality", "Quality value of worst solution", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      double[] qualities = new double[scope.SubScopes.Count];

      for (int i = 0; i < scope.SubScopes.Count; i++)
        qualities[i] = scope.SubScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;

      double worst = qualities[0];
      double best = qualities[0];
      double average = qualities[0];
      for (int i = 1; i < qualities.Length; i++) {
        if (qualities[i] < worst) worst = qualities[i];
        if (qualities[i] > best) best = qualities[i];
        average += qualities[i];
      }
      average = average / qualities.Length;

      if (!GetVariableValue<BoolData>("Maximization", scope, true).Data) {
        double temp = worst;
        worst = best;
        best = temp;
      }

      SetValue(GetVariableInfo("BestQuality"), best, scope);
      SetValue(GetVariableInfo("AverageQuality"), average, scope);
      SetValue(GetVariableInfo("WorstQuality"), worst, scope);

      return null;
    }

    private void SetValue(IVariableInfo info, double value, IScope scope) {
      DoubleData data = GetVariableValue<DoubleData>(info.FormalName, scope, false, false);
      if (data == null) {
        data = new DoubleData();
        if (info.Local)
          AddVariable(new Variable(info.ActualName, data));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), data));
      }
      data.Data = value;
    }
  }
}
