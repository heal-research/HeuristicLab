#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.StatisticalAnalysis;

namespace HeuristicLab.Selection.Uncertainty {
  public class UncertainBestSelector : StochasticSelectorBase {
    public override string Description {
      get { return @"Selects an individual from a tournament group, based on tests of statistical significance of quality arrays."; }
    }

    public UncertainBestSelector()
      : base() {
      AddVariableInfo(new VariableInfo("QualitySamples", "The array of quality samples resulting from several evaluations", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      GetVariable("CopySelected").GetValue<BoolData>().Data = true;
      AddVariableInfo(new VariableInfo("SignificanceLevel", "The significance level for the mann whitney wilcoxon rank sum test", typeof(DoubleData), VariableKind.In));
      GetVariableInfo("SignificanceLevel").Local = true;
      AddVariable(new Variable("SignificanceLevel", new DoubleData(0.05)));
    }

    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      if (source.SubScopes.Count < 1) throw new InvalidOperationException("No source scopes available to select.");
      IVariableInfo qualityInfo = GetVariableInfo("QualitySamples");
      bool maximization = GetVariableValue<BoolData>("Maximization", source, true).Data;
      double alpha = GetVariableValue<DoubleData>("SignificanceLevel", source, true).Data;

      int poolSize = source.SubScopes.Count;
      double[][] selectionGroupSamples = new double[poolSize][];
      double[] selectionGroupAverages = new double[poolSize];
      int[] selectionGroupIndices = new int[poolSize];
      for (int j = 0; j < poolSize; j++) {
        selectionGroupIndices[j] = j;
        selectionGroupSamples[j] = source.SubScopes[j].GetVariableValue<DoubleArrayData>(qualityInfo.FormalName, false).Data;
        double sum = 0.0;
        for (int k = 0; k < selectionGroupSamples[j].Length; k++) {
          sum += selectionGroupSamples[j][k];
        }
        selectionGroupAverages[j] = sum / (double)selectionGroupSamples[j].Length;
      }

      int[] rankList = new int[poolSize];
      for (int j = 0; j < poolSize - 1; j++) {
        for (int k = j + 1; k < poolSize; k++) {
          if (MannWhitneyWilcoxonTest.TwoTailedTest(selectionGroupSamples[j], selectionGroupSamples[k], alpha)) { // if a 2-tailed test is successful it means that two solutions are likely different
            if (maximization && selectionGroupAverages[j] > selectionGroupAverages[k]
              || !maximization && selectionGroupAverages[j] < selectionGroupAverages[k]) {
              rankList[j]++;
            } else if (maximization && selectionGroupAverages[j] < selectionGroupAverages[k]
              || !maximization && selectionGroupAverages[j] > selectionGroupAverages[k]) {
              rankList[k]++;
            }
            // else there's a statistical significant difference, but equal average qualities... can that happen? in any case, nobody gets a rank increase
          }
        }
      }

      Array.Sort<int, int>(rankList, selectionGroupIndices);
      Array.Sort<int>(rankList);

      List<IScope> selectedScopes = new List<IScope>();
      for (int i = 0; i < selected; i++) {        
        int selectedScopeIndex = selectionGroupIndices[poolSize - i - 1];
        IScope selectedScope = source.SubScopes[selectedScopeIndex];
        target.AddSubScope((IScope)selectedScope.Clone());
        selectedScopes.Add(selectedScope);
      }
      if (!copySelected) {
        while (selectedScopes.Count > 0) {
          source.RemoveSubScope(selectedScopes[0]);
          selectedScopes.RemoveAt(0);
        }
      }
    }
  }
}
