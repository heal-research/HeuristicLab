#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents a result which has a name and a data type and holds an IItem.
  /// </summary>
  [Item("Result", "A result which has a name and a data type and holds an IItem.")]
  [StorableType("CDD8C915-3223-44E1-81A1-CA1CE86D2598")]
  public class Result : ResultDefinition, IResult, IStorableContent {
    public string Filename { get; set; }
    public override Image ItemImage {
      get {
        if (value != null) return value.ItemImage;
        else return base.ItemImage;
      }
    }

    [Storable]
    private IItem value;
    public IItem Value {
      get => value;
      set => SetValue(value);
    }

    public bool HasValue => Value != null;

    [StorableConstructor]
    protected Result(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterValueEvents();
    }

    protected Result(Result original, Cloner cloner)
      : base(original, cloner) {
      value = cloner.Clone(original.value);
      RegisterValueEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Result(this, cloner);
    }

    public Result(string name, Type dataType) : this(name, string.Empty, dataType) { }
    public Result(string name, string description, Type dataType) : base(name, description, dataType) {
      value = null;
    }

    public Result(string name, IItem value) : this(name, string.Empty, value.GetType(), value) { }
    public Result(string name, string description, IItem value) : this(name, description, value.GetType(), value) { }
    public Result(string name, string description, Type dataType, IItem value) : base(name, description, dataType) {
      this.value = value;
      RegisterValueEvents();
    }

    private void SetValue(IItem newValue) {
      if (value == newValue) return;
      if (newValue == null) throw new ArgumentNullException(nameof(Value));
      if (!DataType.IsInstanceOfType(newValue))
        throw new ArgumentException(string.Format("Type mismatch. Value is not a \"{0}\".", DataType.GetPrettyName()));

      DeregisterValueEvents();
      value = newValue;
      RegisterValueEvents();
      OnValueChanged();
    }

    public virtual void Reset() {
      DeregisterValueEvents();
      value = null;
      OnValueChanged();
    }

    public override string ToString() {
      if (value != null)
        return string.Format("{0}: {1}", Name, value.ToString());

      return base.ToString();
    }

    public event EventHandler ValueChanged;
    private void OnValueChanged() {
      ValueChanged?.Invoke(this, EventArgs.Empty);
      OnItemImageChanged();
      OnToStringChanged();
    }

    private void RegisterValueEvents() {
      if (value == null) return;

      value.ItemImageChanged += Value_ItemImageChanged;
      value.ToStringChanged += Value_ToStringChanged;
    }
    private void DeregisterValueEvents() {
      if (value == null) return;

      value.ItemImageChanged -= Value_ItemImageChanged;
      value.ToStringChanged -= Value_ToStringChanged;
    }
    private void Value_ItemImageChanged(object sender, EventArgs e) {
      OnItemImageChanged();
    }
    private void Value_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
  }

  /// <summary>
  /// Represents a result which has a name and a data type and holds an IItem.
  /// </summary>
  [Item("Result", "A typed result which has a name and a data type and holds a value of type T.")]
  [StorableType("BA883E2F-1E0B-4F05-A31A-7A0973CB63A3")]
  public sealed class Result<T> : Result, IResult<T>, IStorableContent
    where T : IItem {

    public new T Value {
      get { return (T)base.Value; }
      set { base.Value = value; }
    }

    [StorableConstructor]
    private Result(StorableConstructorFlag _) : base(_) { }
    private Result(Result<T> original, Cloner cloner) : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Result<T>(this, cloner);
    }

    //public Result(string name) : this(name, string.Empty) { }
    public Result(string name, string description) : base(name, description, typeof(T)) { }
    
    //public Result(string name, T value) : this(name, string.Empty, value) { }
    public Result(string name, string description, T value) : base(name, description, typeof(T), value) { }
  }
}
