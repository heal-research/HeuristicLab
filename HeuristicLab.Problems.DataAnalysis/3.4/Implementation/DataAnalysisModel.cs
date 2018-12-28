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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("EF0EF64F-392A-448A-B30F-1AE113C6CC88")]
  [Item("DataAnalysisModel", "Base class for data analysis models.")]
  public abstract class DataAnalysisModel : NamedItem, IDataAnalysisModel {
    [StorableConstructor]
    protected DataAnalysisModel(StorableConstructorFlag _) : base(_) { }
    protected DataAnalysisModel(DataAnalysisModel original, Cloner cloner)
      : base(original, cloner) { }
    protected DataAnalysisModel() { }
    protected DataAnalysisModel(string name) : base(name) { }
    protected DataAnalysisModel(string name, string description) : base(name, description) { }

    public abstract IEnumerable<string> VariablesUsedForPrediction { get; }

    public virtual bool IsDatasetCompatible(IDataset dataset, out string errorMessage) {
      if (dataset == null) throw new ArgumentNullException("dataset", "The provided dataset is null.");
      return IsDatasetCompatible(this, dataset, out errorMessage);
    }

    public abstract bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage);

    public static bool IsDatasetCompatible(IDataAnalysisModel model, IDataset dataset, out string errorMessage) {
      if(model == null) throw new ArgumentNullException("model", "The provided model is null.");
      if (dataset == null) throw new ArgumentNullException("dataset", "The provided dataset is null.");
      errorMessage = string.Empty;

      foreach (var variable in model.VariablesUsedForPrediction) {
        if (!dataset.ContainsVariable(variable)) {
          if (string.IsNullOrEmpty(errorMessage)) {
            errorMessage = "The following variables must be present in the dataset for model evaluation:";
          }
          errorMessage += System.Environment.NewLine + " " + variable;
        }
      }

      return string.IsNullOrEmpty(errorMessage);
    }
  }
}
