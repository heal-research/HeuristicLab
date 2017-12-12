#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.RAPGA {
  /// <summary>
  /// A selection operator which considers a single solution representation and selects duplicates.
  /// </summary>
  /// <remarks>
  /// The remaining scope then contains unique solutions and the selected scope their duplicates.
  /// </remarks>
  [Item("DuplicatesSelector", "A selection operator which considers a single solution representation and selects duplicates. The remaining scope then contains unique solutions and the selected scope their duplicates.")]
  [StorableClass]
  public sealed class DuplicatesSelector : SingleObjectiveSelector {
    public IValueLookupParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IValueLookupParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }

    [StorableConstructor]
    private DuplicatesSelector(bool deserializing) : base(deserializing) { }
    private DuplicatesSelector(DuplicatesSelector original, Cloner cloner) : base(original, cloner) { }
    public DuplicatesSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DuplicatesSelector(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("SimilarityCalculator"))
        Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));
      #endregion
    }

    protected override IScope[] Select(List<IScope> scopes) {
      bool copy = CopySelected.Value;

      var marks = new bool[scopes.Count];
      for (int i = 0; i < scopes.Count; i++)
        for (int j = i + 1; j < scopes.Count; j++)
          marks[j] = SimilarityCalculatorParameter.ActualValue.Equals(scopes[i], scopes[j]);


      var selected = new IScope[marks.Count(x => x)];
      int k = 0;
      if (copy) {
        for (int i = 0; i < scopes.Count; i++)
          if (marks[i]) {
            selected[k++] = (IScope)scopes[i].Clone();
          }
      } else {
        for (int i = 0; i < scopes.Count; i++)
          if (marks[i]) {
            selected[k++] = scopes[i];
            scopes[i] = null;
          }
        scopes.RemoveAll(x => x == null);
      }

      return selected;
    }
  }
}
