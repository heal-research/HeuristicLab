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
  /// A parameter whose value is defined in the parameter itself or is null.
  /// </summary>
  [Item("OptionalValueParameter<T>", "A parameter whose value is defined in the parameter itself or is null.")]
  [StorableClass]
  public class OptionalValueParameter<T> : Parameter, IValueParameter<T> where T : class, IItem {
    public override Image ItemImage {
      get {
        if (value != null) return value.ItemImage;
        else return base.ItemImage;
      }
    }

    [Storable]
    private T value;
    public virtual T Value {
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

    public OptionalValueParameter()
      : base("Anonymous", typeof(T)) {
    }
    public OptionalValueParameter(string name)
      : base(name, typeof(T)) {
    }
    public OptionalValueParameter(string name, T value)
      : base(name, typeof(T)) {
      this.value = value;
      Initialize();
    }
    public OptionalValueParameter(string name, string description)
      : base(name, description, typeof(T)) {
    }
    public OptionalValueParameter(string name, string description, T value)
      : base(name, description, typeof(T)) {
      this.value = value;
      Initialize();
    }
    [StorableConstructor]
    protected OptionalValueParameter(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      RegisterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      OptionalValueParameter<T> clone = (OptionalValueParameter<T>)base.Clone(cloner);
      clone.value = (T)cloner.Clone(value);
      clone.Initialize();
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1}", Name, Value != null ? Value.ToString() : "null");
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
      OnItemImageChanged();
      OnToStringChanged();
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
