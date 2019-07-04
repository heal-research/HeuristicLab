#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// Represents a symbolic classification model
  /// </summary>
  [StorableClass]
  [Item(Name = "SymbolicClassificationModel", Description = "Represents a symbolic classification model.")]
  public abstract class SymbolicClassificationModel : SymbolicDataAnalysisModel, ISymbolicClassificationModel {
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
    protected SymbolicClassificationModel(bool deserializing)
      : base(deserializing) {
      targetVariable = string.Empty;
    }

    protected SymbolicClassificationModel(SymbolicClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      targetVariable = original.targetVariable;
    }

    protected SymbolicClassificationModel(string targetVariable, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter, lowerEstimationLimit, upperEstimationLimit) {
      this.targetVariable = targetVariable;
    }

    public abstract IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows);
    public abstract void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows);

    public abstract ISymbolicClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData);

    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
    }

    public void Scale(IClassificationProblemData problemData) {
      Scale(problemData, problemData.TargetVariable);
    }

    public virtual bool IsProblemDataCompatible(IClassificationProblemData problemData, out string errorMessage) {
      return ClassificationModel.IsProblemDataCompatible(this, problemData, out errorMessage);
    }

    public override bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage) {
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");
      var classificationProblemData = problemData as IClassificationProblemData;
      if (classificationProblemData == null)
        throw new ArgumentException("The problem data is not compatible with this classification model. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
      return IsProblemDataCompatible(classificationProblemData, out errorMessage);
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
