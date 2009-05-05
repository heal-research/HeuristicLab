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
    }

    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      IVariableInfo qualityInfo = GetVariableInfo("QualitySamples");
      bool maximization = GetVariableValue<BoolData>("Maximization", source, true).Data;
      int groupSize = GetVariableValue<IntData>("GroupSize", source, true).Data;
      for (int i = 0; i < selected; i++) {
        if (source.SubScopes.Count < 1) throw new InvalidOperationException("No source scopes available to select.");

        IScope selectedScope = source.SubScopes[random.Next(source.SubScopes.Count)];
        double best = selectedScope.GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
        for (int j = 1; j < groupSize; j++) {
          IScope scope = source.SubScopes[random.Next(source.SubScopes.Count)];
          double quality = scope.GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
          if (((maximization) && (quality > best)) ||
              ((!maximization) && (quality < best))) {
            best = quality;
            selectedScope = scope;
          }
        }

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
