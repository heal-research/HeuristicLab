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

namespace HeuristicLab.Selection {
  /// <summary>
  /// Moves or copies a defined number of the best sub scopes from a source scope to a target scope.
  /// </summary>
  public class TournamentSelector : StochasticSelectorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TournamentSelector"/> with three variable infos 
    /// (<c>Maximization</c>, <c>Quality</c> and <c>GroupSize</c>, being a local variable and set to 
    /// <c>2</c>) with <c>CopySelected</c> set to <c>true</c>.
    /// </summary>
    public TournamentSelector() {
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("GroupSize", "Size of the tournament group", typeof(IntData), VariableKind.In));
      GetVariableInfo("GroupSize").Local = true;
      AddVariable(new Variable("GroupSize", new IntData(2)));
      GetVariable("CopySelected").GetValue<BoolData>().Data = true;
    }

    /// <summary>
    /// Copies or moves the best sub scopes from the given <paramref name="source"/> to the specified
    /// <paramref name="target"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no source sub scopes are available.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="source">The source scope from where to copy/move the sub scopes.</param>
    /// <param name="selected">The number of sub scopes to copy/move.</param>
    /// <param name="target">The target scope where to add the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether the sub scopes shall be moved or copied.</param>
    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      IVariableInfo qualityInfo = GetVariableInfo("Quality");
      bool maximization = GetVariableValue<BoolData>("Maximization", source, true).Data;
      int groupSize = GetVariableValue<IntData>("GroupSize", source, true).Data;
      for (int i = 0; i < selected; i++) {
        if (source.SubScopes.Count < 1) throw new InvalidOperationException("No source scopes available to select.");

        double best = maximization ? double.MinValue : double.MaxValue;
        IScope selectedScope = null;
        for (int j = 0; j < groupSize; j++) {
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
