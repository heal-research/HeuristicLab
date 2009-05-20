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
  /// Logs a specified value into a specified log object.
  /// </summary>
  public class Logger : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Logger"/> with two variable infos 
    /// (<c>Value</c> and <c>Log</c>).
    /// </summary>
    public Logger()
      : base() {
      AddVariableInfo(new VariableInfo("Value", "The value that should be logged", typeof(ObjectData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Log", "Log into which the values are written", typeof(Log), VariableKind.In | VariableKind.Out | VariableKind.New));
    }

    /// <summary>
    /// Logs a specified value into a specified log object.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      ObjectData[] values = new ObjectData[scope.SubScopes.Count];

      for (int i = 0; i < scope.SubScopes.Count; i++)
        values[i] = (ObjectData)scope.SubScopes[i].GetVariableValue<ObjectData>("Value", false).Clone();

      Log log = GetVariableValue<Log>("Log", scope, false, false);
      if (log == null) {
        log = new Log(new ItemList());
        IVariableInfo info = GetVariableInfo("Log");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, log));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), log));
      }
      ItemList row = new ItemList();
      row.AddRange(values);
      log.Items.Add(row);

      return null;
    }
  }
}
