#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "Scaling", Description = "Contains information about scaling of variables for data-analysis algorithms.")]
  public class Scaling : Item {
    [Storable]
    private Dictionary<string, Tuple<double, double>> scalingParameters = new Dictionary<string, Tuple<double, double>>();
    [StorableConstructor]
    protected Scaling(bool deserializing) : base(deserializing) { }
    protected Scaling(Scaling original, Cloner cloner)
      : base(original, cloner) {
      foreach (var pair in original.scalingParameters)
        scalingParameters.Add(pair.Key, Tuple.Create(pair.Value.Item1, pair.Value.Item2));
    }
    public Scaling(IDataset ds, IEnumerable<string> variables, IEnumerable<int> rows) {
      foreach (var variable in variables) {
        var values = ds.GetDoubleValues(variable, rows);
        var min = values.Where(x => !double.IsNaN(x)).Min();
        var max = values.Where(x => !double.IsNaN(x)).Max();
        scalingParameters[variable] = Tuple.Create(min, max);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Scaling(this, cloner);
    }

    public IEnumerable<double> GetScaledValues(IDataset ds, string variable, IEnumerable<int> rows) {
      double min = scalingParameters[variable].Item1;
      double max = scalingParameters[variable].Item2;
      if (min.IsAlmost(max)) return rows.Select(i => 0.0); // return enumerable of zeros
      return ds.GetDoubleValues(variable, rows).Select(x => (x - min) / (max - min));  // scale to range [0..1]
    }

    public void GetScalingParameters(string variable, out double min, out double max) {
      min = scalingParameters[variable].Item1;
      max = scalingParameters[variable].Item2;
    }
  }
}
