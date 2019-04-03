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

namespace HeuristicLab.Optimization {
  [Item("MultiEncoding Crossover", "Applies different crossovers to cross a multi-encoding.")]
  [StorableType("BB0A04E2-899D-460C-82A2-5E4CEEDE8996")]
  internal sealed class MultiEncodingCrossover : MultiEncodingOperator<ICrossover>, ICrossover {

    [StorableConstructor]
    private MultiEncodingCrossover(StorableConstructorFlag _) : base(_) { }
    private MultiEncodingCrossover(MultiEncodingCrossover original, Cloner cloner) : base(original, cloner) { }
    public MultiEncodingCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) { return new MultiEncodingCrossover(this, cloner); }


    public override IOperation InstrumentedApply() {
      SolutionParameter.ActualValue = new CombinedSolution(ExecutionContext.Scope, (CombinedEncoding)EncodingParameter.ActualValue);
      return base.InstrumentedApply();
    }
  }
}
