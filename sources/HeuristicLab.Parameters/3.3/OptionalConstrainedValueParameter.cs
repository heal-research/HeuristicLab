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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value has to be chosen from a set of valid values or is null.
  /// </summary>
  [Item("OptionalConstrainedValueParameter<T>", "A parameter whose value has to be chosen from a set of valid values or is null.")]
  [StorableClass]
  public class OptionalConstrainedValueParameter<T> : Parameter, IValueParameter<T> where T : class, IItem {
    private ItemSet<T> validValues;
    [Storable]
    public ItemSet<T> ValidValues {
      get { return validValues; }
      private set {
        DeregisterValidValuesEvents();
        validValues = value;
        RegisterValidValuesEvents();
      }
    }

    private T value;
    [Storable]
    public virtual T Value {
      get { return this.value; }
      set {
        if (value != this.value) {
          if ((value != null) && !validValues.Contains(value)) throw new ArgumentException("Invalid value.");
          if (this.value != null) this.value.ToStringChanged -= new EventHandler(Value_ToStringChanged);
          this.value = value;
          if (this.value != null) this.value.ToStringChanged += new EventHandler(Value_ToStringChanged);
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

    public OptionalConstrainedValueParameter()
      : base("Anonymous", typeof(T)) {
      ValidValues = new ItemSet<T>();
    }
    public OptionalConstrainedValueParameter(string name)
      : base(name, typeof(T)) {
      ValidValues = new ItemSet<T>();
    }
    public OptionalConstrainedValueParameter(string name, ItemSet<T> validValues)
      : base(name, typeof(T)) {
      ValidValues = validValues;
    }
    public OptionalConstrainedValueParameter(string name, ItemSet<T> validValues, T value)
      : base(name, typeof(T)) {
      ValidValues = validValues;
      Value = value;
    }
    public OptionalConstrainedValueParameter(string name, string description)
      : base(name, description, typeof(T)) {
      ValidValues = new ItemSet<T>();
    }
    public OptionalConstrainedValueParameter(string name, string description, ItemSet<T> validValues)
      : base(name, description, typeof(T)) {
      ValidValues = validValues;
    }
    public OptionalConstrainedValueParameter(string name, string description, ItemSet<T> validValues, T value)
      : base(name, description, typeof(T)) {
      ValidValues = validValues;
      Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      OptionalConstrainedValueParameter<T> clone = (OptionalConstrainedValueParameter<T>)base.Clone(cloner);
      clone.ValidValues = (ItemSet<T>)cloner.Clone(validValues);
      clone.Value = (T)cloner.Clone(value);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, Value != null ? Value.ToString() : "null", DataType.GetPrettyName());
    }

    protected override IItem GetActualValue() {
      return Value;
    }
    protected override void SetActualValue(IItem value) {
      ((IValueParameter)this).Value = value;
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnToStringChanged();
    }

    private void RegisterValidValuesEvents() {
      if (validValues != null) {
        validValues.ItemsAdded += new CollectionItemsChangedEventHandler<T>(validValues_ItemsAdded);
        validValues.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
        validValues.CollectionReset += new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      }
    }

    private void DeregisterValidValuesEvents() {
      if (validValues != null) {
        validValues.ItemsAdded -= new CollectionItemsChangedEventHandler<T>(validValues_ItemsAdded);
        validValues.ItemsRemoved -= new CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
        validValues.CollectionReset -= new CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
      }
    }

    protected virtual void validValues_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) { }
    protected virtual void ValidValues_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if ((Value != null) && !validValues.Contains(Value)) Value = null;
    }
    protected virtual void ValidValues_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if ((Value != null) && !validValues.Contains(Value)) Value = null;
    }
    protected virtual void Value_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
  }
}
