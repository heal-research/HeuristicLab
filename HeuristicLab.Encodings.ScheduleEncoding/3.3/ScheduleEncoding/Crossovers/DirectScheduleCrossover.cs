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
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.ScheduleEncoding.ScheduleEncoding {
  [Item("DirectScheduleCrossover", "An operator which crosses two schedule representations.")]
  [StorableType("841CA966-FDB0-4C0D-BBE2-6A3422DF94CD")]
  public abstract class DirectScheduleCrossover : ScheduleCrossover, IDirectScheduleOperator {
    [StorableConstructor]
    protected DirectScheduleCrossover(StorableConstructorFlag _) : base(_) { }
    protected DirectScheduleCrossover(DirectScheduleCrossover original, Cloner cloner) : base(original, cloner) { }
    public DirectScheduleCrossover()
      : base() {
      ParentsParameter.ActualName = "Schedule";
      ChildParameter.ActualName = "Schedule";
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Job data taken from the JSSP - Instance."));
    }

    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (LookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }

    public abstract Schedule Cross(IRandom random, Schedule parent1, Schedule parent2);

    public override IOperation InstrumentedApply() {
      var parents = ParentsParameter.ActualValue;
      ChildParameter.ActualValue =
        Cross(RandomParameter.ActualValue, parents[0] as Schedule, parents[1] as Schedule);
      return base.InstrumentedApply();
    }
  }
}
