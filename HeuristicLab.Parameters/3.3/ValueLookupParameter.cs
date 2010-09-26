#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is either defined in the parameter itself or is retrieved from the scope.
  /// </summary>
  [Item("ValueLookupParameter", "A parameter whose value is either defined in the parameter itself or is retrieved from or written to a scope.")]
  [StorableClass]
  public class ValueLookupParameter<T> : LookupParameter<T>, IValueLookupParameter<T> where T : class, IItem {
    public override Image ItemImage {
      get {
        if (value != null) return value.ItemImage;
        else return base.ItemImage;
      }
    }

    [Storable]
    private T value;
    public T Value {
      get { return this.value; }
      set {
        if (value != this.value) {
          DeregisterValueEvents();
          this.value = value;
          RegisterValueEvents();
          OnValueChanged();
        }
      }
    }
    IItem IValueParameter.Value {
      get { return Value; }
      set {
        T val = value as T;
        if ((value != null) && (val == null))
          throw new InvalidOperationException(
            string.Format("Type mismatch. Value is not a \"{0}\".",
                          typeof(T).GetPrettyName())
          );
        Value = val;
      }
    }

    [Storable(DefaultValue = true)]
    private bool getsCollected;
    public bool GetsCollected {
      get { return getsCollected; }
      set {
        if (value != getsCollected) {
          getsCollected = value;
          OnGetsCollectedChanged();
        }
      }
    }

    #region Constructors
    public ValueLookupParameter()
      : base() {
      this.getsCollected = true;
    }
    public ValueLookupParameter(string name)
      : base(name) {
      this.getsCollected = true;
    }
    public ValueLookupParameter(string name, bool getsCollected)
      : base(name) {
      this.getsCollected = getsCollected;
    }
    public ValueLookupParameter(string name, T value)
      : base(name) {
      this.value = value;
      this.getsCollected = true;
      Initialize();
    }
    public ValueLookupParameter(string name, T value, bool getsCollected)
      : base(name) {
      this.value = value;
      this.getsCollected = getsCollected;
      Initialize();
    }
    public ValueLookupParameter(string name, string description)
      : base(name, description) {
      this.getsCollected = true;
    }
    public ValueLookupParameter(string name, string description, bool getsCollected)
      : base(name, description) {
      this.getsCollected = getsCollected;
    }
    public ValueLookupParameter(string name, string description, T value)
      : base(name, description) {
      this.value = value;
      this.getsCollected = true;
      Initialize();
    }
    public ValueLookupParameter(string name, string description, T value, bool getsCollected)
      : base(name, description) {
      this.value = value;
      this.getsCollected = getsCollected;
      Initialize();
    }
    public ValueLookupParameter(string name, string description, string actualName)
      : base(name, description, actualName) {
      this.getsCollected = true;
    }
    public ValueLookupParameter(string name, string description, string actualName, bool getsCollected)
      : base(name, description, actualName) {
      this.getsCollected = getsCollected;
    }
    [StorableConstructor]
    protected ValueLookupParameter(bool deserializing) : base(deserializing) { }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      RegisterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueLookupParameter<T> clone = (ValueLookupParameter<T>)base.Clone(cloner);
      clone.value = (T)cloner.Clone(value);
      clone.getsCollected = getsCollected;
      clone.Initialize();
      return clone;
    }

    public override string ToString() {
      if (Value != null)
        return Name + ": " + Value.ToString();
      else if (Name.Equals(ActualName))
        return Name;
      else
        return Name + ": " + ActualName;
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      EventHandler handler = ValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnItemImageChanged();
      OnToStringChanged();
    }
    public event EventHandler GetsCollectedChanged;
    protected virtual void OnGetsCollectedChanged() {
      EventHandler handler = GetsCollectedChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void RegisterValueEvents() {
      if (value != null) {
        value.ItemImageChanged += new EventHandler(Value_ItemImageChanged);
        value.ToStringChanged += new EventHandler(Value_ToStringChanged);
      }
    }
    private void DeregisterValueEvents() {
      if (value != null) {
        value.ItemImageChanged -= new EventHandler(Value_ItemImageChanged);
        value.ToStringChanged -= new EventHandler(Value_ToStringChanged);
      }
    }
    private void Value_ItemImageChanged(object sender, EventArgs e) {
      OnItemImageChanged();
    }
    private void Value_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
  }
}
