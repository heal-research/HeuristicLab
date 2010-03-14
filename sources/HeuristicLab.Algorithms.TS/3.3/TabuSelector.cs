#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.TS {
  /// <summary>
  /// The tabu selector is a selection operator that separates the best n moves that are either not tabu or satisfy the default aspiration criterion from the rest. It expects the move subscopes to be sorted.
  /// </summary>
  /// <remarks>
  /// For different aspiration criteria a new operator should be implemented.
  /// </remarks>
  [Item("TabuSelector", "An operator that selects the best move that is either not tabu or satisfies the aspiration criterion. It expects the move subscopes to be sorted.")]
  [StorableClass]
  public class TabuSelector : Selector {
    /// <summary>
    /// The best found quality so far.
    /// </summary>
    public LookupParameter<DoubleData> BestQualityParameter {
      get { return (LookupParameter<DoubleData>)Parameters["BestQuality"]; }
    }
    /// <summary>
    /// Whether to use the default aspiration criteria or not.
    /// </summary>
    public ValueLookupParameter<BoolData> AspirationParameter {
      get { return (ValueLookupParameter<BoolData>)Parameters["Aspiration"]; }
    }
    /// <summary>
    /// Whether the problem is a maximization problem or not.
    /// </summary>
    public IValueLookupParameter<BoolData> MaximizationParameter {
      get { return (IValueLookupParameter<BoolData>)Parameters["Maximization"]; }
    }
    /// <summary>
    /// The parameter for the move qualities.
    /// </summary>
    public ILookupParameter<ItemArray<DoubleData>> MoveQualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleData>>)Parameters["MoveQuality"]; }
    }
    /// <summary>
    /// The parameter for the tabu status of the moves.
    /// </summary>
    public ILookupParameter<ItemArray<BoolData>> MoveTabuParameter {
      get { return (ILookupParameter<ItemArray<BoolData>>)Parameters["MoveTabu"]; }
    }

    public IntData NumberOfSelectedSubScopes {
      set { NumberOfSelectedSubScopesParameter.Value = value; }
    }

    /// <summary>
    /// Initializes a new intsance with 6 parameters (<c>Quality</c>, <c>BestQuality</c>,
    /// <c>Aspiration</c>, <c>Maximization</c>, <c>MoveQuality</c>, and <c>MoveTabu</c>).
    /// </summary>
    public TabuSelector()
      : base() {
      Parameters.Add(new LookupParameter<DoubleData>("BestQuality", "The best found quality so far."));
      Parameters.Add(new ValueLookupParameter<BoolData>("Aspiration", "Whether the default aspiration criterion should be used or not. The default aspiration criterion accepts a tabu move if it results in a better solution than the best solution found so far.", new BoolData(true)));
      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "Whether the problem is a maximization or minimization problem (used to decide whether a solution is better"));
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("MoveQuality", "The quality of the move."));
      Parameters.Add(new SubScopesLookupParameter<BoolData>("MoveTabu", "The tabu status of the move."));
    }

    /// <summary>
    /// Implements the tabu selection with the default aspiration criteria (choose a tabu move when it is better than the best so far).
    /// </summary>
    /// <param name="scopes">The scopes from which to select.</param>
    /// <returns>The selected scopes.</returns>
    protected override IScope[] Select(List<IScope> scopes) {
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      bool copy = CopySelectedParameter.Value.Value;
      bool aspiration = AspirationParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      double bestQuality = BestQualityParameter.ActualValue.Value;
      ItemArray<DoubleData> moveQualities = MoveQualityParameter.ActualValue;
      ItemArray<BoolData> moveTabus = MoveTabuParameter.ActualValue;

      IScope[] selected = new IScope[count];

      // remember scopes that should be removed
      List<int> scopesToRemove = new List<int>();
      for (int i = 0; i < scopes.Count; i++) {
        if (count > 0 && (!moveTabus[i].Value
          || aspiration && IsBetter(maximization, moveQualities[i].Value, bestQuality))) {
          scopesToRemove.Add(i);
          if (copy) selected[selected.Length - count] = (IScope)scopes[i].Clone();
          else selected[selected.Length - count] = scopes[i];
          count--;
          if (count == 0) break;
        }
      }

      if (count > 0) throw new InvalidOperationException("TabuSelector: The neighborhood contained no or too little moves that are not tabu.");

      // remove from last to first so that the stored indices remain the same
      if (!copy) {
        while (scopesToRemove.Count > 0) {
          scopes.RemoveAt(scopesToRemove[scopesToRemove.Count - 1]);
          scopesToRemove.RemoveAt(scopesToRemove.Count - 1);
        }
      }

      return selected;
    }

    private bool IsBetter(bool maximization, double moveQuality, double bestQuality) {
      return (maximization && moveQuality > bestQuality || !maximization && moveQuality < bestQuality);
    }
  }
}
