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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// Represents a symbolic regression model
  /// </summary>
  [StorableType("2739C33E-4DDB-4285-9DFB-C056D900B2F2")]
  [Item(Name = "Symbolic Regression Model", Description = "Represents a symbolic regression model.")]
  public class SymbolicRegressionModel : SymbolicDataAnalysisModel, ISymbolicRegressionModel {
    [Storable]
    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
      set {
        if (string.IsNullOrEmpty(value) || targetVariable == value) return;
        targetVariable = value;
        OnTargetVariableChanged(this, EventArgs.Empty);
      }
    }

    [StorableConstructor]
    protected SymbolicRegressionModel(StorableConstructorFlag _) : base(_) {
      targetVariable = string.Empty;
    }

    protected SymbolicRegressionModel(SymbolicRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      this.targetVariable = original.targetVariable;
    }

    public SymbolicRegressionModel(string targetVariable, ISymbolicExpressionTree tree,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter, lowerEstimationLimit, upperEstimationLimit) {
      this.targetVariable = targetVariable;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
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

    public virtual bool IsProblemDataCompatible(IRegressionProblemData problemData, out string errorMessage) {
      return RegressionModel.IsProblemDataCompatible(this, problemData, out errorMessage);
    }

    public override bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage) {
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");
      var regressionProblemData = problemData as IRegressionProblemData;
      if (regressionProblemData == null)
        throw new ArgumentException("The problem data is not a regression problem data. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
      return IsProblemDataCompatible(regressionProblemData, out errorMessage);
    }

    #region events
    public event EventHandler TargetVariableChanged;
    private void OnTargetVariableChanged(object sender, EventArgs args) {
      var changed = TargetVariableChanged;
      if (changed != null)
        changed(sender, args);
    }
    #endregion
  }
}
