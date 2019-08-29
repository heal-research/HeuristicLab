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

using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ScheduleDecoder", "A schedule decoder translates a respresentation into an actual schedule.")]
  [StorableType("57A68F4B-4B35-4DB4-9B5E-D5154DD46E45")]
  public abstract class ScheduleDecoder<TSchedule> : SingleSuccessorOperator, IScheduleDecoder<TSchedule>
  where TSchedule : class, ISchedule {

    public ILookupParameter<ISchedule> ScheduleEncodingParameter {
      get { return (ILookupParameter<ISchedule>)Parameters["EncodedSchedule"]; }
    }
    public ILookupParameter<Schedule> ScheduleParameter {
      get { return (ILookupParameter<Schedule>)Parameters["Schedule"]; }
    }
    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (LookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }

    [StorableConstructor]
    protected ScheduleDecoder(StorableConstructorFlag _) : base(_) { }
    protected ScheduleDecoder(ScheduleDecoder<TSchedule> original, Cloner cloner) : base(original, cloner) { }
    public ScheduleDecoder()
      : base() {
      Parameters.Add(new LookupParameter<ISchedule>("EncodedSchedule", "The new scheduling solution represented as encoding."));
      Parameters.Add(new LookupParameter<Schedule>("Schedule", "The decoded scheduling solution represented as generalized schedule."));
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Job data taken from the JSSP - Instance."));
    }

    public Schedule DecodeSchedule(ISchedule schedule, ItemList<Job> jobData) {
      TSchedule solution = schedule as TSchedule;
      if (solution == null) throw new InvalidOperationException("Encoding is not of type " + typeof(TSchedule).GetPrettyName());
      return DecodeSchedule(solution, jobData);
    }
    public abstract Schedule DecodeSchedule(TSchedule schedule, ItemList<Job> jobData);

    public override IOperation Apply() {
      Schedule result = DecodeSchedule(ScheduleEncodingParameter.ActualValue, JobDataParameter.ActualValue);
      ScheduleParameter.ActualValue = result;
      return base.Apply();
    }
  }
}
