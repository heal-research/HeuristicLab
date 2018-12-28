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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.ScheduleEncoding.ScheduleEncoding {
  [Item("DirectScheduleManipulator", "An operator which manipulates a direct schedule representation.")]
  [StorableType("B5E31B46-3C2B-4DA5-AA5F-C0A88A4A8A26")]
  public abstract class DirectScheduleManipulator : ScheduleManipulator, IDirectScheduleOperator {

    [StorableConstructor]
    protected DirectScheduleManipulator(StorableConstructorFlag _) : base(_) { }
    protected DirectScheduleManipulator(DirectScheduleManipulator original, Cloner cloner) : base(original, cloner) { }
    public DirectScheduleManipulator()
      : base() {
      ScheduleEncodingParameter.ActualName = "Schedule";
    }

    protected abstract void Manipulate(IRandom random, Schedule individual);

    public override IOperation InstrumentedApply() {
      var schedule = ScheduleEncodingParameter.ActualValue as Schedule;
      if (schedule == null) throw new InvalidOperationException("ScheduleEncoding was not found or is not of type Schedule.");
      Manipulate(RandomParameter.ActualValue, schedule);
      return base.InstrumentedApply();
    }

  }
}
