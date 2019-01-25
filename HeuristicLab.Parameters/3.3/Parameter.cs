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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A base class for parameters.
  /// </summary>
  [Item("Parameter", "A base class for parameters.")]
  [StorableType("EB8A33BD-466A-4035-8544-8F86E5FDA458")]
  public abstract class Parameter : NamedItem, IParameter {
    public override Image ItemImage {
      get {
        if ((dataType != null) && (typeof(IOperator).IsAssignableFrom(dataType)))
          return HeuristicLab.Common.Resources.VSImageLibrary.Method;
        else
          return base.ItemImage;
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

    [Storable(DefaultValue = false)]
    private bool hidden;
    public bool Hidden {
      get { return hidden; }
      set {
        if (value != hidden) {
          hidden = value;
          OnHiddenChanged();
        }
      }
    }

    public IItem ActualValue {
      get { return GetActualValue(); }
      set { SetActualValue(value); }
    }

    [StorableConstructor]
    protected Parameter(StorableConstructorFlag _) : base(_) {
    }
    protected Parameter(Parameter original, Cloner cloner)
      : base(original, cloner) {
      dataType = original.dataType;
      hidden = original.hidden;
    }
    protected Parameter()
      : base("Anonymous") {
      dataType = typeof(IItem);
      hidden = false;
    }
    protected Parameter(string name, Type dataType)
      : base(name) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
      hidden = false;
    }
    protected Parameter(string name, string description, Type dataType)
      : base(name, description) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
      hidden = false;
    }

    public override string ToString() {
      return Name;
    }

    protected abstract IItem GetActualValue();
    protected abstract void SetActualValue(IItem value);

    public event EventHandler HiddenChanged;
    protected virtual void OnHiddenChanged() {
      EventHandler handler = HiddenChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
