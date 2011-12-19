#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  [Item("SymbolicClassificationPearsonRSquaredEvaluator", "Calculates the pearson R² correlation coefficient of a symbolic classification solution.")]
  [StorableClass]
  public sealed class SymbolicClassificationPearsonRSquaredEvaluator : SymbolicRegressionPearsonsRSquaredEvaluator, ISymbolicClassificationEvaluator {
    public ClassificationProblemData ClassificationProblemData {
      get { return (ClassificationProblemData)RegressionProblemData; }
    }

    [StorableConstructor]
    private SymbolicClassificationPearsonRSquaredEvaluator(bool deserializing) : base(deserializing) { }
    private SymbolicClassificationPearsonRSquaredEvaluator(SymbolicClassificationPearsonRSquaredEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationPearsonRSquaredEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationPearsonRSquaredEvaluator(this, cloner);
    }
  }
}
