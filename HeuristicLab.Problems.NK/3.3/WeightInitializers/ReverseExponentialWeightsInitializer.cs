#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.NK {
  [Item("ReverseExponentialWeightsInitializer", "Assigns exponentially decreasing weights.")]
  [StorableClass]
  public sealed class ReverseExponentialWeightsInitializer : ParameterizedNamedItem, IWeightsInitializer {
    [StorableConstructor]
    private ReverseExponentialWeightsInitializer(bool deserializing) : base(deserializing) { }
    private ReverseExponentialWeightsInitializer(ReverseExponentialWeightsInitializer original, Cloner cloner)
      : base(original, cloner) {
    }
    public ReverseExponentialWeightsInitializer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReverseExponentialWeightsInitializer(this, cloner);
    }

    public IEnumerable<double> GetWeights(int nComponents) {
      for (int i = nComponents - 1; i >= 0; i--)
        yield return Math.Pow(2, i);
    }
  }
}
