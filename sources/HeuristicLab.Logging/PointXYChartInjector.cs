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
  class PointXYChartInjector : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public PointXYChartInjector() {
      VariableInfo connectDotsInfo = new VariableInfo("ConnectDots", "Number of lines the linechart consists of", typeof(BoolData), VariableKind.In);
      connectDotsInfo.Local = true;
      AddVariableInfo(connectDotsInfo);
      AddVariable(new Variable("ConnectDots", new BoolData(true)));
      AddVariableInfo(new VariableInfo("Values", "Item list holding the values of the pointXYchart", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("PointXYchart", "Object representing a pointXYchart", typeof(PointXYChart), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      bool connectDots = GetVariableValue<BoolData>("ConnectDots", scope, true).Data;
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      PointXYChart pointXYchart = GetVariableValue<PointXYChart>("PointXYchart", scope, false, false);
      if (pointXYchart == null) {
        pointXYchart = new PointXYChart(connectDots, values);
        IVariableInfo info = GetVariableInfo("PointXYchart");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, pointXYchart));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), pointXYchart));
      } else
        pointXYchart.Values = values;
      return null;
    }
  }
}
