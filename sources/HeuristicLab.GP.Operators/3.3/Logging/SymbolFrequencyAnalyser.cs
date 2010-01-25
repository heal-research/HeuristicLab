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

namespace HeuristicLab.GP.Operators {
  /// <summary>
  /// Creates accumulated frequencies of function-symbols over the whole population.
  /// </summary>
  public class SymbolFrequencyAnalyser : OperatorBase {
    public override string Description {
      get {
        return @"Creates accumulated frequencies of function-symbols over the whole population.";
      }
    }
    public SymbolFrequencyAnalyser()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionLibrary", "The function library", typeof(FunctionLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to analyse", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("SymbolFrequency", "The accumulated symbol-frequencies over the whole population.", typeof(ItemList<ItemList>), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<ItemList> frequenciesList = GetVariableValue<ItemList<ItemList>>("SymbolFrequency", scope, false, false);
      FunctionLibrary funLib = GetVariableValue<FunctionLibrary>("FunctionLibrary", scope, true);
      if (frequenciesList == null) {
        frequenciesList = new ItemList<ItemList>();
        // first line should contain a list of all functions
        ItemList funList = new ItemList();
        foreach (var fun in funLib.Functions) {
          funList.Add(fun);
        }
        frequenciesList.Add(funList);
        IVariableInfo info = GetVariableInfo("SymbolFrequency");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, frequenciesList));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), frequenciesList));
      }
      double[] frequencySum = new double[funLib.Functions.Count()];
      int treeSizeSum = 0;
      foreach (var subScope in scope.SubScopes) {
        IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", subScope, false);
        var subScopeFrequencies = GetFrequencies(gpModel.FunctionTree, funLib);
        if (subScopeFrequencies.Count() != frequencySum.Length) throw new InvalidProgramException();
        int i = 0;
        foreach (var freq in subScopeFrequencies) {
          frequencySum[i++] += freq;
        }
        treeSizeSum += gpModel.Size;
      }
      ItemList freqList = new ItemList();
      for (int i = 0; i < frequencySum.Length; i++) {
        freqList.Add(new DoubleData(frequencySum[i] / treeSizeSum));
      }
      frequenciesList.Add(freqList);
      return null;
    }

    private static IEnumerable<double> GetFrequencies(IFunctionTree tree, FunctionLibrary funLib) {
      var groupedFuns = (from node in FunctionTreeIterator.IteratePostfix(tree)
                         select node.Function).GroupBy(x => x);

      foreach (var fun in funLib.Functions) {
        var matchingFuns = from g in groupedFuns
                           where g.Key == fun
                           select g.Count();
        if (matchingFuns.Count() == 0) yield return 0.0;
        else {
          yield return matchingFuns.Single(); // / (double)gpModel.Size;
        }
      }
    }
  }
}
