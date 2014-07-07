#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public sealed class DuplicatesSelector : SingleObjectiveSelector, ISimilarityBasedOperator {
    #region ISimilarityBasedOperator Members
    [Storable]
    public ISolutionSimilarityCalculator SimilarityCalculator { get; set; }
    #endregion

    [StorableConstructor]
    private DuplicatesSelector(bool deserializing) : base(deserializing) { }
    private DuplicatesSelector(DuplicatesSelector original, Cloner cloner)
      : base(original, cloner) {
      this.SimilarityCalculator = cloner.Clone(original.SimilarityCalculator);
    }
    public DuplicatesSelector() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DuplicatesSelector(this, cloner);
    }

    protected override IScope[] Select(List<IScope> scopes) {
      bool copy = CopySelected.Value;

      var marks = new bool[scopes.Count];
      for (int i = 0; i < scopes.Count; i++)
        for (int j = i + 1; j < scopes.Count; j++)
          marks[j] = SimilarityCalculator.Equals(scopes[i], scopes[j]);


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
