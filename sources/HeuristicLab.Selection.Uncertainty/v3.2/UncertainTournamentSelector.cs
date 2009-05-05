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
using HeuristicLab.Selection;
using HeuristicLab.StatisticalAnalysis;

namespace HeuristicLab.Selection.Uncertainty {
  public class UncertainTournamentSelector : StochasticSelectorBase {
    public override string Description {
      get { return @"Selects an individual from a tournament group, based on tests of statistical significance of quality arrays."; }
    }

    public UncertainTournamentSelector()
      : base() {
      AddVariableInfo(new VariableInfo("QualitySamples", "The array of quality samples resulting from several evaluations", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("GroupSize", "Size of the tournament group", typeof(IntData), VariableKind.In));
      GetVariableInfo("GroupSize").Local = true;
      AddVariable(new Variable("GroupSize", new IntData(2)));
      GetVariable("CopySelected").GetValue<BoolData>().Data = true;
      AddVariableInfo(new VariableInfo("SignificanceLevel", "The significance level for the mann whitney wilcoxon rank sum test", typeof(DoubleData), VariableKind.In));
      GetVariableInfo("SignificanceLevel").Local = true;
      AddVariable(new Variable("SignificanceLevel", new DoubleData(0.05)));
    }

    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      IVariableInfo qualityInfo = GetVariableInfo("QualitySamples");
      bool maximization = GetVariableValue<BoolData>("Maximization", source, true).Data;
      int groupSize = GetVariableValue<IntData>("GroupSize", source, true).Data;
      double alpha = GetVariableValue<DoubleData>("SignificanceLevel", source, true).Data;

      for (int i = 0; i < selected; i++) {
        if (source.SubScopes.Count < 1) throw new InvalidOperationException("No source scopes available to select.");

        double[][] tournamentGroup = new double[groupSize][];
        int[] tournamentGroupIndices = new int[groupSize];
        double[] tournamentGroupAverages = new double[groupSize];
        for (int j = 0; j < groupSize; j++) {
          tournamentGroupIndices[j] = random.Next(source.SubScopes.Count);
          tournamentGroup[j] = source.SubScopes[tournamentGroupIndices[j]].GetVariableValue<DoubleArrayData>(qualityInfo.FormalName, false).Data;
          double sum = 0.0;
          for (int k = 0; k < tournamentGroup[j].Length; k++) {
            sum += tournamentGroup[j][k];
          }
          tournamentGroupAverages[j] = sum / (double)tournamentGroup[j].Length;
        }

        int[] rankList = new int[groupSize];
        int highestRank = 0;
        IList<int> equalRankList = new List<int>(groupSize);
        for (int j = 0; j < groupSize - 1; j++) {
          for (int k = j + 1; k < groupSize; k++) {
            if (MannWhitneyWilcoxonTest.TwoTailedTest(tournamentGroup[j], tournamentGroup[k], alpha)) { // if a 2-tailed test is successful it means that two solutions are likely different
              if (maximization && tournamentGroupAverages[j] > tournamentGroupAverages[k]
                || !maximization && tournamentGroupAverages[j] < tournamentGroupAverages[k]) {
                rankList[j]++;
                if (rankList[j] > highestRank) {
                  highestRank = rankList[j];
                  equalRankList.Clear();
                  equalRankList.Add(j);
                } else if (rankList[j] == highestRank) {
                  equalRankList.Add(j);
                }
              } else if (maximization && tournamentGroupAverages[j] < tournamentGroupAverages[k]
                || !maximization && tournamentGroupAverages[j] > tournamentGroupAverages[k]) {
                rankList[k]++;
                if (rankList[k] > highestRank) {
                  highestRank = rankList[k];
                  equalRankList.Clear();
                  equalRankList.Add(k);
                } else if (rankList[k] == highestRank) {
                  equalRankList.Add(k);
                }
              }
              // else there's a statistical significant difference, but equal average qualities... can that happen?
            }
          }
        }
        int selectedScopeIndex = 0;
        if (equalRankList.Count == 0)
          selectedScopeIndex = tournamentGroupIndices[random.Next(groupSize)]; // no significance in all the solutions, select one randomly
        else
          selectedScopeIndex = tournamentGroupIndices[equalRankList[random.Next(equalRankList.Count)]]; // select among those with the highest rank randomly

        IScope selectedScope = source.SubScopes[selectedScopeIndex];

        if (copySelected)
          target.AddSubScope((IScope)selectedScope.Clone());
        else {
          source.RemoveSubScope(selectedScope);
          target.AddSubScope(selectedScope);
        }
      }
    }
  }
}
