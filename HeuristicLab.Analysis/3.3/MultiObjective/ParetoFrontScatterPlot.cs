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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Analysis {
  [StorableType("1bc5f640-ed3a-49dd-9dca-aa034cc81e12")]
  [Item("Pareto Front Scatter Plot", "The optimal front, current front(s) and associated items.")]
  public class ParetoFrontScatterPlot<T> : Item where T : class, IItem {

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    [Storable]
    private int objectives;
    public int Objectives {
      get { return objectives; }
    }

    [Storable]
    private IReadOnlyList<double[][]> fronts;
    public IReadOnlyList<double[][]> Fronts {
      get { return fronts; }
    }

    [Storable]
    private IReadOnlyList<T[]> items;
    public IReadOnlyList<T[]> Items {
      get { return items; }
    }

    [Storable]
    private IReadOnlyList<double[]> bestKnownFront;
    public IReadOnlyList<double[]> BestKnownFront {
      get { return bestKnownFront; }
    }

    [StorableConstructor]
    protected ParetoFrontScatterPlot(StorableConstructorFlag _) : base(_) { }
    public ParetoFrontScatterPlot() { }
    /// <summary>
    /// Provides the data for displaying a multi-objective population in a scatter plot.
    /// </summary>
    /// <param name="items">The solutions grouped by pareto front (first is best).</param>
    /// <param name="qualities">The objective vectors grouped by pareto front (first is best).</param>
    /// <param name="objectives">The number of objectives.</param>
    /// <param name="bestKnownParetoFront">Optional, the best known front in objective space.</param>
    public ParetoFrontScatterPlot(IReadOnlyList<T[]> items, IReadOnlyList<double[][]> qualities, int objectives, IReadOnlyList<double[]> bestKnownParetoFront = null) {
      this.fronts = qualities;
      this.items = items;
      this.bestKnownFront = bestKnownParetoFront;
      this.objectives = objectives;
    }
    /// <summary>
    /// Provides the data for displaying a multi-objective population in a scatter plot.
    /// </summary>
    /// <param name="fronts">The fronts (first is best) with the indices of the solutions and qualities.</param>
    /// <param name="items">The solutions.</param>
    /// <param name="qualities">The objective vectors for each solution.</param>
    /// <param name="objectives">The number of objectives.</param>
    /// <param name="bestKnownParetoFront">Optional, the best known front in objective space.</param>
    public ParetoFrontScatterPlot(List<List<int>> fronts, IReadOnlyList<T> items, IReadOnlyList<double[]> qualities, int objectives, IReadOnlyList<double[]> bestKnownParetoFront = null) {
      this.fronts = fronts.Select(x => x.Select(y => qualities[y]).ToArray()).ToArray();
      this.items = fronts.Select(x => x.Select(y => items[y]).ToArray()).ToArray();
      this.bestKnownFront = bestKnownParetoFront;
      this.objectives = objectives;

    }
    protected ParetoFrontScatterPlot(ParetoFrontScatterPlot<T> original, Cloner cloner)
      : base(original, cloner) {
      if (original.fronts != null) fronts = original.fronts.Select(s => s.Select(x => x.ToArray()).ToArray()).ToArray();
      if (original.items != null) items = original.items.Select(s => s.Select(cloner.Clone).ToArray()).ToArray();
      if (original.bestKnownFront != null) bestKnownFront = original.bestKnownFront.Select(s => s.ToArray()).ToArray();
      objectives = original.objectives;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParetoFrontScatterPlot<T>(this, cloner);
    }
  }
}
