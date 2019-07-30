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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.NK {
  [Item("EqualWeightsInitializer", "Initializes all weights to 1.0.")]
  [StorableType("AFFF8CCF-FF3E-48F6-861D-2A5E595FA0C0")]
  public sealed class EqualWeightsInitializer : ParameterizedNamedItem, IWeightsInitializer {
    [StorableConstructor]
    private EqualWeightsInitializer(StorableConstructorFlag _) : base(_) { }
    private EqualWeightsInitializer(EqualWeightsInitializer original, Cloner cloner)
      : base(original, cloner) { }
    public EqualWeightsInitializer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EqualWeightsInitializer(this, cloner);
    }

    public IEnumerable<double> GetWeights(int nComponents) {
      return new[] { 1.0 };
    }
  }
}
