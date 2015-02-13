#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// Represents a symbolic regression model
  /// </summary>
  [StorableClass]
  [Item(Name = "Symbolic Regression Model", Description = "Represents a symbolic regression model.")]
  public class SymbolicRegressionModel : SymbolicDataAnalysisModel, ISymbolicRegressionModel {


    [StorableConstructor]
    protected SymbolicRegressionModel(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionModel(SymbolicRegressionModel original, Cloner cloner) : base(original, cloner) { }

    public SymbolicRegressionModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter, lowerEstimationLimit, upperEstimationLimit) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      return Interpreter.GetSymbolicExpressionTreeValues(SymbolicExpressionTree, dataset, rows)
        .LimitToRange(LowerEstimationLimit, UpperEstimationLimit);
    }

    public ISymbolicRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new SymbolicRegressionSolution(this, new RegressionProblemData(problemData));
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }

    public void Scale(IRegressionProblemData problemData) {
      Scale(problemData, problemData.TargetVariable);
    }
  }
}
