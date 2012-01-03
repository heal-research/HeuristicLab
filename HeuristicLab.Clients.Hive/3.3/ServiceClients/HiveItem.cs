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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  public partial class HiveItem : IHiveItem {
    public string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }
    public static Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Database; }
    }
    public virtual Image ItemImage {
      get {
        if (Modified)
          return HeuristicLab.Common.Resources.VSImageLibrary.DatabaseModified;
        else
          return ItemAttribute.GetImage(this.GetType());
      }
    }

    private bool modified;
    public bool Modified {
      get { return modified; }
      internal set {
        if (value != modified) {
          modified = value;
          OnModifiedChanged();
          RaisePropertyChanged("Modified");
          OnItemImageChanged();
          RaisePropertyChanged("ItemImage");
        }
      }
    }

    #region Constructor and Cloning
    public HiveItem() {
      modified = true;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) {
      modified = false;
    }

    protected HiveItem(HiveItem original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      this.Id = original.Id;
      modified = true;
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new HiveItem(this, cloner);
    }

    public object Clone() {
      return Clone(new Cloner());
    }
    #endregion

    public virtual void Store() {
      HiveClient.Store(this, new System.Threading.CancellationToken());
      Modified = false;
    }

    protected virtual void RaisePropertyChanged(string propertyName) {
      OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
      if ((propertyName != "Id") && (propertyName != "Modified") && (propertyName != "ItemImage")) {
        Modified = true;
      }
    }
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, e);
    }

    #region Event handler
    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ModifiedChanged;
    protected virtual void OnModifiedChanged() {
      EventHandler handler = ModifiedChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    public override string ToString() {
      return ItemName;
    }
  }
}
