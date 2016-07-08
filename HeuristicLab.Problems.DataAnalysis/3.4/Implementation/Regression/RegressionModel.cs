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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Regression Model", "Base class for all regression models.")]
  public abstract class RegressionModel : DataAnalysisModel, IRegressionModel {
    [Storable]
    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
      protected set { targetVariable = value; }
    }

    protected RegressionModel(bool deserializing) : base(deserializing) { }

    protected RegressionModel(RegressionModel original, Cloner cloner)
      : base(original, cloner) {
      this.targetVariable = original.targetVariable;
    }

    protected RegressionModel(string targetVariable)
      : base("Regression Model") {
      this.targetVariable = targetVariable;
    }
    protected RegressionModel(string targetVariable, string name)
      : base(name) {
      this.targetVariable = targetVariable;
    }
    protected RegressionModel(string targetVariable, string name, string description)
      : base(name, description) {
      this.targetVariable = targetVariable;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      targetVariable = string.Empty;
      #endregion
    }
    public abstract IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows);
    public abstract IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData);
  }
}
