﻿#region License Information
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
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class Project : IDeepCloneable, IContent {
    public Project() {
      Description = string.Empty;
      DateCreated = DateTime.Now;
      StartDate = DateCreated;
      EndDate = DateCreated.AddMonths(3);
    }

    protected Project(Project original, Cloner cloner) : base(original, cloner) {
      ParentProjectId = original.ParentProjectId;
      DateCreated = original.DateCreated;
      OwnerUserId = original.OwnerUserId;
      StartDate = original.StartDate;
      EndDate = original.EndDate;
    }
  }
}
