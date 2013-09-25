#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Runtime.Serialization;
using HeuristicLab.Common;
using HeuristicLab.Core;
namespace HeuristicLab.Clients.Access {
  [Item("UserGroup", "A group.")]
  public partial class UserGroup : IDisposable {
    protected UserGroup(UserGroup original, Cloner cloner)
      : base(original, cloner) {
      this.Name = original.Name;
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.UserAccounts; }
    }

    public UserGroup()
      : base() {
      this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UserGroup_PropertyChanged);
    }

    [OnDeserialized]
    private void RegisterNamePropertyChangedEvent(StreamingContext c) {
      this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UserGroup_PropertyChanged);
    }

    void UserGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      //this is a workaround as we can't overide the Name property and fire directly 
      if (e.PropertyName == "Name") {
        OnToStringChanged();
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserGroup(this, cloner);
    }

    public override string ToString() {
      return Name;
    }

    #region IDisposable Members
    public void Dispose() {
      this.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(UserGroup_PropertyChanged);
    }
    #endregion
  }
}
