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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMA Equal-weighted Recombinator", "Calculates weighted mean using equal weights.")]
  [StorableType("989FE031-C410-4435-B5F5-17A22C4A97BC")]
  public class CMAEqualweightedRecombinator : CMARecombinator {

    [StorableConstructor]
    protected CMAEqualweightedRecombinator(StorableConstructorFlag _) : base(_) { }
    protected CMAEqualweightedRecombinator(CMAEqualweightedRecombinator original, Cloner cloner) : base(original, cloner) { }
    public CMAEqualweightedRecombinator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAEqualweightedRecombinator(this, cloner);
    }

    protected override double[] GetWeights(int mu) {
      var weights = new double[mu];
      for (int i = 0; i < mu; i++)
        weights[i] = 1.0 / mu;
      return weights;
    }
  }
}