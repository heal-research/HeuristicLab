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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using System;

namespace HeuristicLab.GP.StructureIdentification {
  /// <summary>
  /// Creates accumulated frequencies of variable-symbols over the whole population.
  /// </summary>
  public class VariableFrequencyAnalyser : OperatorBase {
    public override string Description {
      get {
        return @"Creates accumulated frequencies of variable-symbols over the whole population.";
      }
    }
    public VariableFrequencyAnalyser()
      : base() {
      AddVariableInfo(new VariableInfo("InputVariables", "The input variables", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to analyse", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("VariableFrequency", "The accumulated variable-frequencies over the whole population.", typeof(ItemList<ItemList>), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<ItemList> frequenciesList = GetVariableValue<ItemList<ItemList>>("VariableFrequency", scope, false, false);
      ItemList inputVariables = GetVariableValue<ItemList>("InputVariables", scope, true);
      if (frequenciesList == null) {
        frequenciesList = new ItemList<ItemList>();
        // first line should contain a list of variables
        ItemList varList = new ItemList();
        foreach (var inputVariable in inputVariables) {
          varList.Add(inputVariable);
        }
        frequenciesList.Add(varList);
        IVariableInfo info = GetVariableInfo("VariableFrequency");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, frequenciesList));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), frequenciesList));
      }
      double[] frequencySum = new double[inputVariables.Count()];
      int variableNodesSum = 0;
      foreach (var subScope in scope.SubScopes) {
        IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", subScope, false);
        var subScopeFrequencies = GetFrequencies(gpModel.FunctionTree, inputVariables);
        if (subScopeFrequencies.Count() != frequencySum.Length) throw new InvalidProgramException();
        int i = 0;
        foreach (var freq in subScopeFrequencies) {
          frequencySum[i++] += freq;
        }
        variableNodesSum += CountVariableNodes(gpModel.FunctionTree);
      }
      ItemList freqList = new ItemList();
      for (int i = 0; i < frequencySum.Length; i++) {
        freqList.Add(new DoubleData(frequencySum[i] / variableNodesSum));
      }
      frequenciesList.Add(freqList);
      return null;
    }

    private int CountVariableNodes(IFunctionTree tree) {
      return (from x in FunctionTreeIterator.IteratePostfix(tree)
              where x is VariableFunctionTree
              select 1).Sum();
    }

    private static IEnumerable<double> GetFrequencies(IFunctionTree tree, ItemList inputVariables) {
      var groupedFuns = (from node in FunctionTreeIterator.IteratePostfix(tree)
                         let varNode = node as VariableFunctionTree
                         where varNode != null
                         select varNode.VariableName).GroupBy(x => x);

      foreach (var inputVariable in inputVariables.Cast<StringData>()) {
        var matchingFuns = from g in groupedFuns
                           where g.Key == inputVariable.Data
                           select g.Count();
        if (matchingFuns.Count() == 0) yield return 0.0;
        else {
          yield return matchingFuns.Single(); // / (double)gpModel.Size;
        }
      }
    }
  }
}
