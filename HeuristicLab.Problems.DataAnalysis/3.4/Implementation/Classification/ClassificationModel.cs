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
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("7E6091F9-86FD-4C47-8935-9C35CAB4261B")]
  [Item("Classification Model", "Base class for all classification models.")]
  public abstract class ClassificationModel : DataAnalysisModel, IClassificationModel {
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
    protected ClassificationModel(StorableConstructorFlag _)
      : base(_) {
      targetVariable = string.Empty;
    }
    protected ClassificationModel(ClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      this.targetVariable = original.targetVariable;
    }

    protected ClassificationModel(string targetVariable)
      : base("Classification Model") {
      this.targetVariable = targetVariable;
    }
    protected ClassificationModel(string targetVariable, string name)
      : base(name) {
      this.targetVariable = targetVariable;
    }
    protected ClassificationModel(string targetVariable, string name, string description)
      : base(name, description) {
      this.targetVariable = targetVariable;
    }

    public abstract IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows);
    public abstract IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData);

    public virtual bool IsProblemDataCompatible(IClassificationProblemData problemData, out string errorMessage) {
      return IsProblemDataCompatible(this, problemData, out errorMessage);
    }

    public override bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage) {
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");
      var classificationProblemData = problemData as IClassificationProblemData;
      if (classificationProblemData == null)
        throw new ArgumentException("The problem data is not compatible with this classification model. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
      return IsProblemDataCompatible(classificationProblemData, out errorMessage);
    }

    public static bool IsProblemDataCompatible(IClassificationModel model, IClassificationProblemData problemData, out string errorMessage) {
      if (model == null) throw new ArgumentNullException("model", "The provided model is null.");
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");
      errorMessage = string.Empty;

      if (model.TargetVariable != problemData.TargetVariable)
        errorMessage = string.Format("The target variable of the model {0} does not match the target variable of the problemData {1}.", model.TargetVariable, problemData.TargetVariable);

      var evaluationErrorMessage = string.Empty;
      var datasetCompatible = model.IsDatasetCompatible(problemData.Dataset, out evaluationErrorMessage);
      if (!datasetCompatible)
        errorMessage += evaluationErrorMessage;

      return string.IsNullOrEmpty(errorMessage);
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
