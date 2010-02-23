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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value has to be chosen from a set of valid values which is defined it the parameter itself.
  /// </summary>
  [Item("ConstrainedValueParameter<T>", "A parameter whose value has to be chosen from a set of valid values which is defined it the parameter itself.")]
  public sealed class ConstrainedValueParameter<T> : Parameter, IValueParameter<T> where T : class, IItem {
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
    public T Value {
      get { return this.value; }
      set {
        if (value != this.value) {
          if ((value != null) && !validValues.Contains(value)) throw new ArgumentException("Invalid value.");
          this.value = value;
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

    public ConstrainedValueParameter()
      : base("Anonymous", typeof(T)) {
      ValidValues = new ItemSet<T>();
    }
    public ConstrainedValueParameter(string name)
      : base(name, typeof(T)) {
      ValidValues = new ItemSet<T>();
    }
    public ConstrainedValueParameter(string name, ItemSet<T> validValues)
      : base(name, typeof(T)) {
      ValidValues = validValues;
    }
    public ConstrainedValueParameter(string name, ItemSet<T> validValues, T value)
      : base(name, typeof(T)) {
      ValidValues = validValues;
      Value = value;
    }
    public ConstrainedValueParameter(string name, string description)
      : base(name, description, typeof(T)) {
      ValidValues = new ItemSet<T>();
    }
    public ConstrainedValueParameter(string name, string description, ItemSet<T> validValues)
      : base(name, description, typeof(T)) {
      ValidValues = validValues;
    }
    public ConstrainedValueParameter(string name, string description, ItemSet<T> validValues, T value)
      : base(name, description, typeof(T)) {
      ValidValues = validValues;
      Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ConstrainedValueParameter<T> clone = (ConstrainedValueParameter<T>)base.Clone(cloner);
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
    private void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnChanged();
    }

    private void RegisterValidValuesEvents() {
      if (validValues != null) {
        validValues.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
        validValues.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
        validValues.Changed += new ChangedEventHandler(ValidValues_Changed);
      }
    }
    private void DeregisterValidValuesEvents() {
      if (validValues != null) {
        validValues.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<T>(ValidValues_ItemsRemoved);
        validValues.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<T>(ValidValues_CollectionReset);
        validValues.Changed -= new ChangedEventHandler(ValidValues_Changed);
      }
    }

    private void ValidValues_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<T> e) {
      if ((Value != null) && !validValues.Contains(Value)) Value = null;
    }
    private void ValidValues_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<T> e) {
      if ((Value != null) && !validValues.Contains(Value)) Value = null;
    }
    private void ValidValues_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
