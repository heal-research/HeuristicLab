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

namespace HeuristicLab.DataPreprocessing {

  [Item("Manipulation", "Represents the available manipulations on a data set.")]
  public class ManipulationContent : Item, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }

    public ManipulationLogic ManipulationLogic { get; private set; }
    public SearchLogic SearchLogic { get; private set; }
    public FilterLogic FilterLogic { get; private set; }

    public ManipulationContent(ManipulationLogic manipulationLogic, SearchLogic searchLogic, FilterLogic filterLogic) {
      ManipulationLogic = manipulationLogic;
      SearchLogic = searchLogic;
      FilterLogic = filterLogic;
    }

    public ManipulationContent(ManipulationContent content, Cloner cloner) : base(content, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ManipulationContent(this, cloner);
    }
  }
}
