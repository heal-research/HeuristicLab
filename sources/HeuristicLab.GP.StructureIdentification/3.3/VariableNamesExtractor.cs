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
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.StructureIdentification {
  public class VariableNamesExtractor : OperatorBase {
    public VariableNamesExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree from which the used variables should be extracted", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("VariableNames", "Extracted variable names from model", typeof(ItemList<StringData>), VariableKind.New | VariableKind.Out));
    }

    public override string Description {
      get { return "Extracts the variable names used in the given function tree."; }
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel model = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      ItemList<StringData> data = GetVariableValue<ItemList<StringData>>("VariableNames", scope, true, false);
      if (data == null) {
        data = new ItemList<StringData>();
        IVariableInfo info = GetVariableInfo("VariableNames");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, data));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), data));
      }
      data.Clear();

      foreach (string s in ExtractVariableNames(model.FunctionTree)) {
        data.Add(new StringData(s));
      }

      return null;
    }

    public static IEnumerable<string> ExtractVariableNames(IFunctionTree functionTree) {
      List<string> names = new List<string>();
      Extract(functionTree, names);
      return names;
    }

    private static void Extract(IFunctionTree functionTree, List<string> variableNames) {
      if (functionTree is VariableFunctionTree) {
        VariableFunctionTree v = (VariableFunctionTree)functionTree;
        if (!variableNames.Contains(v.VariableName))
          variableNames.Add(v.VariableName);
      }

      foreach (IFunctionTree child in functionTree.SubTrees) {
        Extract(child, variableNames);
      }
    }
  }
}
