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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("NamedItemWrapper", "A wrapper which wraps an INamedItem.")]
  [StorableClass]
  [NonDiscoverableType]
  public class NamedItemWrapper<T> : ItemWrapper<T>, INamedItem where T : class, INamedItem {
    protected override T WrappedItem {
      get { return base.WrappedItem; }
      set {
        if (value == null) throw new ArgumentNullException("WrappedItem", "WrappedItem cannot be null.");
        if (value != base.WrappedItem) {
          CancelEventArgs<string> e = new CancelEventArgs<string>(value.Name);
          OnNameChanging(e);
          if (!e.Cancel) base.WrappedItem = value;
        }
      }
    }

    public virtual string Name {
      get { return WrappedItem.Name; }
      set { WrappedItem.Name = value; }
    }
    public virtual bool CanChangeName {
      get { return WrappedItem.CanChangeName; }
    }
    public virtual string Description {
      get { return WrappedItem.Description; }
      set { WrappedItem.Description = value; }
    }
    public virtual bool CanChangeDescription {
      get { return WrappedItem.CanChangeDescription; }
    }

    [StorableConstructor]
    protected NamedItemWrapper(bool deserializing) : base(deserializing) { }
    protected NamedItemWrapper(NamedItemWrapper<T> original, Cloner cloner) : base(original, cloner) { }
    public NamedItemWrapper(T item) : base(item) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NamedItemWrapper<T>(this, cloner);
    }

    protected override void OnWrappedItemChanged() {
      base.OnWrappedItemChanged();
      OnNameChanged();
      OnDescriptionChanged();
    }

    #region Events
    public event EventHandler<CancelEventArgs<string>> NameChanging;
    protected virtual void OnNameChanging(CancelEventArgs<string> e) {
      var handler = NameChanging;
      if (handler != null) handler(this, e);
    }
    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    protected override void RegisterWrappedItemEvents() {
      base.RegisterWrappedItemEvents();
      WrappedItem.NameChanging += new EventHandler<CancelEventArgs<string>>(WrappedItem_NameChanging);
      WrappedItem.NameChanged += new EventHandler(WrappedItem_NameChanged);
      WrappedItem.DescriptionChanged += new EventHandler(WrappedItem_DescriptionChanged);
    }
    protected override void DeregisterWrappedItemEvents() {
      WrappedItem.NameChanging -= new EventHandler<CancelEventArgs<string>>(WrappedItem_NameChanging);
      WrappedItem.NameChanged -= new EventHandler(WrappedItem_NameChanged);
      WrappedItem.DescriptionChanged -= new EventHandler(WrappedItem_DescriptionChanged);
      base.DeregisterWrappedItemEvents();
    }

    protected virtual void WrappedItem_NameChanging(object sender, CancelEventArgs<string> e) {
      OnNameChanging(e);
    }
    protected virtual void WrappedItem_NameChanged(object sender, EventArgs e) {
      OnNameChanged();
    }
    protected virtual void WrappedItem_DescriptionChanged(object sender, EventArgs e) {
      OnDescriptionChanged();
    }
    #endregion
  }
}
