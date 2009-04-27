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

namespace HeuristicLab.Operators {
  /// <summary>
  /// Collects values of specific variable names in a given scope.
  /// </summary>
  public class DataCollector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DataCollector"/> with two variable infos 
    /// (<c>VariableNames</c> and <c>Values</c>).
    /// </summary>
    public DataCollector() {
      IVariableInfo variableNamesVariableInfo = new VariableInfo("VariableNames", "Names of variables whose values should be collected", typeof(ItemList<StringData>), VariableKind.In);
      variableNamesVariableInfo.Local = true;
      AddVariableInfo(variableNamesVariableInfo);
      ItemList<StringData> variableNames = new ItemList<StringData>();
      AddVariable(new Variable("VariableNames", variableNames));
      AddVariableInfo(new VariableInfo("Values", "Collected values", typeof(ItemList), VariableKind.New | VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Collects the values of a specified list of variable names in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to collect the values from.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      ItemList<StringData> names = GetVariableValue<ItemList<StringData>>("VariableNames", scope, false);
      ItemList values = GetVariableValue<ItemList>("Values", scope, true, false);
      if (values == null) {
        values = new ItemList();
        IVariableInfo info = GetVariableInfo("Values");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, values));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), values));
      }

      ItemList currentValues = new ItemList();
      for (int i = 0; i < names.Count; i++)
        currentValues.Add((IItem)scope.GetVariableValue(names[i].Data, true).Clone());
      values.Add(currentValues);
      return null;
    }
  }
}
