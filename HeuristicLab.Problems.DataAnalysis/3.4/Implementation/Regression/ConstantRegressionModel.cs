#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Constant Regression Model", "A model that always returns the same constant value regardless of the presented input data.")]
  public class ConstantRegressionModel : NamedItem, IRegressionModel {
    [Storable]
    protected double constant;
    public double Constant {
      get { return constant; }
    }

    [StorableConstructor]
    protected ConstantRegressionModel(bool deserializing) : base(deserializing) { }
    protected ConstantRegressionModel(ConstantRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      this.constant = original.constant;
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new ConstantRegressionModel(this, cloner); }

    public ConstantRegressionModel(double constant)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.constant = constant;
    }

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      return rows.Select(row => Constant);
    }

    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new ConstantRegressionSolution(this, new RegressionProblemData(problemData));
    }
  }
}
