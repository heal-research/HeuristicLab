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
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is defined in the parameter itself.
  /// </summary>
  [Item("ValueParameter", "A parameter whose value is defined in the parameter itself.")]
  [StorableType("CB9B83B6-DE2B-45CC-AB0B-C551E1A6E0BD")]
  public class ValueParameter<T> : OptionalValueParameter<T> where T : class, IItem {
    public override T Value {
      get { return base.Value; }
      set {
        if ((value == null) && (Value != null)) throw new ArgumentNullException();
        base.Value = value;
      }
    }

    [StorableConstructor]
    protected ValueParameter(StorableConstructorFlag _) : base(_) { }
    protected ValueParameter(ValueParameter<T> original, Cloner cloner) : base(original, cloner) { }

    public ValueParameter() : base() { }
    public ValueParameter(string name) : base(name) { base.Value = CreateDefaultValue(); }
    public ValueParameter(string name, bool getsCollected) : base(name, getsCollected) { base.Value = CreateDefaultValue(); }
    public ValueParameter(string name, T value) : base(name, value) { }
    public ValueParameter(string name, T value, bool getsCollected) : base(name, value, getsCollected) { }
    public ValueParameter(string name, string description) : base(name, description) { base.Value = CreateDefaultValue(); }
    public ValueParameter(string name, string description, bool getsCollected) : base(name, description, getsCollected) { base.Value = CreateDefaultValue(); }
    public ValueParameter(string name, string description, T value) : base(name, description, value) { }
    public ValueParameter(string name, string description, T value, bool getsCollected) : base(name, description, value, getsCollected) { }
                                                                                  
    protected T CreateDefaultValue() {
      Type type = typeof(T);
      if (type.IsAbstract) return null;
      ConstructorInfo defaultConstructor = type.GetConstructor(Type.EmptyTypes);
      if (defaultConstructor == null) return null;

      return (T)defaultConstructor.Invoke(new object[0]);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ValueParameter<T>(this, cloner);
    }
  }
}
