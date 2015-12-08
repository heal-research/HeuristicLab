#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("DirectScheduleManipulator", "An operator which manipulates a direct schedule representation.")]
  [StorableClass]
  public abstract class DirectScheduleManipulator : ScheduleManipulator, IDirectScheduleOperator {
    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (LookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }

    [StorableConstructor]
    protected DirectScheduleManipulator(bool deserializing) : base(deserializing) { }
    protected DirectScheduleManipulator(DirectScheduleManipulator original, Cloner cloner) : base(original, cloner) { }

    public DirectScheduleManipulator()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Job data taken from the JSSP - Instance."));
    }

    protected abstract void Manipulate(IRandom random, Schedule individual);

    public override IOperation InstrumentedApply() {
      var schedule = ScheduleParameter.ActualValue as Schedule;
      if (schedule == null) throw new InvalidOperationException("Schedule was not found or is not of type Schedule.");
      Manipulate(RandomParameter.ActualValue, schedule);
      return base.InstrumentedApply();
    }

  }
}
