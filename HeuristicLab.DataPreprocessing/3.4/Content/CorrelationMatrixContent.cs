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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("Feature Correlation Matrix", "Represents the feature correlation matrix.")]
  [StorableClass]
  public class CorrelationMatrixContent : PreprocessingContent, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Gradient; }
    }

    [Storable]
    public PreprocessingContext Context { get; private set; }


    public DataAnalysisProblemData ProblemData {
      get {
        var problemData = (DataAnalysisProblemData)Context.CreateNewProblemData();
        foreach (var input in problemData.InputVariables)
          problemData.InputVariables.SetItemCheckedState(input, true);
        // CorrelationView hides non-input columns per default
        return problemData;
      }
    }

    #region Constructor, Cloning & Persistence
    public CorrelationMatrixContent(PreprocessingContext context)
      : base(context.Data) {
      Context = context;
    }

    public CorrelationMatrixContent(CorrelationMatrixContent original, Cloner cloner)
      : base(original, cloner) {
      Context = original.Context;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CorrelationMatrixContent(this, cloner);
    }

    [StorableConstructor]
    protected CorrelationMatrixContent(bool deserializing)
      : base(deserializing) { }
    #endregion

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }
  }
}
