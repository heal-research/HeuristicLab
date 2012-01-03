#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive {

  public partial class Downtime : IDeepCloneable, IContent {
    public Downtime() { }

    public override void Store() {
      HiveAdminClient.Store(this, new System.Threading.CancellationToken());
      Modified = false;
    }

    protected Downtime(Downtime original, Cloner cloner)
      : base(original, cloner) {
      this.AllDayEvent = original.AllDayEvent;
      this.EndDate = original.EndDate;
      this.Recurring = original.Recurring;
      this.RecurringId = original.RecurringId;
      this.ResourceId = original.ResourceId;
      this.StartDate = original.StartDate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Downtime(this, cloner);
    }
  }
}
