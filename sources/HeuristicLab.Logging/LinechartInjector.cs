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
  /// <summary>
  /// Injects a new Linechart into the current scope.
  /// </summary>
  public class LinechartInjector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LinechartInjector"/> with three variable infos
    /// (<c>NumberOfLines</c>, <c>Values</c> and <c>Linechart</c>).
    /// </summary>
    public LinechartInjector() {
      VariableInfo numberOfLinesInfo = new VariableInfo("NumberOfLines", "Number of lines the linechart consists of", typeof(IntData), VariableKind.In);
      numberOfLinesInfo.Local = true;
      AddVariableInfo(numberOfLinesInfo);
      AddVariable(new Variable("NumberOfLines", new IntData(1)));
      AddVariableInfo(new VariableInfo("Values", "Item list holding the values of the linechart", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("Linechart", "Object representing a linechart", typeof(Linechart), VariableKind.New | VariableKind.Out));
    }

    /// <summary>
    /// Injects a new Linechart variable into the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The current scope where to inject the Linechart variable.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      int numberOfLines = GetVariableValue<IntData>("NumberOfLines", scope, true).Data;
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      Linechart linechart = GetVariableValue<Linechart>("Linechart", scope, false, false);
      if (linechart == null) {
        linechart = new Linechart(numberOfLines, values);
        IVariableInfo info = GetVariableInfo("Linechart");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, linechart));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), linechart));
      } else
        linechart.Values = values;
      return null;
    }
  }
}
