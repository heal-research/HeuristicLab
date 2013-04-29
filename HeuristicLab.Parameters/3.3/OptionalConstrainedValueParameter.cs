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
using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value has to be chosen from a set of valid values or is null.
  /// </summary>
  [Item("OptionalConstrainedValueParameter", "A parameter whose value has to be chosen from a set of valid values or is null.")]
  [StorableClass]
  public class OptionalConstrainedValueParameter<T> : Parameter, IConstrainedValueParameter<T> where T : class, IItem {
    public override Image ItemImage {
      get {
        if (value != null) return value.ItemImage;
        else return base.ItemImage;
      }
    }
      
    [Storable]
    private ItemSet<T> validValues;
    public IItemSet<T> ValidValues {
      get { return validValues; }
    }

    [Storable]
    private T value;
    public virtual T Value {
      get { return this.value; }
      set {
        if (value != this.value) {
          if ((value != null) && !validValues.Contains(value)) throw new ArgumentException("Invalid value.");
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
    [StorableConstructor]
    protected OptionalConstrainedValueParameter(bool deserializing) : base(deserializing) { }
    protected OptionalConstrainedValueParameter(OptionalConstrainedValueParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      validValues = cloner.Clone(original.validValues);
      value = cloner.Clone(original.value);
      getsCollected = original.getsCollected;
      Initialize();
    }
    public OptionalConstrainedValueParameter()
      : base("Anonymous", typeof(T)) {
      this.validValues = new ItemSet<T>();
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name)
      : base(name, typeof(T)) {
      this.validValues = new ItemSet<T>();
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, bool getsCollected)
      : base(name, typeof(T)) {
      this.validValues = new ItemSet<T>();
      this.getsCollected = getsCollected;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, ItemSet<T> validValues)
      : base(name, typeof(T)) {
      this.validValues = validValues;
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, ItemSet<T> validValues, bool getsCollected)
      : base(name, typeof(T)) {
      this.validValues = validValues;
      this.getsCollected = getsCollected;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, ItemSet<T> validValues, T value)
      : base(name, typeof(T)) {
      this.validValues = validValues;
      this.value = value;
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, ItemSet<T> validValues, T value, bool getsCollected)
      : base(name, typeof(T)) {
      this.validValues = validValues;
      this.value = value;
      this.getsCollected = getsCollected;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, string description)
      : base(name, description, typeof(T)) {
      this.validValues = new ItemSet<T>();
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, string description, bool getsCollected)
      : base(name, description, typeof(T)) {
      this.validValues = new ItemSet<T>();
      this.getsCollected = getsCollected;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, string description, ItemSet<T> validValues)
      : base(name, description, typeof(T)) {
      this.validValues = validValues;
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, string description, ItemSet<T> validValues, bool getsCollected)
      : base(name, description, typeof(T)) {
      this.validValues = validValues;
      this.getsCollected = getsCollected;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, string description, ItemSet<T> validValues, T value)
      : base(name, description, typeof(T)) {
      this.validValues = validValues;
      this.value = value;
      this.getsCollected = true;
      Initialize();
    }
    public OptionalConstrainedValueParameter(string name, string description, ItemSet<T> validValues, T value, bool getsCollected)
      : base(name, description, typeof(T)) {
      this.validValues = validValues;
      this.value = value;
      this.getsCollected = getsCollected;
      Initialize();
    }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      RegisterValidValuesEvents();
      RegisterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OptionalConstrainedValueParameter<T>(this, cloner);
    }

    public override string ToString() {
      return Name + ": " + (Value != null ? Value.ToString() : "null");
    }

    protected override IItem GetActualValue() {
      return Value;
    }
    protected override void SetActualValue(IItem value) {
      ((IValueParameter)this).Value = value;
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

    private void RegisterValidValuesEvents() {
      if (validValues != null) {
        validValues.ItemsAdded += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded);
        validValues.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
        validValues.CollectionReset += new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      }
    }
    private void DeregisterValidValuesEvents() {
      if (validValues != null) {
        validValues.ItemsAdded -= new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsAdded);
        validValues.ItemsRemoved -= new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
        validValues.CollectionReset -= new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      }
    }
    protected virtual void ValidValues_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) { }
    protected virtual void ValidValues_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if ((Value != null) && !validValues.Contains(Value)) Value = null;
    }
    protected virtual void ValidValues_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if ((Value != null) && !validValues.Contains(Value)) Value = null;
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
