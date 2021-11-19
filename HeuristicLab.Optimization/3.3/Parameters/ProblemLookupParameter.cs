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
  [Item("ProblemLookupParameter", "A parameter that looks up single-objective problems with a given encoding by type.")]
  [StorableType("d7b99bd8-616d-4ff3-90f8-7c4291a77690")]
  public class ProblemLookupParameter<TEncoding, TEncodedSolution> : ContextLookupParameter<SingleObjectiveProblem<TEncoding, TEncodedSolution>>
    where TEncoding: class, IEncoding
    where TEncodedSolution: class, IEncodedSolution {

    [StorableConstructor]
    protected ProblemLookupParameter(StorableConstructorFlag _) : base(_) { }
    protected ProblemLookupParameter(ProblemLookupParameter<TEncoding, TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public ProblemLookupParameter() : base() { }
    public ProblemLookupParameter(string name) : base(name) { }
    public ProblemLookupParameter(string name, string description) : base(name, description) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProblemLookupParameter<TEncoding, TEncodedSolution>(this, cloner);
    }
  }
}
