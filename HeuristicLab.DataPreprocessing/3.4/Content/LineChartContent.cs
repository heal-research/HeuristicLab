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
  [Item("Line Chart", "Represents the line chart grid.")]
  [StorableClass]
  public class LineChartContent : PreprocessingChartContent {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    [Storable]
    public bool AllInOneMode { get; set; }


    #region Constructor, Cloning & Persistence
    public LineChartContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
      AllInOneMode = true;
    }

    public LineChartContent(LineChartContent original, Cloner cloner)
      : base(original, cloner) {
      AllInOneMode = original.AllInOneMode;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LineChartContent(this, cloner);
    }

    [StorableConstructor]
    protected LineChartContent(bool deserializing)
      : base(deserializing) { }
    #endregion
  }
}
