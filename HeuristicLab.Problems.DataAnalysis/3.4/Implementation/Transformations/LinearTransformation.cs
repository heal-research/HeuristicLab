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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Linear Transformation", "f(x) = k * x + d | Represents a linear transformation with multiplication and addition.")]
  public class LinearTransformation : Transformation<double> {
    protected const string MultiplierParameterName = "Multiplier";
    protected const string AddendParameterName = "Addend";

    #region Parameters
    public IValueParameter<DoubleValue> MultiplierParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[MultiplierParameterName]; }
    }
    public IValueParameter<DoubleValue> AddendParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[AddendParameterName]; }
    }
    #endregion

    #region properties
    public override string ShortName {
      get { return "Lin"; }
    }
    public double Multiplier {
      get { return MultiplierParameter.Value.Value; }
      set {
        MultiplierParameter.Value.Value = value;
      }
    }

    public double Addend {
      get { return AddendParameter.Value.Value; }
      set {
        AddendParameter.Value.Value = value;
      }
    }
    #endregion

    [StorableConstructor]
    protected LinearTransformation(bool deserializing) : base(deserializing) { }
    protected LinearTransformation(LinearTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public LinearTransformation(IEnumerable<string> allowedColumns)
      : base(allowedColumns) {
      Parameters.Add(new ValueParameter<DoubleValue>(MultiplierParameterName, "k | Multiplier for linear transformation", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(AddendParameterName, "d | Addend for linear transformation", new DoubleValue(0.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearTransformation(this, cloner);
    }

    public override IEnumerable<double> Apply(IEnumerable<double> data) {
      var m = Multiplier;
      var a = Addend;
      return data.Select(e => e * m + a);
    }

    public override bool Check(IEnumerable<double> data, out string errorMsg) {
      errorMsg = null;
      if (Multiplier.IsAlmost(0.0)) {
        errorMsg = String.Format("Multiplicand is 0, all {0} entries will be set to {1}. Inverse apply will not be possible (division by 0).", data.Count(), Addend);
        return false;
      }
      return true;
    }
  }
}
