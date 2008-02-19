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
  public class QualityLogger : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public QualityLogger()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value of a solution", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("QualityLog", "Log of best, average and worst quality values", typeof(Log), VariableKind.In | VariableKind.Out | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      double[] qualities = new double[scope.SubScopes.Count];

      for (int i = 0; i < scope.SubScopes.Count; i++)
        qualities[i] = scope.SubScopes[i].GetVariableValue<DoubleData>(GetVariableInfo("Quality").ActualName, false).Data;

      double min = qualities[0];
      double max = qualities[0];
      double average = qualities[0];
      for (int i = 1; i < qualities.Length; i++) {
        if (qualities[i] < min) min = qualities[i];
        if (qualities[i] > max) max = qualities[i];
        average += qualities[i];
      }
      average = average / qualities.Length;

      Log log = GetVariableValue<Log>("QualityLog", scope, false, false);
      if (log == null) {
        log = new Log(new ItemList());
        IVariableInfo info = GetVariableInfo("QualityLog");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, log));
        else
          scope.AddVariable(new Variable(info.ActualName, log));
      }
      log.Items.Add(new DoubleArrayData(new double[] { min, average, max } ));

      return null;
    }
  }
}
