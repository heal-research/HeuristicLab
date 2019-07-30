#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.TabuSearch {
  /// <summary>
  /// The tabu selector is a selection operator that separates the best n moves that are either not tabu or satisfy the default aspiration criterion from the rest. It expects the move subscopes to be sorted.
  /// </summary>
  /// <remarks>
  /// For different aspiration criteria a new operator should be implemented.
  /// </remarks>
  [Item("TabuSelector", "An operator that selects the best move that is either not tabu or satisfies the aspiration criterion. It expects the move subscopes to be sorted by the qualities of the moves (the best move is first).")]
  [StorableType("63A67432-6076-4BDE-B6D5-C9919FAA48DE")]
  public class TabuSelector : Selector {
    /// <summary>
    /// The best found quality so far.
    /// </summary>
    public LookupParameter<DoubleValue> BestQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    /// <summary>
    /// Whether to use the default aspiration criteria or not.
    /// </summary>
    public ValueLookupParameter<BoolValue> AspirationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Aspiration"]; }
    }
    /// <summary>
    /// Whether the problem is a maximization problem or not.
    /// </summary>
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    /// <summary>
    /// The parameter for the move qualities.
    /// </summary>
    public ILookupParameter<ItemArray<DoubleValue>> MoveQualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["MoveQuality"]; }
    }
    /// <summary>
    /// The parameter for the tabu status of the moves.
    /// </summary>
    public ILookupParameter<ItemArray<BoolValue>> MoveTabuParameter {
      get { return (ILookupParameter<ItemArray<BoolValue>>)Parameters["MoveTabu"]; }
    }
    protected IValueLookupParameter<BoolValue> CopySelectedParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["CopySelected"]; }
    }
    public ILookupParameter<BoolValue> EmptyNeighborhoodParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["EmptyNeighborhood"]; }
    }

    public BoolValue CopySelected {
      get { return CopySelectedParameter.Value; }
      set { CopySelectedParameter.Value = value; }
    }

    [StorableConstructor]
    protected TabuSelector(StorableConstructorFlag _) : base(_) { }
    protected TabuSelector(TabuSelector original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TabuSelector(this, cloner);
    }

    /// <summary>
    /// Initializes a new intsance with 6 parameters (<c>Quality</c>, <c>BestQuality</c>,
    /// <c>Aspiration</c>, <c>Maximization</c>, <c>MoveQuality</c>, and <c>MoveTabu</c>).
    /// </summary>
    public TabuSelector()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "The best found quality so far."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Aspiration", "Whether the default aspiration criterion should be used or not. The default aspiration criterion accepts a tabu move if it results in a better solution than the best solution found so far.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "Whether the problem is a maximization or minimization problem (used to decide whether a solution is better"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("MoveQuality", "The quality of the move."));
      Parameters.Add(new ScopeTreeLookupParameter<BoolValue>("MoveTabu", "The tabu status of the move."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("CopySelected", "True if the selected move should be copied.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<BoolValue>("EmptyNeighborhood", "Will be set to true if the neighborhood didn't contain any non-tabu moves, otherwise it is set to false."));
      CopySelectedParameter.Hidden = true;
    }

    /// <summary>
    /// Implements the tabu selection with the default aspiration criteria (choose a tabu move when it is better than the best so far).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the neighborhood contained too little moves which are not tabu.</exception>
    /// <param name="scopes">The scopes from which to select.</param>
    /// <returns>The selected scopes.</returns>
    protected override IScope[] Select(List<IScope> scopes) {
      bool copy = CopySelectedParameter.Value.Value;
      bool aspiration = AspirationParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      double bestQuality = BestQualityParameter.ActualValue.Value;
      ItemArray<DoubleValue> moveQualities = MoveQualityParameter.ActualValue;
      ItemArray<BoolValue> moveTabus = MoveTabuParameter.ActualValue;

      IScope[] selected = new IScope[1];

      // remember scopes that should be removed
      List<int> scopesToRemove = new List<int>();
      for (int i = 0; i < scopes.Count; i++) {
        if (!moveTabus[i].Value
          || aspiration && IsBetter(maximization, moveQualities[i].Value, bestQuality)) {
          scopesToRemove.Add(i);
          if (copy) selected[0] = (IScope)scopes[i].Clone();
          else selected[0] = scopes[i];
          break;
        }
      }

      if (selected[0] == null) {
        EmptyNeighborhoodParameter.ActualValue = new BoolValue(true);
        selected[0] = new Scope("All moves are tabu.");
      } else EmptyNeighborhoodParameter.ActualValue = new BoolValue(false);

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
