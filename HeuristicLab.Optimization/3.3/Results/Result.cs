#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents a result which has a name and a data type and holds an IItem.
  /// </summary>
  [Item("Result", "A result which has a name and a data type and holds an IItem.")]
  [StorableClass]
  public sealed class Result : NamedItem, IResult, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get {
        if (value != null) return value.ItemImage;
        else return base.ItemImage;
      }
    }

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [Storable]
    private Type dataType;
    public Type DataType {
      get { return dataType; }
    }

    [Storable]
    private IItem value;
    public IItem Value {
      get { return value; }
      set {
        if (this.value != value) {
          if ((value != null) && (!dataType.IsInstanceOfType(value)))
            throw new ArgumentException(
              string.Format("Type mismatch. Value is not a \"{0}\".",
                            dataType.GetPrettyName())
            );

          DeregisterValueEvents();
          this.value = value;
          RegisterValueEvents();
          OnValueChanged();
        }
      }
    }

    [StorableConstructor]
    private Result(bool deserializing) : base(deserializing) { }
    private Result(Result original, Cloner cloner)
      : base(original, cloner) {
      dataType = original.dataType;
      value = cloner.Clone(original.value);
      Initialize();
    }
    public Result()
      : base("Anonymous") {
      this.dataType = typeof(IItem);
      this.value = null;
    }
    public Result(string name, Type dataType)
      : base(name) {
      this.dataType = dataType;
      this.value = null;
    }
    public Result(string name, string description, Type dataType)
      : base(name, description) {
      this.dataType = dataType;
      this.value = null;
    }
    public Result(string name, IItem value)
      : base(name) {
      this.dataType = value == null ? typeof(IItem) : value.GetType();
      this.value = value;
      Initialize();
    }
    public Result(string name, string description, IItem value)
      : base(name, description) {
      this.dataType = value == null ? typeof(IItem) : value.GetType();
      this.value = value;
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Result(this, cloner);
    }

    private void Initialize() {
      RegisterValueEvents();
    }

    public override string ToString() {
      return string.Format("{0}: {1}", Name, Value == null ? "null" : Value.ToString());
    }

    public event EventHandler ValueChanged;
    private void OnValueChanged() {
      var handler = ValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
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
