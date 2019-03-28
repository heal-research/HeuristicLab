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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("DirectScheduleDecoder", "An item used to convert a direct schedule into a generalized schedule.")]
  [StorableType("151C772B-6E6C-40D9-B171-F5626C676A5F")]
  public class DirectScheduleDecoder : ScheduleDecoder<Schedule> {
    [StorableConstructor]
    protected DirectScheduleDecoder(StorableConstructorFlag _) : base(_) { }
    protected DirectScheduleDecoder(DirectScheduleDecoder original, Cloner cloner) : base(original, cloner) { }
    public DirectScheduleDecoder() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DirectScheduleDecoder(this, cloner);
    }

    public override Schedule DecodeSchedule(Schedule solution, ItemList<Job> jobData) {
      return Decode(solution, jobData);
    }

    public static Schedule Decode(Schedule solution, ItemList<Job> jobData) {
      return solution;
    }
  }
}
