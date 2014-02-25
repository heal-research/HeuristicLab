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

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JSMCrossover", "An operator which crosses two JSM representations.")]
  [StorableClass]
  public abstract class JSMCrossover : ScheduleCrossover, IJSMOperator {

    [StorableConstructor]
    protected JSMCrossover(bool deserializing) : base(deserializing) { }
    protected JSMCrossover(JSMCrossover original, Cloner cloner) : base(original, cloner) { }
    public JSMCrossover()
      : base() {
      ParentsParameter.ActualName = "JobSequenceMatrix";
      ChildParameter.ActualName = "JobSequenceMatrix";
    }

    public abstract JSMEncoding Cross(IRandom random, JSMEncoding parent1, JSMEncoding parent2);

    public override IOperation InstrumentedApply() {
      var parents = ParentsParameter.ActualValue;

      ChildParameter.ActualValue =
        Cross(RandomParameter.ActualValue, parents[0] as JSMEncoding, parents[1] as JSMEncoding);

      return base.InstrumentedApply();
    }
  }
}
