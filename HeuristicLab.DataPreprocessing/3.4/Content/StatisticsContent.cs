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

namespace HeuristicLab.DataPreprocessing {
  [Item("Statistics", "Represents the statistics grid.")]
  [StorableClass]
  public class StatisticsContent : PreprocessingContent, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Object; }
    }

    #region Constructor, Cloning & Persistence
    public StatisticsContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public StatisticsContent(StatisticsContent original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new StatisticsContent(this, cloner);
    }

    [StorableConstructor]
    protected StatisticsContent(bool deserializing)
      : base(deserializing) { }
    #endregion

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }
  }
}
