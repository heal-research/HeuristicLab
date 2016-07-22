#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  public class HiveDowntime : Calendar.Appointment {
    private Guid recurringId = Guid.Empty;

    public HiveDowntime()
      : base() {
      Changed = false;
    }

    public bool Changed { get; set; }

    public new bool AllDayEvent {
      get { return base.AllDayEvent; }
      set { base.AllDayEvent = value; Changed = true; }
    }

    public new bool Recurring {
      get { return base.Recurring; }
      set { base.Recurring = value; Changed = true; }
    }

    public Guid RecurringId {
      get { return recurringId; }
      set { recurringId = value; Changed = true; }
    }

    public new DateTime EndDate {
      get { return base.EndDate; }
      set { base.EndDate = value; Changed = true; }
    }

    public new DateTime StartDate {
      get { return base.StartDate; }
      set { base.StartDate = value; Changed = true; }
    }

    public bool Deleted { get; set; }

    public Guid Id { get; set; }
  }
}
