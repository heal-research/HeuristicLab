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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("Transformation", "Represents the transformation grid.")]
  [StorableClass]
  public class TransformationContent : PreprocessingContent, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }

    [Storable]
    public ICheckedItemList<ITransformation> CheckedTransformationList { get; private set; }

    #region Constructor, Cloning & Persistence
    public TransformationContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
      CheckedTransformationList = new CheckedItemList<ITransformation>();
    }

    public TransformationContent(TransformationContent original, Cloner cloner)
      : base(original, cloner) {
      CheckedTransformationList = cloner.Clone(original.CheckedTransformationList);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TransformationContent(this, cloner);
    }

    [StorableConstructor]
    protected TransformationContent(bool deserializing)
      : base(deserializing) { }
    #endregion
  }
}
