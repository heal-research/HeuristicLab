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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JSMManipulator", "An operator which manipulates a JSM representation.")]
  [StorableType("E190A576-B835-4343-9F0C-096E970B8DDD")]
  public abstract class JSMManipulator : ScheduleManipulator, IJSMOperator {
    [StorableConstructor]
    protected JSMManipulator(StorableConstructorFlag _) : base(_) { }
    protected JSMManipulator(JSMManipulator original, Cloner cloner) : base(original, cloner) { }
    public JSMManipulator()
      : base() {
      ScheduleEncodingParameter.ActualName = "JobSequenceMatrix";
    }

    protected abstract void Manipulate(IRandom random, IScheduleEncoding individual);

    public override IOperation InstrumentedApply() {
      var solution = ScheduleEncodingParameter.ActualValue as JSMEncoding;
      if (solution == null) throw new InvalidOperationException("ScheduleEncoding was not found or is not of type JSMEncoding.");
      Manipulate(RandomParameter.ActualValue, solution);
      return base.InstrumentedApply();
    }

  }
}
