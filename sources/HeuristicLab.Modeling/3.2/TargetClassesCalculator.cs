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
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.Modeling {
  public class TargetClassesCalculator : OperatorBase {

    public TargetClassesCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "The dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetClassValues", "The original class values of target variable (for instance negative=0 and positive=1).", typeof(ItemList<DoubleData>), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;

      ItemList<DoubleData> classes = new ItemList<DoubleData>();
      foreach (double x in dataset.GetVariableValues(targetVariable).Distinct().OrderBy(v => v)) {
        classes.Add(new DoubleData(x));
      }

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TargetClassValues"), classes));
      return null;
    }
  }
}
