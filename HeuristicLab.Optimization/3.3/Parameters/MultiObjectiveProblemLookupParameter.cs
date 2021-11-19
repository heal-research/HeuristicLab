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
  [Item("MultiObjectiveProblemLookupParameter", "A parameter that looks up multi-objective problems with a given encoding by type.")]
  [StorableType("bb66b77c-12bd-4024-ad97-99c70489c2ee")]
  public class MultiObjectiveProblemLookupParameter<TEncoding, TEncodedSolution> : ContextLookupParameter<SingleObjectiveProblem<TEncoding, TEncodedSolution>>
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {

    [StorableConstructor]
    protected MultiObjectiveProblemLookupParameter(StorableConstructorFlag _) : base(_) { }
    protected MultiObjectiveProblemLookupParameter(MultiObjectiveProblemLookupParameter<TEncoding, TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveProblemLookupParameter() : base() { }
    public MultiObjectiveProblemLookupParameter(string name) : base(name) { }
    public MultiObjectiveProblemLookupParameter(string name, string description) : base(name, description) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveProblemLookupParameter<TEncoding, TEncodedSolution>(this, cloner);
    }
  }
}
