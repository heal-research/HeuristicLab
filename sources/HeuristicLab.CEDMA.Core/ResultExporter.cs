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
using System.Xml;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.IO;

namespace HeuristicLab.CEDMA.Core {
  public class ResultExporter {
    internal void Export(IAgent agent, ResultTable t) {
      foreach(IResult r in agent.Results) {
        Export(r, t);
      }
      foreach(IAgent a in agent.SubAgents) {
        Export(a, t);
      }
    }

    private void Export(IResult result, ResultTable t) {
      Export(result.Item, t);
      foreach(IResult subResult in result.SubResults) {
        Export(subResult, t);
      }
    }

    private void Export(IItem item, ResultTable t) {
      if(item is IScope) {
        IScope scope = item as IScope;
        ResultRow row = new ResultRow();
        foreach(IVariable variable in scope.Variables) {
          row.AddAttribute(variable.Name, variable.Value.ToString());
        }
        t.AddRow(row);
      } 
    }
  }
}
