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
using System.Collections.Generic;
using System.ComponentModel;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class Job : IDeepCloneable, IContent {
    public List<Guid> ResourceIds { get; set; }

    #region Constructors and Cloning
    public Job() {
      ProjectId = Guid.Empty;
      DateCreated = DateTime.Now;
      Permission = Permission.Full;
    }

    protected Job(Job original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      this.OwnerUserId = original.OwnerUserId;
      this.DateCreated = original.DateCreated;
      this.ProjectId = original.ProjectId;
      this.Name = original.Name;
      this.Description = original.Description;
      this.Id = original.Id;
      this.Permission = original.Permission;
      this.ResourceIds = original.ResourceIds;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Job(this, cloner);
    }
    #endregion

    #region Events
    public event EventHandler StateLogListChanged;
    private void OnStateLogListChanged() {
      var handler = StateLogListChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
      base.OnPropertyChanged(e);
      if (e.PropertyName == "Name") {
        OnToStringChanged();
      }
    }

    protected override void RaisePropertyChanged(string propertyName) {
      if (!(propertyName == "ExecutionTime")
        && !(propertyName == "JobCount")
        && !(propertyName == "CalculatingCount")
        && !(propertyName == "FinishedCount")) {
        base.RaisePropertyChanged(propertyName);
      }
    }

    public override string ToString() {
      return Name;
    }
  }
}
