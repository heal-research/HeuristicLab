#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("CMA Linear-weighted Recombinator", "Calculates weighted mean using linear decreasing weights.")]
  [StorableType("628D27E7-AD3F-418B-A44D-E5338017CA69")]
  public class CMALinearweightedRecombinator : CMARecombinator {

    [StorableConstructor]
    protected CMALinearweightedRecombinator(StorableConstructorFlag _) : base(_) { }
    protected CMALinearweightedRecombinator(CMALinearweightedRecombinator original, Cloner cloner) : base(original, cloner) { }
    public CMALinearweightedRecombinator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMALinearweightedRecombinator(this, cloner);
    }

    protected override double[] GetWeights(int mu) {
      var weights = new double[mu];
      var sum = (mu + 1) * mu / 2.0; // sum of arithmetic progression mu..1
      for (int i = 0; i < mu; i++)
        weights[i] = (mu - i) / sum;
      return weights;
    }
  }
}