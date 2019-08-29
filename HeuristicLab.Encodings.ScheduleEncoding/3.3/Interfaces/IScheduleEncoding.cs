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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [StorableType("09b9d24c-2576-495a-b06c-338d095cba0d")]
  public interface IScheduleEncoding : IEncoding<ISchedule> {
    IFixedValueParameter<ItemList<Job>> JobDataParameter { get; set; }
    IFixedValueParameter<IntValue> JobsParameter { get; set; }
    IFixedValueParameter<IntValue> ResourcesParameter { get; set; }

    ItemList<Job> JobData { get; }
    int Jobs { get; set; }
    int Resources { get; set; }


    Schedule Decode(ISchedule schedule, ItemList<Job> jobData);
  }

  public interface IScheduleEncoding<TSchedule> : IEncoding<TSchedule>
    where TSchedule : class, ISchedule {

  }
}
