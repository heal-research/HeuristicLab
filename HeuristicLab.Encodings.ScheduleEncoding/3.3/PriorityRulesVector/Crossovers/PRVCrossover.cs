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

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PRVCrossover", "An operator which crosses two PRV representations.")]
  [StorableType("D5AD4BED-029A-4424-8512-F2F7E10D748D")]
  public abstract class PRVCrossover : ScheduleCrossover, IPRVOperator {

    [StorableConstructor]
    protected PRVCrossover(StorableConstructorFlag _) : base(_) { }
    protected PRVCrossover(PRVCrossover original, Cloner cloner) : base(original, cloner) { }
    public PRVCrossover() : base() { }

    public abstract PRVEncoding Cross(IRandom random, PRVEncoding parent1, PRVEncoding parent2);

    public override IOperation InstrumentedApply() {
      var parents = ParentsParameter.ActualValue;
      ChildParameter.ActualValue =
        Cross(RandomParameter.ActualValue, parents[0] as PRVEncoding, parents[1] as PRVEncoding);
      return base.InstrumentedApply();
    }
  }
}
