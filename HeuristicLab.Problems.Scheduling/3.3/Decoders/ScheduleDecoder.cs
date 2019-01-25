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
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [Item("ScheduleDecoder", "A schedule decoder translates a respresentation into an actual schedule.")]
  [StorableType("B48CB500-3983-4D83-98A8-3BC8FB167211")]
  public abstract class ScheduleDecoder : SingleSuccessorOperator, IScheduleDecoder {

    public ILookupParameter<IScheduleEncoding> ScheduleEncodingParameter {
      get { return (ILookupParameter<IScheduleEncoding>)Parameters["ScheduleEncoding"]; }
    }
    public ILookupParameter<Schedule> ScheduleParameter {
      get { return (ILookupParameter<Schedule>)Parameters["Schedule"]; }
    }

    [StorableConstructor]
    protected ScheduleDecoder(StorableConstructorFlag _) : base(_) { }
    protected ScheduleDecoder(ScheduleDecoder original, Cloner cloner) : base(original, cloner) { }
    public ScheduleDecoder()
      : base() {
      Parameters.Add(new LookupParameter<IScheduleEncoding>("ScheduleEncoding", "The new scheduling solution represented as encoding."));
      Parameters.Add(new LookupParameter<Schedule>("Schedule", "The decoded scheduling solution represented as generalized schedule."));
    }

    public abstract Schedule CreateScheduleFromEncoding(IScheduleEncoding solution);

    public override IOperation Apply() {
      Schedule result = CreateScheduleFromEncoding(ScheduleEncodingParameter.ActualValue);
      ScheduleParameter.ActualValue = result;
      return base.Apply();
    }
  }
}
