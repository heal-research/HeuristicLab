#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("Feature Correlation Matrix", "Represents the feature correlation matrix.")]
  public class CorrelationMatrixContent : Item, IViewChartShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Gradient; }
    }

    private PreprocessingContext Context { get; set; }
    private ITransactionalPreprocessingData PreprocessingData {
      get { return Context.Data; }
    }

    public DataAnalysisProblemData ProblemData {
      get {
        var problemData = (DataAnalysisProblemData)Context.CreateNewProblemData();
        foreach (var input in problemData.InputVariables)
          problemData.InputVariables.SetItemCheckedState(input, true);
        // CorrelationView hides non-input columns per default
        return problemData;
      }
    }

    public CorrelationMatrixContent(PreprocessingContext context) {
      Context = context;
    }

    public CorrelationMatrixContent(CorrelationMatrixContent original, Cloner cloner)
      : base(original, cloner) {
      Context = original.Context;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CorrelationMatrixContent(this, cloner);
    }

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }
  }
}
