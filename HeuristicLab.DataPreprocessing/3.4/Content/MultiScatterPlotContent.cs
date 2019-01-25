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
using HEAL.Attic;

namespace HeuristicLab.DataPreprocessing {
  [Item("Multi Scatter Plot", "Represents a multi scatter plot.")]
  [StorableType("1F55FF9A-2EE3-4A5B-BEF4-8F19D9A3A54D")]
  public class MultiScatterPlotContent : ScatterPlotContent {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    #region Constructor, Cloning & Persistence
    public MultiScatterPlotContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public MultiScatterPlotContent(MultiScatterPlotContent original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiScatterPlotContent(this, cloner);
    }

    [StorableConstructor]
    protected MultiScatterPlotContent(StorableConstructorFlag _) : base(_) { }
    #endregion
  }
}
