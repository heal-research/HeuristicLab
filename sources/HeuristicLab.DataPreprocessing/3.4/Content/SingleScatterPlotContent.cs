#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataPreprocessing {
  [Item("Scatter Plot", "Represents a scatter plot.")]
  [StorableClass]
  public class SingleScatterPlotContent : ScatterPlotContent {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    [Storable]
    public string SelectedXVariable { get; set; }
    [Storable]
    public string SelectedYVariable { get; set; }

    #region Constructor, Cloning & Persistence
    public SingleScatterPlotContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public SingleScatterPlotContent(SingleScatterPlotContent original, Cloner cloner)
      : base(original, cloner) {
      SelectedXVariable = original.SelectedXVariable;
      SelectedYVariable = original.SelectedYVariable;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleScatterPlotContent(this, cloner);
    }

    [StorableConstructor]
    protected SingleScatterPlotContent(bool deserializing)
      : base(deserializing) { }
    #endregion
  }
}
