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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// Represents a symbolic regression solution (model + data) and attributes of the solution like accuracy and complexity
  /// </summary>
  [StorableClass]
  [Item(Name = "SymbolicRegressionSolution", Description = "Represents a symbolic regression solution (model + data) and attributes of the solution like accuracy and complexity.")]
  public sealed class SymbolicRegressionSolution : RegressionSolution, ISymbolicRegressionSolution {
    private const string ModelLengthResultName = "Model Length";
    private const string ModelDepthResultName = "Model Depth";

    public new ISymbolicRegressionModel Model {
      get { return (ISymbolicRegressionModel)base.Model; }
      set { base.Model = value; }
    }
    ISymbolicDataAnalysisModel ISymbolicDataAnalysisSolution.Model {
      get { return (ISymbolicDataAnalysisModel)base.Model; }
    }
    public int ModelLength {
      get { return ((IntValue)this[ModelLengthResultName].Value).Value; }
      private set { ((IntValue)this[ModelLengthResultName].Value).Value = value; }
    }

    public int ModelDepth {
      get { return ((IntValue)this[ModelDepthResultName].Value).Value; }
      private set { ((IntValue)this[ModelDepthResultName].Value).Value = value; }
    }

    [StorableConstructor]
    private SymbolicRegressionSolution(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionSolution(SymbolicRegressionSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicRegressionSolution(ISymbolicRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      Add(new Result(ModelLengthResultName, "Length of the symbolic regression model.", new IntValue()));
      Add(new Result(ModelDepthResultName, "Depth of the symbolic regression model.", new IntValue()));
      RecalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolution(this, cloner);
    }

    protected override void RecalculateResults() {
      base.RecalculateResults();
      ModelLength = Model.SymbolicExpressionTree.Length;
      ModelDepth = Model.SymbolicExpressionTree.Depth;
    }
  }
}
