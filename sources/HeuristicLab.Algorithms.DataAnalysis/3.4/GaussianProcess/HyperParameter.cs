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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "HyperParameter", Description = "Represents a hyperparameter for Gaussian processes.")]
  public class HyperParameter<T> : OptionalValueParameter<T>, IValueParameter<T> where T : class, IItem {

    [Storable]
    private bool @fixed = false;

    public bool Fixed {
      get { return @fixed; }
    }

    [StorableConstructor]
    protected HyperParameter(bool deserializing)
      : base(deserializing) {
    }

    protected HyperParameter(HyperParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      this.@fixed = original.@fixed;
    }

    public HyperParameter() : base("HyperParameter", "Represents a hyperparameter for Gaussian processes.") { }

    public HyperParameter(string name, string description)
      : base(name, description) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HyperParameter<T>(this, cloner);
    }

    // if a value is set through this property then we fix the value of the hyperparameter 
    public override T Value {
      get {
        return base.Value;
      }
      set {
        base.Value = value;
        @fixed = value != null;
      }
    }

    // in the optimization we are allowed to set the value of non-fixed parameters
    public void SetValue(T value) {
      if (@fixed) throw new InvalidOperationException("Can't set the value of a fixed hyperparameter");
      base.Value = value;
    }
  }
}
