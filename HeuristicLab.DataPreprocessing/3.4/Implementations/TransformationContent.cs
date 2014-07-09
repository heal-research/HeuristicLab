#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Transformation", "Represents the transformation grid.")]
  public class TransformationContent : Item, IViewShortcut {

    public IPreprocessingData Data { get; private set; }
    public IFilterLogic FilterLogic { get; private set; }

    public ICheckedItemList<ITransformation> CheckedTransformationList { get; private set; }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }

    public TransformationContent(IPreprocessingData data, IFilterLogic filterLogic) {
      Data = data;
      CheckedTransformationList = new CheckedItemList<ITransformation>();
      FilterLogic = filterLogic;
    }

    public TransformationContent(TransformationContent original, Cloner cloner)
      : base(original, cloner) {
      Data = original.Data;
      CheckedTransformationList = new CheckedItemList<ITransformation>(original.CheckedTransformationList);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TransformationContent(this, cloner);
    }
  }
}
