#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.PriorityRulesVector {
  [Item("PRVCrossover", "An operator which crosses two PRV representations.")]
  [StorableClass]
  public abstract class PRVCrossover : ScheduleCrossover, IPRVOperator {

    [StorableConstructor]
    protected PRVCrossover(bool deserializing) : base(deserializing) { }
    protected PRVCrossover(PRVCrossover original, Cloner cloner) : base(original, cloner) { }
    public PRVCrossover()
      : base() {
      ParentsParameter.ActualName = "PriorityRulesVector";
      ChildParameter.ActualName = "PriorityRulesVector";
    }

    public abstract PRVEncoding Cross(IRandom random, PRVEncoding parent1, PRVEncoding parent2);

    public override IOperation Apply() {
      var parents = ParentsParameter.ActualValue;
      ChildParameter.ActualValue =
        Cross(RandomParameter.ActualValue, parents[0] as PRVEncoding, parents[1] as PRVEncoding);
      return base.Apply();
    }
  }
}
