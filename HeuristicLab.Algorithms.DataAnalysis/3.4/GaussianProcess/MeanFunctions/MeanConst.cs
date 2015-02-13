#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanConst", Description = "Constant mean function for Gaussian processes.")]
  public sealed class MeanConst : ParameterizedNamedItem, IMeanFunction {
    public IValueParameter<DoubleValue> ValueParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Value"]; }
    }

    [StorableConstructor]
    private MeanConst(bool deserializing) : base(deserializing) { }
    private MeanConst(MeanConst original, Cloner cloner)
      : base(original, cloner) {
    }
    public MeanConst()
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;

      Parameters.Add(new OptionalValueParameter<DoubleValue>("Value", "The constant value for the constant mean function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanConst(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return ValueParameter.Value != null ? 0 : 1;
    }

    public void SetParameter(double[] p) {
      double c;
      GetParameters(p, out c);
      ValueParameter.Value = new DoubleValue(c);
    }

    private void GetParameters(double[] p, out double c) {
      if (ValueParameter.Value == null) {
        c = p[0];
      } else {
        if (p.Length > 0)
          throw new ArgumentException(
            "The length of the parameter vector does not match the number of free parameters for the constant mean function.",
            "p");
        c = ValueParameter.Value.Value;
      }
    }

    public ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, IEnumerable<int> columnIndices) {
      double c;
      GetParameters(p, out c);
      var mf = new ParameterizedMeanFunction();
      mf.Mean = (x, i) => c;
      mf.Gradient = (x, i, k) => {
        if (k > 0) throw new ArgumentException();
        return 1.0;
      };
      return mf;
    }
  }
}
