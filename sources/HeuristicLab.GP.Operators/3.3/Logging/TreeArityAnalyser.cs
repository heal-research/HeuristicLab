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

namespace HeuristicLab.GP.Operators {
  /// <summary>
  /// Creates a histogram of the arities of functions used in the whole GP population.
  /// </summary>
  public class TreeArityAnalyser : OperatorBase {
    public override string Description {
      get {
        return @"Creates a histogram of the arities of functions used in the whole GP population.";
      }
    }
    public TreeArityAnalyser()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to analyse", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Histogram", "The histogram of arities over all functions in all functiontrees", typeof(ItemList<ItemList<IntData>>), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<ItemList<IntData>> histogram = GetVariableValue<ItemList<ItemList<IntData>>>("Histogram", scope, false, false);
      if(histogram == null) {
        histogram = new ItemList<ItemList<IntData>>();
        IVariableInfo info = GetVariableInfo("Histogram");
        if(info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, histogram));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), histogram));
      } else {
        histogram.Clear();
      }

      foreach(IScope subScope in scope.SubScopes) {
        ItemList<ItemList<IntData>> subHistogram = GetArityHistogram(subScope);
        MergeHistogram(subHistogram, histogram);
      }

      return null;
    }

    private ItemList<ItemList<IntData>> GetArityHistogram(IScope subScope) {
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", subScope, false);
      List<int> arities = new List<int>();
      Arities(gpModel.FunctionTree, arities);
      var histogram = arities.GroupBy(x => x, (g, xs) => new { Group = g, Count = xs.Count() }).OrderBy(g=>g.Group); // count number of distinct arities
      ItemList<ItemList<IntData>> result = new ItemList<ItemList<IntData>>();
      
      // translate enumerable to itemlist
      histogram.Aggregate(result, (r, g) => { 
        ItemList<IntData> entry = new ItemList<IntData>(); 
        entry.Add(new IntData(g.Group));
        entry.Add(new IntData(g.Count));
        r.Add(entry); return r; 
      });
      return result;
    }

    private void Arities(IFunctionTree tree, List<int> arities) {
      arities.Add(tree.SubTrees.Count);
      foreach(IFunctionTree subTree in tree.SubTrees) {
        Arities(subTree, arities);
      }
    }

    private void MergeHistogram(ItemList<ItemList<IntData>> srcHistogram, ItemList<ItemList<IntData>> dstHistogram) {
      int j = 0;
      for(int i = 0; i < srcHistogram.Count; i++) {
        int g = srcHistogram[i][0].Data;
        int c = srcHistogram[i][1].Data;
        int dstG, dstC;
        if(j < dstHistogram.Count) {
          dstG = dstHistogram[j][0].Data;
          dstC = dstHistogram[j][1].Data;
        } else {
          dstG = int.MaxValue;
          dstC = 0;
        }
        if(g < dstG) {
          // new group before the current group
          if(j < dstHistogram.Count)
            dstHistogram.Insert(j, srcHistogram[i]);
          else
            dstHistogram.Add(srcHistogram[i]);
        } else if(g == dstG) {
          dstHistogram[j][1] = new IntData(c + dstC);
        }
        j++;
      }
    }
  }
}
