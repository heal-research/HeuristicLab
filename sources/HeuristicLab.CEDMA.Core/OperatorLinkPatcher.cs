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
using System.Xml;
using System.IO;
using System.IO.Compression;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Core {
  public static class OperatorLinkPatcher {
    public static void LinkDatabase(IOperatorGraph opGraph, IDatabase database) {
      foreach(IOperator op in opGraph.Operators) {
        LinkDatabase(op, database);
      }
    }

    public static void LinkDatabase(IOperator op, IDatabase database) {
      if(op is OperatorLink) {
        OperatorLink link = op as OperatorLink;
        link.Database = database;
      } else if(op is CombinedOperator) {
        LinkDatabase(((CombinedOperator)op).OperatorGraph, database);
      }
      // also patch operator links contained (indirectly) in variables
      foreach(VariableInfo varInfo in op.VariableInfos) {
        IVariable var = op.GetVariable(varInfo.ActualName);
        if(var != null && var.Value is IOperatorGraph) {
          LinkDatabase((IOperatorGraph)var.Value, database);
        } else if(var != null && var.Value is IOperator) {
          LinkDatabase((IOperator)var.Value, database);
        }
      }
    }
  }
}
