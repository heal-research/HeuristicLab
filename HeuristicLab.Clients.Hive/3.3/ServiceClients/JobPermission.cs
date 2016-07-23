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

using System.ComponentModel;
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class JobPermission : IDeepCloneable, IContent {

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

    public JobPermission() {
      this.Permission = Permission.Read;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
      base.OnPropertyChanged(e);
      if (e.PropertyName == "GrantedUserName" || e.PropertyName == "Permission") {
        HiveExperimentPermission_PropertyChanged(this, e);
      }
    }

    protected JobPermission(JobPermission original, Cloner cloner)
      : base(original, cloner) {
      this.GrantedByUserId = original.GrantedByUserId;
      this.GrantedUserId = original.GrantedUserId;
      this.JobId = original.JobId;
      this.Permission = original.Permission;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JobPermission(this, cloner);
    }

    private void HiveExperimentPermission_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnToStringChanged();
    }

    public override string ToString() {
      return string.Format("{0}: {1}", GrantedUserName, Permission.ToString());
    }
  }
}
