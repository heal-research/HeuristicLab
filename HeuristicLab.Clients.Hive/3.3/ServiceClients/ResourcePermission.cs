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

using System.ComponentModel;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class ProjectPermission : IDeepCloneable, IContent {

    private string grantedUserName;
    public string GrantedUserName {
      get { return grantedUserName; }
      set {
        if (value != grantedUserName) {
          grantedUserName = value;
          RaisePropertyChanged("GrantedUserName");
        }
      }
    }

    public void UnmodifiedGrantedUserNameUpdate(string userName) {
      grantedUserName = userName;
    }

    public ProjectPermission() { }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
      base.OnPropertyChanged(e);
      if (e.PropertyName == "GrantedUserName") {
        HiveExperimentPermission_PropertyChanged(this, e);
      }
    }

    protected ProjectPermission(ProjectPermission original, Cloner cloner)
      : base(original, cloner) {
      this.GrantedByUserId = original.GrantedByUserId;
      this.GrantedUserId = original.GrantedUserId;
      this.ProjectId = original.ProjectId;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProjectPermission(this, cloner);
    }

    private void HiveExperimentPermission_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnToStringChanged();
    }

    public override string ToString() {
      return string.Format("{0}", GrantedUserName);
    }
  }
}
