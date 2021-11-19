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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("AlgorithmLookupParameter", "A parameter that looks up algorithms by type.")]
  [StorableType("8d34b2ba-5fb3-4be0-915a-cfa18d2bc1a9")]
  public class AlgorithmLookupParameter : ContextLookupParameter<Algorithm> {

    [StorableConstructor]
    protected AlgorithmLookupParameter(StorableConstructorFlag _) : base(_) { }
    protected AlgorithmLookupParameter(AlgorithmLookupParameter original, Cloner cloner) : base(original, cloner) { }
    public AlgorithmLookupParameter() : base() { }
    public AlgorithmLookupParameter(string name) : base(name) { }
    public AlgorithmLookupParameter(string name, string description) : base(name, description) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlgorithmLookupParameter(this, cloner);
    }
  }
}
