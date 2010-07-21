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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is defined in the parameter itself.
  /// </summary>
  [Item("ValueParameter", "A parameter whose value is defined in the parameter itself.")]
  [StorableClass]
  public class ValueParameter<T> : OptionalValueParameter<T> where T : class, IItem {
    public override T Value {
      get { return base.Value; }
      set {
        if ((value == null) && (Value != null)) throw new ArgumentNullException();
        base.Value = value;
      }
    }

    public ValueParameter() : base() { }
    public ValueParameter(string name) : base(name) { }
    public ValueParameter(string name, T value) : base(name, value) { }
    public ValueParameter(string name, string description) : base(name, description) { }
    public ValueParameter(string name, string description, T value) : base(name, description, value) { }
  }
}
